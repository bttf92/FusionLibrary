using FusionLibrary.Extensions;
using GTA;
using GTA.Native;
using System;
using System.Collections.Generic;
using static FusionLibrary.FusionEnums;

namespace FusionLibrary
{
    public delegate void OnTimeChanged(DateTime time);
    public delegate void OnDayNightChange();

    public class TimeHandler
    {
        public static List<Vehicle> UsedVehiclesByPlayer { get; } = new List<Vehicle>();
        private static readonly List<Vehicle> RemoveUsedVehicle = new List<Vehicle>();

        public static OnTimeChanged OnTimeChanged;
        public static OnDayNightChange OnDayNightChange;

        public static bool IsNight { get; internal set; }

        public static bool TrafficVolumeYearBased { get; set; }

        public static bool MissionTraffic = false;

        public static bool RealTime
        {
            get => realTime;

            set
            {
                realTime = value;
                realSecond = Game.GameTime + 1000;
                World.IsClockPaused = value;
            }
        }
        private static bool realTime;
        private static int realSecond;

        public static void SetTimer(ScriptTimer scriptTimer, int value)
        {
            Function.Call(scriptTimer == ScriptTimer.A ? Hash.SETTIMERA : Hash.SETTIMERB, value);
        }

        public static int GetTimer(ScriptTimer scriptTimer)
        {
            return Function.Call<int>(scriptTimer == ScriptTimer.A ? Hash.TIMERA : Hash.TIMERB);
        }

        internal static void Tick()
        {
            if (realTime)
            {
                World.IsClockPaused = true;

                if (Game.GameTime > realSecond)
                {
                    Function.Call(Hash.ADD_TO_CLOCK_TIME, 0, 0, 1);
                    realSecond = Game.GameTime + 1000;
                }
            }

            UsedVehiclesByPlayer.ForEach(x =>
            {
                // || x.IsDMC12TimeMachine()
                if (!x.IsFunctioning() || x.Decorator().RemoveFromUsed)
                {
                    RemoveUsedVehicle.Add(x);
                }
            });

            if (RemoveUsedVehicle.Count > 0)
            {
                RemoveUsedVehicle.ForEach(x => UsedVehiclesByPlayer.Remove(x));
                RemoveUsedVehicle.Clear();
            }

            // && !FusionUtils.PlayerVehicle.IsDMC12TimeMachine()
            if (FusionUtils.PlayerVehicle.IsFunctioning() && !FusionUtils.PlayerVehicle.IsTrain && !FusionUtils.PlayerVehicle.Decorator().DrivenByPlayer && !FusionUtils.PlayerVehicle.Decorator().RemoveFromUsed && !UsedVehiclesByPlayer.Contains(FusionUtils.PlayerVehicle))
            {
                UsedVehiclesByPlayer.Add(FusionUtils.PlayerVehicle);
                FusionUtils.PlayerVehicle.Decorator().DrivenByPlayer = true;
            }

            bool isNight = FusionUtils.CurrentTime.Hour >= 20 || (FusionUtils.CurrentTime.Hour >= 0 && FusionUtils.CurrentTime.Hour <= 5);

            if (isNight != IsNight)
            {
                IsNight = isNight;
                OnDayNightChange?.Invoke();
            }

            if (!TrafficVolumeYearBased || MissionTraffic)
            {
                return;
            }

            float vehDensity = 1;

            float year = FusionUtils.CurrentTime.Year;

            if (year > 1900 && year < 1950)
            {
                year -= 1900;

                if (!FusionUtils.IsTrafficAlive)
                    FusionUtils.IsTrafficAlive = true;

                vehDensity = year / 50f;
            }
            else if (year <= 1900)
            {
                vehDensity = 0;

                if (FusionUtils.IsTrafficAlive)
                    FusionUtils.IsTrafficAlive = false;
            }

            if (vehDensity >= 1)
            {
                if (!FusionUtils.IsTrafficAlive)
                    FusionUtils.IsTrafficAlive = true;

                return;
            }

            Function.Call(Hash.SET_VEHICLE_DENSITY_MULTIPLIER_THIS_FRAME, vehDensity);
            Function.Call(Hash.SET_PARKED_VEHICLE_DENSITY_MULTIPLIER_THIS_FRAME, vehDensity);
            Function.Call(Hash.SET_RANDOM_VEHICLE_DENSITY_MULTIPLIER_THIS_FRAME, vehDensity);
        }

        public static void TimeTravelTo(DateTime destinationTime)
        {
            new MomentReplica();

            FusionUtils.ClearWorld();

            UsedVehiclesByPlayer.Clear();

            FusionUtils.CurrentTime = destinationTime;

            MomentReplica momentReplica = MomentReplica.SearchForMoment();

            if (momentReplica == null)
            {
                MomentReplica.Randomize();
            }
            else
            {
                MomentReplica.MomentReplicas?.ForEach(x =>
                {
                    x.Applied = false;
                });
                momentReplica.Apply();
            }

            OnTimeChanged?.Invoke(destinationTime);
        }
    }
}