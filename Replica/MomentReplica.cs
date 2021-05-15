using FusionLibrary.Extensions;
using FusionLibrary.Memory;
using GTA;
using GTA.Native;
using System;
using System.Collections.Generic;
using static FusionLibrary.FusionEnums;

namespace FusionLibrary
{
    [Serializable]
    public class MomentReplica
    {
        public static List<MomentReplica> MomentReplicas = new List<MomentReplica>();

        public Weather Weather { get; set; } = Weather.ExtraSunny;
        public int WantedLevel { get; set; } = 0;
        public float PuddleLevel { get; set; } = 0f;
        public float RainLevel { get; set; } = -1f;
        public DateTime CurrentDate { get; set; }
        public List<VehicleReplica> VehicleReplicas { get; set; }

        public MomentReplica(DateTime dateTime)
        {
            CurrentDate = dateTime;
            MomentReplicas.Add(this);
        }

        public MomentReplica()
        {
            Update();
        }

        public static MomentReplica SearchForMoment()
        {
            foreach (MomentReplica momentReplica in MomentReplicas)
                if (momentReplica.IsNow())
                    return momentReplica;

            return null;
        }

        public bool IsNow()
        {
            if (CurrentDate.Between(FusionUtils.CurrentTime.AddMinutes(-10), FusionUtils.CurrentTime.AddMinutes(10)))
                return true;

            return false;
        }

        public static void Randomize()
        {
            // Set the weather to a random weather
            Function.Call(Hash.SET_RANDOM_WEATHER_TYPE);

            // Initial puddle level
            float puddleLevel = 0;

            // If the weather is raining
            if (World.Weather == Weather.Raining)
            {
                // Set the puddle to a random number between 0.4 and 0.8
                puddleLevel = (float)FusionUtils.Random.NextDouble(0.4, 0.8);
            }
            // If the weather is clearing
            else if (World.Weather == Weather.Clearing)
            {
                // Set the puddle to 0.2
                puddleLevel = 0.2f;
            }
            // If the weather is a thunderstorm
            else if (World.Weather == Weather.ThunderStorm)
            {
                // Set the puddle to 0.9f
                puddleLevel = 0.9f;
            }

            // Apply the puddle level
            RainPuddleEditor.Level = puddleLevel;

            // Reset wanted level
            Game.Player.WantedLevel = 0;

            MomentReplicas.Add(new MomentReplica());
        }

        public void Apply()
        {
            World.Weather = Weather;
            RainPuddleEditor.Level = PuddleLevel;
            FusionUtils.RainLevel = RainLevel;
            Game.Player.WantedLevel = WantedLevel;

            VehicleReplicas?.ForEach(x => TimeHandler.UsedVehiclesByPlayer.Add(x.Spawn(SpawnFlags.Default)));
        }

        public void Update()
        {
            CurrentDate = FusionUtils.CurrentTime;
            Weather = World.Weather;
            RainLevel = FusionUtils.RainLevel;
            PuddleLevel = RainPuddleEditor.Level;
            WantedLevel = Game.Player.WantedLevel;

            VehicleReplicas = new List<VehicleReplica>();

            TimeHandler.UsedVehiclesByPlayer.ForEach(x =>
            {
                if (x != FusionUtils.PlayerVehicle)
                    VehicleReplicas.Add(new VehicleReplica(x));
            });
        }
    }
}
