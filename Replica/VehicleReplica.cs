using FusionLibrary.Memory;
using GTA;
using GTA.Math;
using System;
using System.Collections.Generic;
using static FusionLibrary.Enums;

namespace FusionLibrary
{
    [Serializable]
    public class VehicleReplica
    {
        public int ModelHash { get; set; }
        public Model Model
        {
            get => new Model(ModelHash);
            set => ModelHash = value.Hash;
        }
        public Vector3 Velocity { get; }
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public float Heading { get; set; }
        public float Speed { get; }
        public float Health { get; }
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

        public float[] WheelsRotations { get; }
        public float[] WheelsCompressions { get; }

        public VehicleReplica(Vehicle vehicle, SpawnFlags spawnFlags = SpawnFlags.Default)
        {
            Model = vehicle.Model;
            Velocity = vehicle.Velocity;
            Position = vehicle.Position;
            Rotation = vehicle.Rotation;
            Heading = vehicle.Heading;
            Speed = vehicle.Speed;
            Health = vehicle.HealthFloat;
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

            WheelsRotations = VehicleControl.GetWheelRotations(vehicle);
            WheelsCompressions = VehicleControl.GetWheelCompressions(vehicle);

            if (spawnFlags.HasFlag(SpawnFlags.NoOccupants))
                return;

            Occupants = new List<PedReplica>();

            foreach (Ped x in vehicle.Occupants)
                Occupants.Add(new PedReplica(x));
        }

        public Vehicle Spawn(SpawnFlags spawnFlags, Vector3 position = default, float heading = default)
        {
            Vehicle veh = null;

            if (spawnFlags.HasFlag(SpawnFlags.CheckExists))
                veh = World.GetClosestVehicle(Position, 1.0f, Model);

            if (spawnFlags.HasFlag(SpawnFlags.NoPosition))
            {
                if (veh == null)
                    veh = World.CreateVehicle(Model, position, heading);
                else
                {
                    veh.Position = position;
                    veh.Heading = heading;
                }
            }
            else
            {
                if (veh == null)
                    veh = World.CreateVehicle(Model, Position, Heading);
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
                return;

            foreach (PedReplica pedReplica in Occupants)
                pedReplica.Spawn(vehicle);
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
                vehicle.Rotation = Rotation;

            if (!spawnFlags.HasFlag(SpawnFlags.NoVelocity))
            {
                vehicle.Velocity = Velocity;
                vehicle.Speed = Speed;
            }

            for (int i = 0; i < WheelsRotations.Length; i++)
            {
                VehicleControl.SetWheelRotation(vehicle, i, WheelsRotations[i]);
                VehicleControl.SetWheelCompression(vehicle, i, WheelsCompressions[i]);
            }
        }

        public void ApplyTo(Vehicle vehicle, SpawnFlags spawnFlags = SpawnFlags.Default, VehicleReplica nextReplica = default, float adjustedRatio = 0)
        {
            ApplyTo2(vehicle, spawnFlags.HasFlag(SpawnFlags.NoOccupants));

            if (nextReplica == default || nextReplica == null)
                nextReplica = this;

            if (!spawnFlags.HasFlag(SpawnFlags.NoPosition))
            {
                vehicle.PositionNoOffset = Utils.Lerp(Position, nextReplica.Position, adjustedRatio);
                vehicle.Heading = Utils.Lerp(Heading, nextReplica.Heading, adjustedRatio);
                vehicle.Rotation = Utils.Lerp(Rotation, nextReplica.Rotation, adjustedRatio, -180, 180);
            }

            if (spawnFlags.HasFlag(SpawnFlags.SetRotation))
                vehicle.Rotation = Utils.Lerp(Rotation, nextReplica.Rotation, adjustedRatio, -180, 180);

            if (!spawnFlags.HasFlag(SpawnFlags.NoVelocity))
            {
                vehicle.Velocity = Utils.Lerp(Velocity, nextReplica.Velocity, adjustedRatio);
                vehicle.Speed = Utils.Lerp(Speed, nextReplica.Speed, adjustedRatio);
            }

            for (int i = 0; i < WheelsRotations.Length; i++)
            {
                VehicleControl.SetWheelRotation(vehicle, i, Utils.Lerp(WheelsRotations[i], nextReplica.WheelsRotations[i], adjustedRatio));
                VehicleControl.SetWheelCompression(vehicle, i, Utils.Lerp(WheelsCompressions[i], nextReplica.WheelsCompressions[i], adjustedRatio));
            }
        }
    }
}
