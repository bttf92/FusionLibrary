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
        public int MaxSpawned { get; set; } = 1;
        public int MaxInWorld { get; set; } = 10;
        public bool SwapOnlyDesiredModels { get; set; }
        public List<string> ModelsToSwap { get; set; } = new List<string>();

        private int gameTime;
        private bool modelInit;
        private DateTime endTime;
        private CustomModel baseModel;
        private readonly List<CustomModel> swapModels = new List<CustomModel>();
        private float endSpan;

        public void Init()
        {
            baseModel = new CustomModel(Model);

            swapModels.Clear();
            foreach (string model in ModelsToSwap)
                swapModels.Add(new CustomModel(model));

            endTime = EndProductionDate.AddYears(5);
            endSpan = (float)(endTime - EndProductionDate).TotalSeconds;

            modelInit = true;
        }

        //public ModelSwap()
        //{

        //}

        //internal ModelSwap(VehicleModelInfo vehicleModelInfo, int year)
        //{
        //    Enabled = false;

        //    Model = vehicleModelInfo.Name;
        //    VehicleType = vehicleModelInfo.VehicleType;
        //    VehicleClass = vehicleModelInfo.VehicleClass;

        //    DateBased = true;
        //    StartProductionDate = new DateTime(year, 1, 1, 0, 0, 0);
        //    EndProductionDate = new DateTime(year + 5, 1, 1, 0, 0, 0);

        //    MaxSpawned = FusionUtils.Random.Next(1, 3);
        //}

        internal void Process()
        {
            if (!modelInit)
                Init();

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

                    //GTA.UI.Screen.ShowSubtitle($"{chanceMulti} {count} {tempMax} {vehicles.Count()} {endTime} {DateBased}");

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

        public override bool Equals(object obj)
        {
            return Model == ((ModelSwap)obj).Model;
        }

        public override int GetHashCode()
        {
            return Model.GetHashCode();
        }

        public override string ToString()
        {
            return Model;
        }
    }
}
