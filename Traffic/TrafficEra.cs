using FusionLibrary.Extensions;
using System;
using System.Collections;

namespace FusionLibrary
{
    public class TrafficEras : ICollection
    {
        private readonly ArrayList trafficEras = new ArrayList();

        public int Count => trafficEras.Count;

        public object SyncRoot => this;

        public bool IsSynchronized => false;

        public TrafficEra this[int index] => (TrafficEra)trafficEras[index];

        public void CopyTo(Array array, int index)
        {
            trafficEras.CopyTo(array, index);
        }

        public IEnumerator GetEnumerator()
        {
            return trafficEras.GetEnumerator();
        }

        public void Add(TrafficEra trafficEra)
        {
            trafficEras.Add(trafficEra);
        }

        public bool Contains(TrafficEra trafficEra)
        {
            return trafficEras.Contains(trafficEra);
        }
    }

    [Serializable]
    public class TrafficEra
    {
        public bool Enabled { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool ReduceChanceNearEndDate { get; set; }

        public ModelSwaps ModelSwaps { get; } = new ModelSwaps();

        internal void Process()
        {
            if (!Enabled || !FusionUtils.CurrentTime.Between(StartDate, EndDate))
                return;

            foreach (ModelSwap modelSwap in ModelSwaps)
            {
                TimeSpan timeSpan = EndDate - StartDate;
                TimeSpan timeSpan1 = EndDate - FusionUtils.CurrentTime;

                float chanceMulti = ((float)timeSpan1.TotalSeconds).Remap((float)timeSpan.TotalSeconds, 0, 1, 0);

                modelSwap.Process(chanceMulti);
            }
        }
    }
}
