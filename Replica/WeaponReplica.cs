using GTA;
using System;

namespace FusionLibrary
{
    [Serializable]
    public class WeaponReplica
    {
        public WeaponReplica(Ped ped, Weapon weapon)
        {
            WeaponHash = weapon.Hash;
            Ammo = weapon.Ammo;
            IsEquipped = ped.Weapons.Current == weapon;
            IsAmmoLoaded = weapon.AmmoInClip > 0;
        }

        public WeaponHash WeaponHash { get; }
        public int Ammo { get; }
        public bool IsEquipped { get; }
        public bool IsAmmoLoaded { get; }

        public void Give(Ped ped)
        {
            ped.Weapons.Give(WeaponHash, Ammo, IsEquipped, IsAmmoLoaded);
        }
    }
}
