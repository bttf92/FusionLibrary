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
        public float Armor { get; }
        public int[,] Components { get; private set; } = new int[12, 3];
        public int[,] Props { get; private set; } = new int[5, 2];
        public int Money { get; }
        public List<WeaponReplica> Weapons { get; }

        private static readonly Array WeaponsHash = Enum.GetValues(typeof(WeaponHash));

        public PedReplica(Ped ped) : base(ped)
        {
            Model = ped.Model;
            Type = Function.Call<int>(Hash.GET_PED_TYPE, ped);

            Seat = ped.SeatIndex;

            Armor = ped.ArmorFloat;

            for (int x = 0; x <= 11; x++)
            {
                Components[x, 0] = Function.Call<int>(Hash.GET_PED_DRAWABLE_VARIATION, ped, x);
                Components[x, 1] = Function.Call<int>(Hash.GET_PED_TEXTURE_VARIATION, ped, x);
                Components[x, 2] = Function.Call<int>(Hash.GET_PED_PALETTE_VARIATION, ped, x);
            }

            for (int x = 0; x <= 4; x++)
            {
                if (x <= 2)
                {
                    Props[x,0] = Function.Call<int>(Hash.GET_PED_PROP_INDEX, ped, x);
                    Props[x,1] = Function.Call<int>(Hash.GET_PED_PROP_TEXTURE_INDEX, ped, x);
                }
                else
                {
                    Props[x, 0] = Function.Call<int>(Hash.GET_PED_PROP_INDEX, ped, x + 3);
                    Props[x, 1] = Function.Call<int>(Hash.GET_PED_PROP_TEXTURE_INDEX, ped, x + 3);
                }
            }

            Money = ped.Money;

            Weapons = new List<WeaponReplica>();

            foreach (WeaponHash x in WeaponsHash)
            {
                if (ped.Weapons.HasWeapon(x))
                {
                    Weapons.Add(new WeaponReplica(ped, ped.Weapons[x]));
                }
            }
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
            ped.ArmorFloat = Armor;
            ped.Money = Money;
        }

        public Ped Spawn()
        {
            Ped ped = Function.Call<Ped>(Hash.CREATE_PED, Type, Model, Position.X, Position.Y, Position.Z, Heading, false, false);

            ped.Rotation = Rotation;

            CommonSpawn(ped);

            return ped;
        }

        public Ped Spawn(Vector3 position, float heading)
        {
            Ped ped = Function.Call<Ped>(Hash.CREATE_PED, Type, Model, position.X, position.Y, position.Z, heading, false, false);

            CommonSpawn(ped);

            return ped;
        }

        public Ped Spawn(Vehicle vehicle)
        {
            VehicleSeat seat = Seat;

            if (!vehicle.IsSeatFree(seat))
            {
                return null;
            }

            if (seat == VehicleSeat.None)
            {
                seat = VehicleSeat.Any;
            }

            Ped ped = Function.Call<Ped>(Hash.CREATE_PED_INSIDE_VEHICLE, vehicle, Type, Model, seat, false, false);

            CommonSpawn(ped);

            return ped;
        }

        public Ped Spawn(Vehicle vehicle, VehicleSeat vehicleSeat)
        {
            if (!vehicle.IsSeatFree(vehicleSeat))
            {
                return null;
            }

            if (vehicleSeat == VehicleSeat.None)
            {
                vehicleSeat = VehicleSeat.Any;
            }

            Ped ped = Function.Call<Ped>(Hash.CREATE_PED_INSIDE_VEHICLE, vehicle, Type, Model, vehicleSeat, false, false);

            CommonSpawn(ped);

            return ped;
        }

        private void CommonSpawn(Ped ped)
        {
            ApplyTo(ped);

            foreach (WeaponReplica x in Weapons)
            {
                x.Give(ped);
            }

            for (int x = 0; x <= 11; x++)
            {
                Function.Call(Hash.SET_PED_COMPONENT_VARIATION, ped, x, Components[x, 0], Components[x, 1], Components[x, 2]);
            }

            for (int x = 0; x <= 4; x++)
            {
                if (x <= 2)
                {
                    Function.Call(Hash.SET_PED_PROP_INDEX, ped, x, Props[x, 0], Props[x, 1], true);
                }
                else
                {
                    Function.Call(Hash.SET_PED_PROP_INDEX, ped, x + 3, Props[x, 0], Props[x, 1], true);
                }
            }
        }
    }
}
