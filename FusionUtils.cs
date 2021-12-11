using FusionLibrary.Extensions;
using GTA;
using GTA.Math;
using GTA.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using static FusionLibrary.FusionEnums;

namespace FusionLibrary
{
    public static class FusionUtils
    {
        /// <summary>
        /// Checks if first tick has gone.
        /// </summary>
        public static bool FirstTick { get; internal set; } = true;

        /// <summary>
        /// Represents a pseudo-random number generator, which is an algorithm that produces a sequence of numbers that meet certain statistical requirements for randomness.
        /// </summary>
        public static Random Random = new Random(DateTime.Now.Millisecond);

        //internal static Model DMC12 = new Model("dmc12");

        /// <summary>
        /// Serializes and deserializes an object, or an entire graph of connected objects, in binary format.
        /// </summary>
        public static BinaryFormatter BinaryFormatter { get; } = new BinaryFormatter();

        private static int _padShakeStop;

        /// <summary>
        /// Gets or sets current <see cref="DateTime"/> of the game's world.
        /// </summary>
        public static DateTime CurrentTime
        {
            get => GetWorldTime();

            set => SetWorldTime(value);
        }

        /// <summary>
        /// Gets the <see cref="Ped"/> of the current <see cref="GTA.Player"/>.
        /// </summary>
        public static Ped PlayerPed => Game.Player.Character;

        /// <summary>
        /// Gets the current driven <see cref="Vehicle"/> by <see cref="GTA.Player"/>.
        /// </summary>
        public static Vehicle PlayerVehicle => PlayerPed.CurrentVehicle;

        public static List<Vehicle> AllVehicles { get; internal set; }

        public static int NumDlcVehicles { get; } = Function.Call<int>(Hash.GET_NUM_DLC_VEHICLES);

        public static List<VehicleModelInfo> AllVehiclesModels { get; internal set; } = new List<VehicleModelInfo>();

        /// <summary>
        /// Toggles visibility state of game's GUI.
        /// </summary>
        public static bool HideGUI { get; set; } = false;

        public static string HelpText { get; set; } = null;
        public static string NotificationText { get; set; } = null;
        public static string SubtitleText { get; set; } = null;

        private static bool randomTrains = true;

        /// <summary>
        /// Toggles random generated trains on map.
        /// </summary>
        public static bool RandomTrains
        {
            get => randomTrains;

            set
            {
                Function.Call(Hash.SET_RANDOM_TRAINS, value);

                if (!value)
                {
                    Function.Call(Hash.DELETE_ALL_TRAINS);
                }

                randomTrains = value;
            }
        }

        public static Hash GetDlcVehicleModel(int dlcIndex)
        {
            return Function.Call<Hash>(Hash.GET_DLC_VEHICLE_MODEL, dlcIndex);
        }

        /// <summary>
        /// Requests the given <paramref name="model"/> in game.
        /// </summary>
        /// <param name="model">Instance of a <see cref="Model"/>.</param>
        /// <param name="name">Name of the <paramref name="model"/>.</param>
        /// <returns></returns>
        public static Model LoadAndRequestModel(Model model, string name = default)
        {
            if (!model.IsInCdImage || !model.IsValid)
            {
                if (name == default)
                {
                    throw new Exception(model + " not present!");
                }
                else
                {
                    throw new Exception(name + " not present!");
                }
            }

            model.Request();

            while (!model.IsLoaded)
            {
                Script.Yield();
            }

            return model;
        }

        /// <summary>
        /// Given a <paramref name="position"/> returns the nearest roadside point.
        /// </summary>
        /// <param name="position">Instance of a <see cref="Vector3"/>.</param>
        /// <returns>Nearest roadside point</returns>
        public static Vector3 GetPointOnRoadSide(Vector3 position)
        {
            OutputArgument ret = new OutputArgument();

            Function.Call((Hash)0x16F46FB18C8009E4, position.X, position.Y, position.Z, -1, ret);

            return ret.GetResult<Vector3>();
        }

        /// <summary>
        /// Cleares game's world from every ped and vehicles. Except entities with <see cref="Decorator.DoNotDelete"/> == <see langword="true"/>.
        /// </summary>
        public static void ClearWorld()
        {
            Function.Call(Hash.DELETE_ALL_TRAINS);

            Vehicle[] allVehicles = World.GetAllVehicles();

            allVehicles.Where(x => x.NotNullAndExists() && !x.Decorator().DoNotDelete).ToList()
                .ForEach(x => x?.DeleteCompletely());

            Ped[] allPeds = World.GetAllPeds();

            allPeds.Where(x => x.NotNullAndExists() && x != PlayerPed && (!PlayerVehicle.NotNullAndExists() || !PlayerVehicle.Passengers.Contains(x))).ToList()
                .ForEach(x => x?.Delete());
        }

        /// <summary>
        /// Given two <see cref="Vector3"/> points, returns the rotation between them.
        /// </summary>
        /// <param name="src">First <see cref="Vector3"/> point.</param>
        /// <param name="dst">Second <see cref="Vector3"/> point.</param>
        /// <param name="roll">Desired roll for output rotation.</param>
        /// <returns>Rotation between the two points</returns>
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

        /// <summary>
        /// Checks if pad is shaking.
        /// </summary>
        public static bool IsPadShaking => _padShakeStop >= Game.GameTime;

        /// <summary>
        /// Sets pad shake <paramref name="duration"/> and <paramref name="frequency"/>.
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="frequency"></param>
        public static void SetPadShake(int duration, int frequency)
        {
            _padShakeStop = Game.GameTime + duration;

            Function.Call(Hash.SET_PAD_SHAKE, 0, duration, frequency);
        }

        /// <summary>
        /// Instantly stops pad shake.
        /// </summary>
        public static void StopPadShake()
        {
            _padShakeStop = 0;

            Function.Call(Hash.STOP_PAD_SHAKE);
        }

        /// <summary>
        /// Gets the current game's world <see cref="DateTime"/>.
        /// </summary>
        /// <returns></returns>
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
                Function.Call(Hash.SET_CLOCK_DATE, 1985, 8, 21);
                Function.Call(Hash.SET_CLOCK_TIME, 8, 0, 0);

                return new DateTime(1985, 9, 21, 8, 0, 0);
            }            
        }

        /// <summary>
        /// Sets the current game's world <see cref="DateTime"/>.
        /// </summary>
        /// <param name="time"></param>
        private static void SetWorldTime(DateTime time)
        {
            Function.Call(Hash.SET_CLOCK_DATE, time.Day, time.Month - 1, time.Year);
            Function.Call(Hash.SET_CLOCK_TIME, time.Hour, time.Minute, time.Second);
        }

        // https://code.google.com/archive/p/slimmath/
        public static bool Decompose(Matrix matrix, out Vector3 scale, out Quaternion rotation, out Vector3 translation)
        {
            const float ZeroTolerance = 1e-6f;

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

        /// <summary>
        /// Wraps <paramref name="x"/> between <paramref name="min"/> and <paramref name="max"/>.
        /// </summary>
        /// <param name="x">Original value</param>
        /// <param name="min">Min value</param>
        /// <param name="max">Max value</param>
        /// <returns>Wrapped value</returns>
        public static float Wrap(float x, float min, float max)
        {
            float delta = max - min;

            x = (x + max) % delta;

            if (x < 0)
            {
                x += delta;
            }

            return x + min;
        }

        /// <summary>
        /// Linear interpolation between <paramref name="a"/> and <paramref name="b"/> by <paramref name="f"/> value.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="f"></param>
        /// <returns>Interpolated value.</returns>
        public static float Lerp(float a, float b, float f)
        {
            return a + (b - a) * f;
        }

        /// <summary>
        /// Combination of <see cref="Lerp(float, float, float)"/> and <see cref="Wrap(float, float, float)"/>.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="f"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static float Lerp(float a, float b, float f, float min, float max)
        {
            return Wrap(Lerp(a, b, f), min, max);
        }

        /// <summary>
        /// Linear interpolation between <paramref name="a"/> and <paramref name="b"/> by <paramref name="f"/> value.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="f"></param>
        /// <returns>Interpolated value.</returns>
        public static int Lerp(int a, int b, float f)
        {
            return (int)(a + (b - (float)a) * f);
        }

        /// <summary>
        /// Linear interpolation between <paramref name="a"/> and <paramref name="b"/> by <paramref name="f"/> value.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="f"></param>
        /// <returns>Interpolated value.</returns>
        public static Vector3 Lerp(Vector3 a, Vector3 b, float f)
        {
            return new Vector3() { X = Lerp(a.X, b.X, f), Y = Lerp(a.Y, b.Y, f), Z = Lerp(a.Z, b.Z, f) };
        }

        /// <summary>
        /// Combination of <see cref="Lerp(float, float, float)"/> and <see cref="Wrap(float, float, float)"/>.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="f"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static Vector3 Lerp(Vector3 a, Vector3 b, float f, float min, float max)
        {
            return new Vector3() { X = Lerp(a.X, b.X, f, min, max), Y = Lerp(a.Y, b.Y, f, min, max), Z = Lerp(a.Z, b.Z, f, min, max) };
        }

        /// <summary>
        /// Returns the unit vector of the specified <paramref name="coordinate"/>.
        /// </summary>
        /// <param name="coordinate">Desired <see cref="Coordinate"/> of unit vector.</param>
        /// <returns>Unit vector.</returns>
        public static Vector3 GetUnitVector(Coordinate coordinate)
        {
            switch (coordinate)
            {
                case Coordinate.X:
                    return Vector3.UnitX;
                case Coordinate.Y:
                    return Vector3.UnitY;
                default:
                    return Vector3.UnitZ;
            }
        }

        /// <summary>
        /// List of wheels bones names.
        /// </summary>
        public static readonly string[] WheelsBonesNames = new string[10]
        {
            "wheel_lf",
            "wheel_rf",
            "wheel_lr",
            "wheel_rr",
            "wheel_lm1",
            "wheel_rm1",
            "wheel_lm2",
            "wheel_rm2",
            "wheel_lm3",
            "wheel_rm3"
        };

        /// <summary>
        /// Returns the <see cref="VehicleWheelBoneId"/> of the given wheel's <paramref name="name"/>.
        /// </summary>
        /// <param name="name">Wheel's name.</param>
        /// <returns><see cref="VehicleWheelBoneId"/> of the wheel.</returns>
        public static VehicleWheelBoneId ConvertWheelNameToID(string name)
        {
            switch (name)
            {
                case "wheel_lf":
                    return VehicleWheelBoneId.WheelLeftFront;
                case "wheel_lr":
                    return VehicleWheelBoneId.WheelLeftRear;
                case "wheel_rf":
                    return VehicleWheelBoneId.WheelRightFront;
                case "wheel_rr":
                    return VehicleWheelBoneId.WheelRightRear;
                case "wheel_lm1":
                    return VehicleWheelBoneId.WheelLeftMiddle1;
                case "wheel_rm1":
                    return VehicleWheelBoneId.WheelRightMiddle1;
                case "wheel_lm2":
                    return VehicleWheelBoneId.WheelLeftMiddle2;
                case "wheel_rm2":
                    return VehicleWheelBoneId.WheelRightMiddle2;
                case "wheel_lm3":
                    return VehicleWheelBoneId.WheelLeftMiddle3;
                case "wheel_rm3":
                    return VehicleWheelBoneId.WheelRightMiddle3;
                default:
                    return VehicleWheelBoneId.Invalid;
            }
        }

        public static string ConvertWheelIDToName(VehicleWheelBoneId id)
        {
            switch (id)
            {
                case VehicleWheelBoneId.WheelLeftFront:
                    return "wheel_lf";
                case VehicleWheelBoneId.WheelLeftRear:
                    return "wheel_lr";
                case VehicleWheelBoneId.WheelRightFront:
                    return "wheel_rf";
                case VehicleWheelBoneId.WheelRightRear:
                    return "wheel_rr";
                case VehicleWheelBoneId.WheelLeftMiddle1:
                    return "wheel_lm1";
                case VehicleWheelBoneId.WheelRightMiddle1:
                    return "wheel_rm1";
                case VehicleWheelBoneId.WheelLeftMiddle2:
                    return "wheel_lm2";
                case VehicleWheelBoneId.WheelRightMiddle2:
                    return "wheel_rm2";
                case VehicleWheelBoneId.WheelLeftMiddle3:
                    return "wheel_lm3";
                case VehicleWheelBoneId.WheelRightMiddle3:
                    return "wheel_rm3";
                default:
                    return "";
            }
        }

        /// <summary>
        /// Spawns a train using <paramref name="variation"/> config and <paramref name="direction"/> at <paramref name="position"/>.
        /// </summary>
        /// <param name="variation">ID of the train configuration. Check trains.xml for available ones.</param>
        /// <param name="position">Position of the train. Actually it will be used the nearest track to this position.</param>
        /// <param name="direction">Direction of the train.</param>
        /// <returns><see cref="Vehicle"/> instance of the head of the train.</returns>
        public static Vehicle CreateMissionTrain(int variation, Vector3 position, bool direction)
        {
            return Function.Call<Vehicle>(Hash.CREATE_MISSION_TRAIN, variation, position.X, position.Y, position.Z, direction);
        }

        /// <summary>
        /// Parses a <paramref name="raw"/> string trying to retrieve a correct <see cref="DateTime"/> representation.
        /// </summary>
        /// <param name="raw">Raw string</param>
        /// <param name="currentTime">Original <see cref="DateTime"/>.</param>
        /// <param name="inputType">Returns the <see cref="InputType"/>.</param>
        /// <returns><see cref="DateTime"/> value; otherwise <c>null</c>.</returns>
        public static DateTime? ParseFromRawString(string raw, DateTime currentTime, out InputType inputType)
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

                    inputType = InputType.Full;

                    return new DateTime(int.Parse(year), int.Parse(month), int.Parse(day), int.Parse(hour), int.Parse(minute), 0);
                }
                else if (raw.Length == 8)
                {
                    string month = raw.Substring(0, 2);
                    string day = raw.Substring(2, 2);
                    string year = raw.Substring(4, 4);

                    inputType = InputType.Date;

                    return new DateTime(int.Parse(year), int.Parse(month), int.Parse(day), currentTime.Hour, currentTime.Minute, 0);
                }
                else if (raw.Length == 4)
                {
                    string hour = raw.Substring(0, 2);
                    string minute = raw.Substring(2, 2);

                    inputType = InputType.Time;

                    return new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, int.Parse(hour), int.Parse(minute), 0);
                }

                inputType = InputType.Error;

                return null;
            }
            catch (Exception)
            {
                inputType = InputType.Error;
                return null;
            }
        }

        /// <summary>
        /// Returns the 2D squared distance between <paramref name="entity1"/> and <paramref name="entity2"/>.
        /// </summary>
        /// <param name="entity1">Instance of an entity.</param>
        /// <param name="entity2">Instance of an entity.</param>
        /// <returns>Distance in <c>float</c> between the entities.</returns>
        public static float DistanceToSquared2D(Entity entity1, Entity entity2)
        {
            return entity1.Position.DistanceToSquared2D(entity2.Position);
        }

        /// <summary>
        /// Draws a line.
        /// </summary>
        /// <param name="from">First point.</param>
        /// <param name="to">Second point.</param>
        /// <param name="color">Color of the line.</param>
        public static void DrawLine(Vector3 from, Vector3 to, Color color)
        {
            Function.Call(Hash.DRAW_LINE, from.X, from.Y, from.Z, to.X, to.Y, to.Z, color.R, color.G, color.B, color.A);
        }

        /// <summary>
        /// Gets the position on ground of the given <paramref name="position"/> with <paramref name="verticalOffset"/>.
        /// </summary>
        /// <param name="position">Point in the world.</param>
        /// <param name="verticalOffset">Z offset.</param>
        /// <returns>Point on ground.</returns>
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

        /// <summary>
        /// Gets the position of the waypoint, if any.
        /// </summary>
        /// <returns><see cref="Vector3"/> position of the waypoint. Returns <see cref="Vector3.Zero"/> if waypoint is not present.</returns>
        public static Vector3 GetWaypointPosition()
        {
            if (!Game.IsWaypointActive)
            {
                return Vector3.Zero;
            }

            Vector3 position = World.WaypointPosition;

            do
            {
                position.RequestCollision();
                Script.Yield();
                position.Z = World.GetGroundHeight(new Vector2(position.X, position.Y));
            } while (position.Z == 0);

            return position;
        }

        /// <summary>
        /// Checks if current camera is in first person view.
        /// </summary>
        /// <returns><see langword="true"/> if FPV is enabled; otherwise <see langword="false"/>.</returns>
        public static bool IsCameraInFirstPerson()
        {
            return Function.Call<int>(Hash.GET_FOLLOW_PED_CAM_VIEW_MODE) == 4 && !GameplayCamera.IsLookingBehind && !Function.Call<bool>((Hash)0xF5F1E89A970B7796);
        }

        public static float RainLevel
        {
            get => Function.Call<float>((Hash)0x96695E368AD855F3);

            set => Function.Call((Hash)0x643E26EA6E024D92, value);
        }

        public static float WindSpeed
        {
            get => Function.Call<float>((Hash)0xA8CF1CC0AFCD3F12);

            set => Function.Call((Hash)0xEE09ECEDBABE47FC, value);
        }

        public static float Magnitude(Vector3 vector3)
        {
            return Function.Call<float>(Hash.VMAG2, vector3.X, vector3.Z, vector3.Y);
        }

        /// <summary>
        /// Checks if wheel at <paramref name="pos"/> of <paramref name="vehicle"/> is on rail tracks.
        /// </summary>
        /// <param name="pos"><see cref="Vector3"/> of the wheel.</param>
        /// <param name="vehicle">Instance of a <see cref="Vector3"/>.</param>
        /// <returns><see langword="true"/> wheel is on rail tracks; otherwise <see langword="false"/>.</returns>
        internal static bool IsWheelOnTracks(Vector3 pos, Vehicle vehicle)
        {
            RaycastResult ret = World.Raycast(pos, pos.GetSingleOffset(Coordinate.Z, -1), IntersectFlags.Map, vehicle);

            // Tracks materials
            List<MaterialHash> allowedSurfaces = new List<MaterialHash>
                {
                    MaterialHash.MetalSolidRoadSurface,
                    MaterialHash.MetalSolidSmall,
                    MaterialHash.MetalSolidMedium,
                    MaterialHash.MetalSolidLarge,
                    MaterialHash.GravelTrainTrack
                };

            return allowedSurfaces.Contains(ret.MaterialHash);
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
