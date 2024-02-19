using GTA;
using GTA.Math;
using GTA.Native;
using static FusionLibrary.FusionEnums;

namespace FusionLibrary
{
    public delegate void OnSwitchingComplete();
    public delegate void OnSwitchingStart();

    public static class PlayerSwitch
    {
        public static bool IsInProgress => Function.Call<bool>(Hash.IS_PLAYER_SWITCH_IN_PROGRESS);

        public static bool IsManualInProgress => IsInProgress && IsSwitching;

        public static bool Disable { get; set; } = false;

        public static SwitchTypes SwitchType { get; private set; }
        public static bool IsSwitching { get; private set; }
        public static Ped To { get; private set; }
        public static bool ForceShort { get; set; }
        public static OnSwitchingComplete OnSwitchingComplete { get; set; }
        public static OnSwitchingStart OnSwitchingStart { get; set; }

        private static int _health;
        private static bool _ragdoll;

        public static void Switch(Ped to, bool forceShort, bool instant = false, bool fullHealth = false)
        {
            _health = to.Health;
            _ragdoll = to.IsRagdoll;

            if (instant)
            {
                Function.Call(Hash.CHANGE_PLAYER_PED, Game.Player, to, false, false);
                FusionUtils.PlayerPed.Health = _health;

                if (_ragdoll)
                {
                    FusionUtils.PlayerPed.Ragdoll(1);
                }

                OnSwitchingComplete?.Invoke();
                return;
            }

            To = to;
            ForceShort = forceShort;
            IsSwitching = true;

            if (ForceShort)
            {
                SwitchType = SwitchTypes.Short;
            }
            else
            {
                SwitchType = (SwitchTypes)Function.Call<int>(Hash.GET_IDEAL_PLAYER_SWITCH_TYPE, FusionUtils.PlayerPed.Position.X, FusionUtils.PlayerPed.Position.Y, FusionUtils.PlayerPed.Position.Z, to.Position.X, to.Position.Y, to.Position.Z);
            }

            Function.Call(Hash.START_PLAYER_SWITCH, FusionUtils.PlayerPed, To, 1024, SwitchType);
            Function.Call(Hash.CHANGE_PLAYER_PED, Game.Player, To, false, false);
            if (!fullHealth)
            {
                FusionUtils.PlayerPed.Health = _health;
            }

            OnSwitchingStart?.Invoke();
        }

        public static Ped CreatePedAndSwitch(out Ped originalPed, Vector3 pos, float heading, bool forceShort, bool instant = false, bool fullHealth = false)
        {
            originalPed = FusionUtils.PlayerPed;

            Ped clone = World.CreatePed(FusionUtils.PlayerPed.Model, pos, heading);

            if (!fullHealth)
            {
                clone.Health = originalPed.Health;
            }

            Function.Call(Hash.CLONE_PED_TO_TARGET, FusionUtils.PlayerPed, clone);

            Switch(clone, forceShort, instant);

            return clone;
        }

        internal static void Tick()
        {
            if (!IsSwitching)
            {
                return;
            }

            if (!Function.Call<bool>(Hash.IS_PLAYER_SWITCH_IN_PROGRESS))
            {
                Function.Call(Hash.SET_ENTITY_HEALTH, FusionUtils.PlayerPed, _health);

                if (_ragdoll)
                {
                    FusionUtils.PlayerPed.Ragdoll(1);
                }

                OnSwitchingComplete?.Invoke();
                IsSwitching = false;
                To = null;
            }
        }
    }
}
