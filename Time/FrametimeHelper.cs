using GTA;
using System;

namespace FusionLibrary
{
    public class FrametimeHelper
    {
        private double remainder;
        public float BaseFPS { get; }

        public int Count { get; private set; }

        public FrametimeHelper(float fps)
        {
            BaseFPS = fps;

            Reset();
        }

        public void Reset()
        {
            remainder = 0;
            Count = 0;
        }

        public void Tick()
        {
            float delta = BaseFPS / Game.FPS;

            Count = (int)Math.Truncate(delta);

            remainder += delta - Count;

            if (remainder >= 1)
            {
                int _remainder = (int)Math.Truncate(remainder);

                Count += _remainder;
                remainder -= _remainder;
            }
        }
    }
}
