using FusionLibrary.Extensions;
using GTA;
using GTA.Math;
using GTA.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using static FusionLibrary.Enums;

namespace FusionLibrary
{
    public static class Utils
    {
        public static Random Random = new Random(DateTime.Now.Millisecond);

        internal static CustomModel DMC12 = new CustomModel("dmc12");
        internal static CustomModel DMC12Debug = new CustomModel("dmc_debug");

        internal static CustomModel SierraVisibleModel = new CustomModel("sierra");
        internal static CustomModel SierraTenderModel = new CustomModel("sierratender");
        internal static CustomModel SierraModel = new CustomModel("sierra_debug");

        private static int _padShakeStop;

        public static DateTime CurrentTime
        {
            get => GetWorldTime();
            set => SetWorldTime(value);
        }

        public static Ped PlayerPed => Game.Player.Character;

        public static Vehicle PlayerVehicle => PlayerPed.CurrentVehicle;

        public static bool HideGUI { get; set; } = false;

        private static bool randomTrains = true;
        public static bool RandomTrains
        {
            get => randomTrains;
            set
            {
                Function.Call(Hash.SET_RANDOM_TRAINS, value);

                if (!value)
                    Function.Call(Hash.DELETE_ALL_TRAINS);

                randomTrains = value;
            }
        }

        public static Model LoadAndRequestModel(string modelName)
        {
            Model model = new Model(modelName);

            return LoadAndRequestModel(model);
        }

        public static Model LoadAndRequestModel(Model model)
        {
            if (!model.IsInCdImage || !model.IsValid)
                throw new Exception(model + " not present!");

            model.Request();

            while (!model.IsLoaded)
                Script.Yield();

            return model;
        }

        public static void ClearWorld()
        {
            Function.Call(Hash.DELETE_ALL_TRAINS);

            Vehicle[] allVehicles = World.GetAllVehicles();

            allVehicles.Where(x => x.NotNullAndExists() && !x.IsTimeMachine2() && PlayerVehicle != x && x.Model != DMC12Debug && x.Model != SierraModel && x.Model != SierraVisibleModel && x.Model != SierraTenderModel).ToList()
                .ForEach(x => x?.DeleteCompletely());

            Ped[] allPeds = World.GetAllPeds();

            allPeds.Where(x => x.NotNullAndExists() && x != PlayerPed && (!PlayerVehicle.NotNullAndExists() || !PlayerVehicle.Passengers.Contains(x))).ToList()
                .ForEach(x => x?.Delete());
        }

        public static Vector3 DirectionToRotation(Vector3 src, Vector3 dst, float roll)
        {
            Vector3 dir = src.GetDirectionTo(dst);
            Vector3 rotval = Vector3.Zero;
            rotval.Z = -(((float)Math.Atan2(dir.X, dir.Y)).ToDeg());
            Vector3 rotpos = Vector3.Normalize(new Vector3(dir.Z, new Vector3(dir.X, dir.Y, 0.0f).Length(), 0.0f));
            rotval.X = ((float)Math.Atan2(rotpos.X, rotpos.Y)).ToDeg();
            rotval.Y = roll;
            return rotval;
        }

        public static bool IsPadShaking => _padShakeStop >= Game.GameTime;

        public static void SetPadShake(int duration, int frequency)
        {
            _padShakeStop = Game.GameTime + duration;

            Function.Call(Hash.SET_PAD_SHAKE, 0, duration, frequency);
        }

        public static void StopPadShake()
        {
            _padShakeStop = 0;

            Function.Call(Hash.STOP_PAD_SHAKE);
        }

        private static DateTime GetWorldTime()
        {
            try
            {
                int month = Function.Call<int>(Hash.GET_CLOCK_MONTH) + 1;
                int year = Function.Call<int>(Hash.GET_CLOCK_YEAR);
                int day = Function.Call<int>(Hash.GET_CLOCK_DAY_OF_MONTH);
                int hour = Function.Call<int>(Hash.GET_CLOCK_HOURS);
                int minute = Function.Call<int>(Hash.GET_CLOCK_MINUTES);
                int second = Function.Call<int>(Hash.GET_CLOCK_SECONDS);

                return new DateTime(year, month, day, hour, minute, second);
            }
            catch (Exception)
            {
                Function.Call(Hash.SET_CLOCK_DATE, 1985, 9, 21);
            }

            return DateTime.MinValue;
        }

        private static void SetWorldTime(DateTime time)
        {
            Function.Call(Hash.SET_CLOCK_DATE, time.Day, time.Month - 1, time.Year);
            Function.Call(Hash.SET_CLOCK_TIME, time.Hour, time.Minute, time.Second);
        }

        // https://code.google.com/archive/p/slimmath/
        public static bool Decompose(Matrix matrix, out Vector3 scale, out Quaternion rotation, out Vector3 translation)
        {
            const float ZeroTolerance = 1e-6f;

            rotation = Quaternion.Identity;
            translation = Vector3.Zero;
            scale = Vector3.Zero;

            //Source: Unknown
            //References: http://www.gamedev.net/community/forums/topic.asp?topic_id=441695

            //Get the translation.
            translation.X = matrix.M41;
            translation.Y = matrix.M42;
            translation.Z = matrix.M43;

            //Scaling is the length of the rows.
            scale.X = (float)Math.Sqrt((matrix.M11 * matrix.M11) + (matrix.M12 * matrix.M12) + (matrix.M13 * matrix.M13));
            scale.Y = (float)Math.Sqrt((matrix.M21 * matrix.M21) + (matrix.M22 * matrix.M22) + (matrix.M23 * matrix.M23));
            scale.Z = (float)Math.Sqrt((matrix.M31 * matrix.M31) + (matrix.M32 * matrix.M32) + (matrix.M33 * matrix.M33));

            //If any of the scaling factors are zero, than the rotation matrix can not exist.
            if (Math.Abs(scale.X) < ZeroTolerance ||
                Math.Abs(scale.Y) < ZeroTolerance ||
                Math.Abs(scale.Z) < ZeroTolerance)
            {
                rotation = Quaternion.Identity;
                return false;
            }

            //The rotation is the left over matrix after dividing out the scaling.
            Matrix rotationmatrix = new Matrix
            {
                M11 = matrix.M11 / scale.X,
                M12 = matrix.M12 / scale.X,
                M13 = matrix.M13 / scale.X,

                M21 = matrix.M21 / scale.Y,
                M22 = matrix.M22 / scale.Y,
                M23 = matrix.M23 / scale.Y,

                M31 = matrix.M31 / scale.Z,
                M32 = matrix.M32 / scale.Z,
                M33 = matrix.M33 / scale.Z,

                M44 = 1f
            };

            rotation = Quaternion.RotationMatrix(rotationmatrix);
            return true;
        }

        public static float Clamp(float value, float max, float max2)
        {
            return (value * max2) / max;
        }

        public static float Lerp(float minValue, float maxValue, float by)
        {
            return minValue + (maxValue - minValue) * by;
        }

        public static int Lerp(int firstFloat, int secondFloat, float by)
        {
            return (int)(firstFloat + (secondFloat - (float)firstFloat) * by);
        }

        public static readonly string[] WheelNames = new string[4]
        {
            "wheel_lf",
            "wheel_lr",
            "wheel_rr",
            "wheel_rf"
        };

        public static Vehicle CreateMissionTrain(int var, Vector3 pos, bool direction)
        {
            return Function.Call<Vehicle>(Hash.CREATE_MISSION_TRAIN, var, pos.X, pos.Y, pos.Z, direction);
        }

        public static void LiftUpWheel(Vehicle vehicle, WheelId id, float height)
        {
            //_SET_HYDRAULIC_STATE
            Function.Call((Hash)0x84EA99C62CB3EF0C, vehicle, id, height);
        }

        public static DateTime? ParseFromRawString(string raw, DateTime currentTime)
        {
            try
            {
                if (raw.Length == 12)
                {
                    string month = raw.Substring(0, 2);
                    string day = raw.Substring(2, 2);
                    string year = raw.Substring(4, 4);
                    string hour = raw.Substring(8, 2);
                    string minute = raw.Substring(10, 2);

                    return new DateTime(int.Parse(year), int.Parse(month), int.Parse(day), int.Parse(hour), int.Parse(minute), 0);
                }
                else if (raw.Length == 8)
                {
                    string month = raw.Substring(0, 2);
                    string day = raw.Substring(2, 2);
                    string year = raw.Substring(4, 4);

                    return new DateTime(int.Parse(year), int.Parse(month), int.Parse(day), currentTime.Hour, currentTime.Minute, 0);
                }
                else if (raw.Length == 4)
                {
                    string hour = raw.Substring(0, 2);
                    string minute = raw.Substring(2, 2);

                    return new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, int.Parse(hour), int.Parse(minute), 0);
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static float Distance2DBetween(Entity entity1, Entity entity2)
        {
            return entity1.Position.DistanceTo2D(entity2.Position);
        }

        public static void DrawLine(Vector3 from, Vector3 to, Color col)
        {
            Function.Call(Hash.DRAW_LINE, from.X, from.Y, from.Z, to.X, to.Y, to.Z, col.R, col.G, col.B, col.A);
        }

        public static Vector3 GetPositionOnGround(Vector3 position, float verticalOffset)
        {
            float result = -1;

            position.Z += 2.5f;
            unsafe
            {
                Function.Call(Hash.GET_GROUND_Z_FOR_3D_COORD, position.X, position.Y, position.Z, &result, false);
            }
            position.Z = result + verticalOffset;

            return position;
        }

        public static void DisplayHelpText(string text)
        {
            Function.Call(Hash.BEGIN_TEXT_COMMAND_DISPLAY_HELP, "STRING");
            Function.Call(Hash.ADD_TEXT_COMPONENT_SUBSTRING_PLAYER_NAME, text);
            Function.Call(Hash.END_TEXT_COMMAND_DISPLAY_HELP, 0, 0, 1, -1);
        }

        public static Vector3 GetWaypointPosition()
        {
            if (!Game.IsWaypointActive)
                return Vector3.Zero;

            bool blipFound = false;
            Vector3 position = Vector3.Zero;

            int it = Function.Call<int>(Hash._GET_BLIP_INFO_ID_ITERATOR);
            for (int i = Function.Call<int>(Hash.GET_FIRST_BLIP_INFO_ID, it); Function.Call<bool>(Hash.DOES_BLIP_EXIST, i); i = Function.Call<int>(Hash.GET_NEXT_BLIP_INFO_ID, it))
            {
                if (Function.Call<int>(Hash.GET_BLIP_INFO_ID_TYPE, i) == 4)
                {
                    position = Function.Call<Vector3>(Hash.GET_BLIP_INFO_ID_COORD, i);
                    blipFound = true;
                    break;
                }
            }

            if (blipFound)
            {
                do
                {
                    position.RequestCollision();
                    Script.Yield();
                    position.Z = World.GetGroundHeight(new Vector2(position.X, position.Y));
                } while (position.Z == 0);
            }

            return position;
        }

        public static bool IsAnyOfFrontDoorsOpen(Vehicle vehicle)
        {
            bool doorOpen = false;
            foreach (VehicleDoor door in vehicle.Doors)
            {
                if (door.IsOpen)
                    doorOpen = true;
            }

            return doorOpen;
        }

        public static bool IsPlayerUseFirstPerson()
        {
            return Function.Call<int>(Hash.GET_FOLLOW_PED_CAM_VIEW_MODE) == 4 && !GameplayCamera.IsLookingBehind && !Function.Call<bool>((Hash)0xF5F1E89A970B7796);
        }

        public static bool IsAllTiresBurst(Vehicle vehicle)
        {
            if (!Function.Call<bool>(Hash.IS_VEHICLE_TYRE_BURST, vehicle, (int)WheelId.FrontLeft, true))
                return false;

            if (!Function.Call<bool>(Hash.IS_VEHICLE_TYRE_BURST, vehicle, (int)WheelId.FrontRight, true))
                return false;

            if (!Function.Call<bool>(Hash.IS_VEHICLE_TYRE_BURST, vehicle, (int)WheelId.RearRight, true))
                return false;

            if (!Function.Call<bool>(Hash.IS_VEHICLE_TYRE_BURST, vehicle, (int)WheelId.RearLeft, true))
                return false;

            return true;
        }

        public static bool IsAnyTireBurst(Vehicle vehicle)
        {
            if (Function.Call<bool>(Hash.IS_VEHICLE_TYRE_BURST, vehicle, (int)WheelId.FrontLeft, true))
                return true;

            if (Function.Call<bool>(Hash.IS_VEHICLE_TYRE_BURST, vehicle, (int)WheelId.FrontRight, true))
                return true;

            if (Function.Call<bool>(Hash.IS_VEHICLE_TYRE_BURST, vehicle, (int)WheelId.RearRight, true))
                return true;

            if (Function.Call<bool>(Hash.IS_VEHICLE_TYRE_BURST, vehicle, (int)WheelId.RearLeft, true))
                return true;

            return false;
        }

        public static void SetTiresBurst(Vehicle vehicle, bool toggle)
        {
            if (toggle)
            {
                vehicle.Wheels[(int)WheelId.FrontLeft].Burst();
                vehicle.Wheels[(int)WheelId.FrontRight].Burst();
                vehicle.Wheels[(int)WheelId.RearRight].Burst();
                vehicle.Wheels[(int)WheelId.RearLeft].Burst();
            }
            else
            {
                vehicle.Wheels[(int)WheelId.FrontLeft].Fix();
                vehicle.Wheels[(int)WheelId.FrontRight].Fix();
                vehicle.Wheels[(int)WheelId.RearRight].Fix();
                vehicle.Wheels[(int)WheelId.RearLeft].Fix();
            }
        }

        public static float Magnitude(Vector3 vector3)
        {
            return Function.Call<float>(Hash.VMAG2, vector3.X, vector3.Z, vector3.Y);
        }

        public static List<Vector3> GetWheelsPositions(Vehicle vehicle)
        {
            return new List<Vector3>
                    {
                        vehicle.Bones["wheel_lf"].Position,
                        vehicle.Bones["wheel_rf"].Position,
                        vehicle.Bones["wheel_rr"].Position,
                        vehicle.Bones["wheel_lr"].Position
                    };
        }

        public static bool IsVehicleOnTracks(Vehicle vehicle)
        {
            return GetWheelsPositions(vehicle).TrueForAll(x => IsWheelOnTracks(x, vehicle));
        }

        public static bool IsWheelOnTracks(Vector3 pos, Vehicle vehicle)
        {
            // What it basicly does is drawing circle around that "pos" so we can
            //  detect if wheel position is near tracks position

            //                                 **    **
            //                            **              **
            //                                 ||    ||
            //                        **       ||    ||       **
            //                                 ||    ||
            //                      **         ||    ||         **
            //                                 ||    || 
            //                      **         ||    ||         **
            //                                 ||    ||  
            //                       **        ||    ||        **
            //                                 ||    || 
            //                          **                  **
            //                                 **    ** 

            const float r = 0.15f;
            for (float i = 0; i <= 360; i += 30)
            {
                float angleRad = i * (float)Math.PI / 180;

                double x = r * Math.Cos(angleRad);
                double y = r * Math.Sin(angleRad);

                Vector3 circlePos = pos;
                circlePos.X += (float)x;
                circlePos.Y += (float)y;

                //DrawLine(pos, circlePos, Color.Aqua);

                // Then we check for every pos if it hits tracks material
                RaycastResult surface = World.Raycast(circlePos, circlePos + new Vector3(0, 0, -10),
                    IntersectFlags.Everything, vehicle);

                // Tracks materials
                List<MaterialHash> allowedSurfaces = new List<MaterialHash>
                {
                    MaterialHash.MetalSolidRoadSurface,
                    MaterialHash.MetalSolidSmall,
                    MaterialHash.MetalSolidMedium
                };

                if (allowedSurfaces.Contains(surface.MaterialHash))
                    return true;
            }
            return false;
        }

        public static DateTime RandomDate()
        {
            Random rand = new Random();

            int second = rand.Next(0, 59);
            int minute = rand.Next(0, 59);
            int hour = rand.Next(0, 23);
            int month = rand.Next(1, 12);
            int year = rand.Next(1, 9999);
            int day = rand.Next(1, DateTime.DaysInMonth(year, month));

            return new DateTime(year, month, day, hour, minute, second);
        }

        public static string RemoveIllegalFileNameChars(string input, string replacement = "")
        {
            string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            Regex r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
            return r.Replace(input, replacement);
        }
    }
}
