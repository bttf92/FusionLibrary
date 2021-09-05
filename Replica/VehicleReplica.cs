using FusionLibrary.Extensions;
using FusionLibrary.Memory;
using GTA;
using GTA.Math;
using System;
using System.Collections.Generic;
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
        public int Livery { get; }
        public List<PedReplica> Occupants { get; }

        public float Handbrake { get; }
        public float RPM { get; }
        public int Gear { get; }
        public float Throttle { get; }
        public float Brake { get; }
        public float SteeringAngle { get; }
        public bool Lights { get; }
        public bool Headlights { get; }
        public bool IsGoingForward { get; }

        public float[] WheelsRotations { get; }
        public float[] WheelsCompressions { get; }

        public VehicleReplica(Vehicle vehicle, SpawnFlags spawnFlags = SpawnFlags.Default) : base(vehicle)
        {
            EngineHealth = vehicle.EngineHealth;
            EngineRunning = vehicle.IsEngineRunning;
            PrimaryColor = vehicle.Mods.PrimaryColor;
            SecondaryColor = vehicle.Mods.SecondaryColor;
            Livery = vehicle.Mods.Livery;

            RPM = vehicle.CurrentRPM;
            Gear = vehicle.CurrentGear;

            Throttle = vehicle.ThrottlePower;
            Brake = vehicle.BrakePower;
            Handbrake = VehicleControl.GetHandbrake(vehicle);

            SteeringAngle = vehicle.SteeringAngle;
            Lights = vehicle.AreLightsOn;
            Headlights = vehicle.AreHighBeamsOn;

            IsGoingForward = vehicle.IsGoingForward();

            WheelsRotations = VehicleControl.GetWheelRotations(vehicle);
            WheelsCompressions = VehicleControl.GetWheelCompressions(vehicle);

            if (spawnFlags.HasFlag(SpawnFlags.NoOccupants))
            {
                return;
            }

            Occupants = new List<PedReplica>();

            foreach (Ped x in vehicle.Occupants)
            {
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

        private void ApplyTo2(Vehicle vehicle, bool noOccupants)
        {
            vehicle.ThrottlePower = Throttle;
            vehicle.BrakePower = Brake;
            VehicleControl.SetHandbrake(vehicle, Handbrake);
            vehicle.SteeringAngle = SteeringAngle;
            vehicle.AreLightsOn = Lights;
            vehicle.AreHighBeamsOn = Headlights;

            vehicle.HealthFloat = Health;
            vehicle.EngineHealth = EngineHealth;
            vehicle.Mods.PrimaryColor = PrimaryColor;
            vehicle.Mods.SecondaryColor = SecondaryColor;
            vehicle.Mods.Livery = Livery;

            vehicle.IsEngineRunning = EngineRunning;

            vehicle.CurrentRPM = RPM;
            vehicle.CurrentGear = Gear;

            if (noOccupants)
            {
                return;
            }

            foreach (PedReplica pedReplica in Occupants)
            {
                pedReplica.Spawn(vehicle);
            }
        }

        public void ApplyTo(Vehicle vehicle, SpawnFlags spawnFlags = SpawnFlags.Default)
        {
            ApplyTo2(vehicle, spawnFlags.HasFlag(SpawnFlags.NoOccupants));

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
            ApplyTo2(vehicle, spawnFlags.HasFlag(SpawnFlags.NoOccupants));

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
