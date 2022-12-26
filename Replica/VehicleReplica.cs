using FusionLibrary.Extensions;
using FusionLibrary.Memory;
using GTA;
using GTA.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using static FusionLibrary.FusionEnums;

namespace FusionLibrary
{
    [Serializable]
    public class VehicleReplica : EntityReplica
    {
        public float EngineHealth { get; }
        public bool EngineRunning { get; }
        public VehicleColor PrimaryColor { get; }
        public VehicleColor SecondaryColor { get; }
        public VehicleColor InteriorColor { get; }
        public VehicleColor DashboardColor { get; }
        public VehicleColor WheelColor { get; }
        public VehicleWheelType WheelType { get; }
        public LicensePlateStyle PlateStyle { get; }
        public string PlateNumber { get; }
        public VehicleWindowTint WindowTint { get; }
        public int Livery { get; }
        public List<bool> Extras { get; }
        public Dictionary<VehicleModType, int> Mods { get; }
        public List<PedReplica> Occupants { get; }

        public float Handbrake { get; }
        public float RPM { get; }
        public int Gear { get; }
        public float Throttle { get; }
        public float Brake { get; }
        public float SteeringAngle { get; }
        public bool Lights { get; }
        public bool Headlights { get; }
        public RunningDirection RunningDirection { get; }

        public float[] WheelsRotations { get; }
        public float[] WheelsCompressions { get; }

        public VehicleReplica(Vehicle vehicle, SpawnFlags spawnFlags = SpawnFlags.Default) : base(vehicle)
        {
            EngineHealth = vehicle.EngineHealth;
            EngineRunning = vehicle.IsEngineRunning;
            WheelType = vehicle.Mods.WheelType;
            PrimaryColor = vehicle.Mods.PrimaryColor;
            SecondaryColor = vehicle.Mods.SecondaryColor;
            InteriorColor = vehicle.Mods.TrimColor;
            DashboardColor = vehicle.Mods.DashboardColor;
            WheelColor = vehicle.Mods.RimColor;
            PlateStyle = vehicle.Mods.LicensePlateStyle;
            PlateNumber = vehicle.Mods.LicensePlate;
            WindowTint = vehicle.Mods.WindowTint;
            Livery = vehicle.Mods.Livery;

            if (!spawnFlags.HasFlag(SpawnFlags.NoMods))
            {
                Extras = new List<bool>();

                for (int x = 1; x <= 13; x++)
                {
                    Extras.Add(vehicle.IsExtraOn(x));
                }

                Mods = new Dictionary<VehicleModType, int>();

                foreach (VehicleModType x in (VehicleModType[])Enum.GetValues(typeof(VehicleModType)))
                {
                    Mods.Add(x, vehicle.Mods[x].Index);
                }
            }

            RPM = vehicle.CurrentRPM;
            Gear = vehicle.CurrentGear;

            Throttle = vehicle.ThrottlePower;
            Brake = vehicle.BrakePower;
            Handbrake = VehicleControl.GetHandbrake(vehicle);

            SteeringAngle = vehicle.SteeringAngle;
            Lights = vehicle.AreLightsOn;
            Headlights = vehicle.AreHighBeamsOn;

            RunningDirection = vehicle.RunningDirection();

            WheelsRotations = VehicleControl.GetWheelRotations(vehicle);
            WheelsCompressions = VehicleControl.GetWheelCompressions(vehicle);

            if (spawnFlags.HasFlag(SpawnFlags.NoOccupants))
            {
                return;
            }

            Occupants = new List<PedReplica>();

            foreach (Ped x in vehicle.Occupants)
            {
                if (x.SeatIndex == VehicleSeat.Driver && spawnFlags.HasFlag(SpawnFlags.NoDriver))
                    continue;

                Occupants.Add(new PedReplica(x));
            }
        }

        public Vehicle Spawn(SpawnFlags spawnFlags, Vector3 position = default, float heading = default)
        {
            Vehicle veh = null;

            if (spawnFlags.HasFlag(SpawnFlags.CheckExists))
            {
                veh = World.GetClosestVehicle(Position, 1.0f, Model);
            }

            if (spawnFlags.HasFlag(SpawnFlags.NoPosition))
            {
                if (veh == null)
                {
                    veh = World.CreateVehicle(Model, position, heading);
                }
                else
                {
                    veh.Position = position;
                    veh.Heading = heading;
                }
            }
            else
            {
                if (veh == null)
                {
                    veh = World.CreateVehicle(Model, Position, Heading);
                }
                else
                {
                    veh.Position = Position;
                    veh.Heading = Heading;
                }
            }

            ApplyTo(veh, spawnFlags);

            return veh;
        }

        private void ApplyToPrivate(Vehicle vehicle, SpawnFlags spawnFlags)
        {
            vehicle.ThrottlePower = Throttle;
            vehicle.BrakePower = Brake;
            VehicleControl.SetHandbrake(vehicle, Handbrake);
            vehicle.SteeringAngle = SteeringAngle;
            vehicle.AreLightsOn = Lights;
            vehicle.AreHighBeamsOn = Headlights;

            vehicle.HealthFloat = Health;
            vehicle.EngineHealth = EngineHealth;
            vehicle.Mods.WheelType = WheelType;
            vehicle.Mods.PrimaryColor = PrimaryColor;
            vehicle.Mods.SecondaryColor = SecondaryColor;
            vehicle.Mods.TrimColor = InteriorColor;
            vehicle.Mods.DashboardColor = DashboardColor;
            vehicle.Mods.RimColor = WheelColor;
            vehicle.Mods.LicensePlateStyle = PlateStyle;
            vehicle.Mods.LicensePlate = PlateNumber;
            vehicle.Mods.WindowTint = WindowTint;
            vehicle.Mods.Livery = Livery;

            if (!spawnFlags.HasFlag(SpawnFlags.NoMods))
            {
                if (Extras != null)
                {
                    for (int i = 0; i < Extras.Count; i++)
                    {
                        vehicle.ToggleExtra(i + 1, Extras[i]);
                    }
                }

                if (Mods != null)
                {
                    for (int i = 0; i < Mods.Count; i++)
                    {
                        var x = Mods.ElementAt(i);

                        vehicle.Mods[x.Key].Index = x.Value;
                    }
                }
            }

            vehicle.IsEngineRunning = EngineRunning;

            vehicle.CurrentRPM = RPM;
            vehicle.CurrentGear = Gear;

            if (spawnFlags.HasFlag(SpawnFlags.NoOccupants))
            {
                return;
            }

            foreach (PedReplica pedReplica in Occupants)
            {
                if (pedReplica.Seat == VehicleSeat.Driver && spawnFlags.HasFlag(SpawnFlags.NoDriver))
                    continue;

                pedReplica.Spawn(vehicle);
            }
        }

        public void ApplyTo(Vehicle vehicle, SpawnFlags spawnFlags = SpawnFlags.Default)
        {
            ApplyToPrivate(vehicle, spawnFlags);

            if (!spawnFlags.HasFlag(SpawnFlags.NoPosition))
            {
                vehicle.PositionNoOffset = Position;
                vehicle.Heading = Heading;
                vehicle.Rotation = Rotation;
            }

            if (spawnFlags.HasFlag(SpawnFlags.SetRotation))
            {
                vehicle.Rotation = Rotation;
            }

            if (!spawnFlags.HasFlag(SpawnFlags.NoVelocity))
            {
                vehicle.Velocity = Velocity;
                vehicle.Speed = Speed;
            }

            if (spawnFlags.HasFlag(SpawnFlags.NoWheels))
            {
                return;
            }

            for (int i = 0; i < WheelsRotations.Length; i++)
            {
                VehicleControl.SetWheelRotation(vehicle, i, FusionUtils.Wrap(WheelsRotations[i], -(float)Math.PI, (float)Math.PI));
                VehicleControl.SetWheelCompression(vehicle, i, WheelsCompressions[i]);
            }
        }

        public void ApplyTo(Vehicle vehicle, SpawnFlags spawnFlags = SpawnFlags.Default, VehicleReplica nextReplica = default, float adjustedRatio = 0)
        {
            ApplyToPrivate(vehicle, spawnFlags);

            if (nextReplica == default || nextReplica == null)
            {
                nextReplica = this;
            }

            if (!spawnFlags.HasFlag(SpawnFlags.NoPosition))
            {
                vehicle.PositionNoOffset = FusionUtils.Lerp(Position, nextReplica.Position, adjustedRatio);
                vehicle.Heading = FusionUtils.Lerp(Heading, nextReplica.Heading, adjustedRatio);
                vehicle.Rotation = FusionUtils.Lerp(Rotation, nextReplica.Rotation, adjustedRatio, -180, 180);
            }

            if (spawnFlags.HasFlag(SpawnFlags.SetRotation))
            {
                vehicle.Rotation = FusionUtils.Lerp(Rotation, nextReplica.Rotation, adjustedRatio, -180, 180);
            }

            if (!spawnFlags.HasFlag(SpawnFlags.NoVelocity))
            {
                vehicle.Velocity = FusionUtils.Lerp(Velocity, nextReplica.Velocity, adjustedRatio);
                vehicle.Speed = FusionUtils.Lerp(Speed, nextReplica.Speed, adjustedRatio);
            }

            if (spawnFlags.HasFlag(SpawnFlags.NoWheels))
            {
                return;
            }

            for (int i = 0; i < WheelsRotations.Length; i++)
            {
                VehicleControl.SetWheelRotation(vehicle, i, FusionUtils.Lerp(WheelsRotations[i], nextReplica.WheelsRotations[i], adjustedRatio));
                VehicleControl.SetWheelCompression(vehicle, i, FusionUtils.Lerp(WheelsCompressions[i], nextReplica.WheelsCompressions[i], adjustedRatio));
            }
        }
    }
}
