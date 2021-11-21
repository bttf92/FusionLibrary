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
        public bool DateBased { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Wait { get; set; } = -1;
        public int MaxSpawned { get; set; } = -1;
        public int MaxInWorld { get; set; } = -1;
        public bool SwapOnlyDesiredModels { get; set; }
        public List<string> ModelsToSwap { get; set; } = new List<string>();

        private int gameTime;
        private bool modelInit;
        private CustomModel baseModel;
        private readonly List<CustomModel> swapModels = new List<CustomModel>();

        private void InitModels()
        {
            baseModel = new CustomModel(Model);

            foreach (string model in ModelsToSwap)
                swapModels.Add(new CustomModel(model));

            modelInit = true;
        }

        public void Process()
        {
            if (!modelInit)
                InitModels();

            if (!Enabled || Game.GameTime < gameTime || (DateBased && !FusionUtils.CurrentTime.Between(StartDate, EndDate)) || FusionUtils.AllVehicles.Count(x => x.Model == baseModel) >= MaxInWorld)
                return;

            IEnumerable<Vehicle> vehicles = FusionUtils.AllVehicles.Where(x => FusionUtils.PlayerVehicle != x && x.Type == VehicleType && x.IsAlive && (!SwapOnlyDesiredModels || swapModels.Contains(x.Model)) && !x.Decorator().DrivenByPlayer && !x.Decorator().IgnoreForSwap);

            int count = vehicles.Count(x => x.Model == baseModel);

            if (count >= MaxSpawned)
                return;

            vehicles = vehicles.Where(x => !x.Decorator().ModelSwapped).SelectRandomElements(MaxSpawned - count);

            foreach (Vehicle vehicle in vehicles)
            {
                float dist = vehicle.DistanceToSquared2D(FusionUtils.PlayerPed);

                if (dist < 100 * 100)
                    break;

                VehicleReplica vehicleReplica = new VehicleReplica(vehicle);

                vehicle.DeleteCompletely();

                vehicleReplica.Model = baseModel;

                Vehicle newVehicle = vehicleReplica.Spawn(FusionEnums.SpawnFlags.Default | FusionEnums.SpawnFlags.NoWheels);

                while (!newVehicle.NotNullAndExists())
                    Script.Yield();

                newVehicle.PlaceOnGround();
                newVehicle.Decorator().ModelSwapped = true;

                //newVehicle.AddBlip();

                if (newVehicle.Driver.NotNullAndExists())
                    newVehicle.Driver.Task.CruiseWithVehicle(newVehicle, 30);

                foreach (Ped ped in newVehicle.Occupants)
                    ped?.MarkAsNoLongerNeeded();

                newVehicle.MarkAsNoLongerNeeded();
            }

            gameTime = Game.GameTime + Wait;
        }
    }
}
