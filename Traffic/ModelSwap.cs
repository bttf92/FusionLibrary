using FusionLibrary.Extensions;
using GTA;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FusionLibrary
{
    public class ModelSwaps : ICollection
    {
        private readonly ArrayList modelSwaps = new ArrayList();

        public int Count => modelSwaps.Count;

        public object SyncRoot => this;

        public bool IsSynchronized => false;

        public ModelSwap this[int index] => (ModelSwap)modelSwaps[index];

        public void Add(ModelSwap item)
        {
            modelSwaps.Add(item);
        }

        public void Clear()
        {
            modelSwaps.Clear();
        }

        public bool Contains(ModelSwap item)
        {
            return modelSwaps.Contains(item);
        }

        public void CopyTo(Array array, int index)
        {
            modelSwaps.CopyTo(array, index);
        }

        public IEnumerator GetEnumerator()
        {
            return modelSwaps.GetEnumerator();
        }
    }

    [Serializable]
    public class ModelSwap
    {
        public bool Enabled { get; set; }
        public string Model { get; set; }
        public VehicleType VehicleType { get; set; }
        public VehicleClass VehicleClass { get; set; }
        public bool DateBased { get; set; }
        public DateTime StartProductionDate { get; set; }
        public DateTime EndProductionDate { get; set; }
        public int WaitBetweenSpawns { get; set; } = -1;
        public float ChanceOfSpawn { get; set; } = 1;
        public int MaxSpawned { get; set; } = -1;
        public int MaxInWorld { get; set; } = -1;
        public bool SwapOnlyDesiredModels { get; set; }
        public List<string> ModelsToSwap { get; set; } = new List<string>();

        private int gameTime;
        private bool modelInit;
        private DateTime endTime;
        private CustomModel baseModel;
        private readonly List<CustomModel> swapModels = new List<CustomModel>();
        private float endSpan;

        private void InitModels()
        {
            baseModel = new CustomModel(Model);

            foreach (string model in ModelsToSwap)
                swapModels.Add(new CustomModel(model));

            endTime = EndProductionDate.AddYears(5);
            endSpan = (float)(endTime - EndProductionDate).TotalSeconds;

            modelInit = true;
        }

        internal void Process()
        {
            if (!modelInit)
                InitModels();

            if (!Enabled || Game.GameTime < gameTime || (DateBased && !FusionUtils.CurrentTime.Between(StartProductionDate, endTime)) || FusionUtils.AllVehicles.Count(x => x.Model == baseModel) >= MaxInWorld)
                return;

            float chanceMulti = 1f;

            if (FusionUtils.CurrentTime.Between(EndProductionDate, endTime))
            {
                chanceMulti = (float)(endTime - FusionUtils.CurrentTime).TotalSeconds;
                chanceMulti = chanceMulti.Remap(0, endSpan, 0, 1);
                chanceMulti = Math.Max(chanceMulti, 0.02f);
            }

            float chance = (float)Math.Round(FusionUtils.Random.NextDouble(), 2);

            //GTA.UI.Screen.ShowSubtitle($"{chance} {ChanceOfSpawn * chanceMulti}");

            if (chance < (ChanceOfSpawn * chanceMulti))
            {
                IEnumerable<Vehicle> vehicles = FusionUtils.AllVehicles.Where(x => FusionUtils.PlayerVehicle != x && x.IsAlive && !x.Decorator().DrivenByPlayer && !x.Decorator().IgnoreForSwap);

                int count = vehicles.Count(x => x.Model == baseModel);

                int tempMax = (int)Math.Round(Math.Max(MaxSpawned * chanceMulti, 1));

                if (count < tempMax)
                {
                    vehicles = vehicles.Where(x => x.Model != baseModel && !x.Decorator().ModelSwapped && ((SwapOnlyDesiredModels && swapModels.Contains(x.Model)) || (!SwapOnlyDesiredModels && x.Type == VehicleType && x.ClassType == VehicleClass))).SelectRandomElements(tempMax - count);

                    //GTA.UI.Screen.ShowSubtitle($"{chanceMulti} {count} {tempMax} {vehicles.Count()}");

                    foreach (Vehicle vehicle in vehicles)
                    {
                        float dist = vehicle.DistanceToSquared2D(FusionUtils.PlayerPed);

                        if (dist < 50 * 50)
                            break;

                        vehicle.Replace(baseModel).Decorator().ModelSwapped = true;
                    }
                }
            }

            gameTime = Game.GameTime + WaitBetweenSpawns;
        }
    }
}
