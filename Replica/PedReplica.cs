using GTA;
using GTA.Math;
using GTA.Native;
using System;
using System.Collections.Generic;

namespace FusionLibrary
{
    [Serializable]
    public class PedReplica : EntityReplica
    {
        public int Type { get; }
        public VehicleSeat Seat { get; }
        public List<WeaponReplica> Weapons { get; }

        private static Array WeaponsHash = Enum.GetValues(typeof(WeaponHash));

        public PedReplica(Ped ped) : base(ped)
        {
            Model = ped.Model;
            Type = Function.Call<int>(Hash.GET_PED_TYPE, ped);

            Seat = ped.SeatIndex;

            Weapons = new List<WeaponReplica>();

            foreach (WeaponHash x in WeaponsHash)
                if (ped.Weapons.HasWeapon(x))
                    Weapons.Add(new WeaponReplica(ped, ped.Weapons[x]));
        }

        public void ApplyTo(Ped ped)
        {
            ped.IsVisible = IsVisible;
            ped.PositionNoOffset = Position;
            ped.Heading = Heading;
            ped.Rotation = Rotation;
            ped.Speed = Speed;
            ped.Velocity = Velocity;
            ped.HealthFloat = Health;
        }

        public Ped Spawn()
        {
            Ped ped = Function.Call<Ped>(Hash.CREATE_PED, Type, Model, Position.X, Position.Y, Position.Z, Heading, false, false);

            ped.Rotation = Rotation;

            foreach (WeaponReplica x in Weapons)
                x.Give(ped);

            return ped;
        }

        public Ped Spawn(Vector3 position, float heading)
        {
            Ped ped = Function.Call<Ped>(Hash.CREATE_PED, Type, Model, position.X, position.Y, position.Z, heading, false, false);

            foreach (WeaponReplica x in Weapons)
                x.Give(ped);

            return ped;
        }

        public Ped Spawn(Vehicle vehicle)
        {
            VehicleSeat seat = Seat;

            if (!vehicle.IsSeatFree(seat))
                return null;

            if (seat == VehicleSeat.None)
                seat = VehicleSeat.Any;

            Ped ped = Function.Call<Ped>(Hash.CREATE_PED_INSIDE_VEHICLE, vehicle, Type, Model, seat, false, false);

            foreach (WeaponReplica x in Weapons)
                x.Give(ped);

            return ped;
        }

        public Ped Spawn(Vehicle vehicle, VehicleSeat vehicleSeat)
        {
            if (!vehicle.IsSeatFree(vehicleSeat))
                return null;

            if (vehicleSeat == VehicleSeat.None)
                vehicleSeat = VehicleSeat.Any;

            Ped ped = Function.Call<Ped>(Hash.CREATE_PED_INSIDE_VEHICLE, vehicle, Type, Model, vehicleSeat, false, false);

            foreach (WeaponReplica x in Weapons)
                x.Give(ped);

            return ped;
        }
    }
}
