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
                if (!x.IsFunctioning() || x.IsDMC12TimeMachine())
                {
                    RemoveUsedVehicle.Add(x);
                }
            });

            if (RemoveUsedVehicle.Count > 0)
            {
                RemoveUsedVehicle.ForEach(x => UsedVehiclesByPlayer.Remove(x));
                RemoveUsedVehicle.Clear();
            }

            if (FusionUtils.PlayerVehicle.IsFunctioning() && !FusionUtils.PlayerVehicle.IsDMC12TimeMachine() && !FusionUtils.PlayerVehicle.IsTrain && !UsedVehiclesByPlayer.Contains(FusionUtils.PlayerVehicle))
            {
                UsedVehiclesByPlayer.Add(FusionUtils.PlayerVehicle);
            }

            bool isNight = FusionUtils.CurrentTime.Hour >= 20 || (FusionUtils.CurrentTime.Hour >= 0 && FusionUtils.CurrentTime.Hour <= 5);

            if (isNight != IsNight)
            {
                IsNight = isNight;
                OnDayNightChange?.Invoke();
            }

            if (!TrafficVolumeYearBased)
            {
                return;
            }

            float vehDensity = 1;

            float year = FusionUtils.CurrentTime.Year;

            if (year > 1900 && year < 1950)
            {
                year -= 1900;

                vehDensity = year / 50f;
            }
            else if (year <= 1900)
            {
                vehDensity = 0;
            }

            if (vehDensity >= 1)
            {
                return;
            }

            Function.Call(Hash.SET_VEHICLE_DENSITY_MULTIPLIER_THIS_FRAME, vehDensity);
            Function.Call(Hash.SET_PARKED_VEHICLE_DENSITY_MULTIPLIER_THIS_FRAME, vehDensity);
            Function.Call(Hash.SET_RANDOM_VEHICLE_DENSITY_MULTIPLIER_THIS_FRAME, vehDensity);
        }

        public static void TimeTravelTo(DateTime destinationTime)
        {
            MomentReplica momentReplica = MomentReplica.SearchForMoment();

            if (momentReplica == null)
            {
                MomentReplica.MomentReplicas.Add(new MomentReplica());
            }
            else
            {
                momentReplica.Update();
            }

            FusionUtils.ClearWorld();

            UsedVehiclesByPlayer.Clear();

            FusionUtils.CurrentTime = destinationTime;

            momentReplica = MomentReplica.SearchForMoment();

            if (momentReplica == null)
            {
                MomentReplica.Randomize();
            }
            else
            {
                momentReplica.Apply();
            }

            OnTimeChanged?.Invoke(destinationTime);
        }
    }
}
