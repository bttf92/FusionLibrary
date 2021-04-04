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
        public int Model { get; set; }
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

        public float Throttle { get; }
        public float Brake { get; }
        public float SteeringAngle { get; }
        public bool Lights { get; }
        public bool Headlights { get; }

        public float[] WheelsRotations { get; }
        public float[] WheelsCompressions { get; }

        public VehicleReplica(Vehicle veh, SpawnFlags spawnFlags = SpawnFlags.Default)
        {
            Model = veh.Model;
            Velocity = veh.Velocity;
            Position = veh.Position;
            Rotation = veh.Rotation;
            Heading = veh.Heading;
            Speed = veh.Speed;
            Health = veh.HealthFloat;
            EngineHealth = veh.EngineHealth;
            EngineRunning = veh.IsEngineRunning;
            PrimaryColor = veh.Mods.PrimaryColor;
            SecondaryColor = veh.Mods.SecondaryColor;
            Livery = veh.Mods.Livery;

            Throttle = veh.ThrottlePower;
            Brake = veh.BrakePower;

            SteeringAngle = veh.SteeringAngle;
            Lights = veh.AreLightsOn;
            Headlights = veh.AreHighBeamsOn;

            WheelsRotations = VehicleControl.GetWheelRotations(veh);
            WheelsCompressions = VehicleControl.GetWheelCompressions(veh);

            if (spawnFlags.HasFlag(SpawnFlags.NoOccupants))
                return;

            Occupants = new List<PedReplica>();

            foreach (Ped x in veh.Occupants)
                Occupants.Add(new PedReplica(x));
        }

        public Vehicle Spawn(SpawnFlags spawnFlags, Vector3 position = default, float heading = default)
        {
            Vehicle veh = null;

            if (spawnFlags.HasFlag(SpawnFlags.CheckExists))
                veh = World.GetClosestVehicle(Position, 1.0f, Model);

            if (spawnFlags.HasFlag(SpawnFlags.ForcePosition))
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

        public void ApplyTo(Vehicle veh, SpawnFlags spawnFlags = SpawnFlags.Default, float timeRatio = 0, VehicleReplica nextReplica = default)
        {
            if (nextReplica == default)
                nextReplica = this;

            veh.ThrottlePower = Throttle;
            veh.BrakePower = Brake;
            veh.SteeringAngle = SteeringAngle;
            veh.AreLightsOn = Lights;
            veh.AreHighBeamsOn = Headlights;

            veh.HealthFloat = Health;
            veh.EngineHealth = EngineHealth;
            veh.Mods.PrimaryColor = PrimaryColor;
            veh.Mods.SecondaryColor = SecondaryColor;
            veh.Mods.Livery = Livery;

            veh.IsEngineRunning = EngineRunning;

            if (!spawnFlags.HasFlag(SpawnFlags.ForcePosition))
            {
                veh.PositionNoOffset = Utils.Lerp(Position, nextReplica.Position, timeRatio);
                veh.Heading = Utils.Lerp(Heading, nextReplica.Heading, timeRatio);
                veh.Rotation = Utils.Lerp(Rotation, nextReplica.Rotation, timeRatio, -180, 180);
            }

            if (!spawnFlags.HasFlag(SpawnFlags.NoVelocity))
            {
                veh.Velocity = Utils.Lerp(Velocity, nextReplica.Velocity, timeRatio);
                veh.Speed = Utils.Lerp(Speed, nextReplica.Speed, timeRatio);
            }

            for (int i = 0; i < WheelsRotations.Length; i++)
            {
                VehicleControl.SetWheelRotation(veh, i, Utils.Lerp(WheelsRotations[i], nextReplica.WheelsRotations[i], timeRatio, -(float)Math.PI, (float)Math.PI));
                VehicleControl.SetWheelCompression(veh, i, Utils.Lerp(WheelsCompressions[i], nextReplica.WheelsCompressions[i], timeRatio));
            }

            if (!spawnFlags.HasFlag(SpawnFlags.NoOccupants))
                foreach (PedReplica pedReplica in Occupants)
                    pedReplica.Spawn(veh);
        }
    }
}
