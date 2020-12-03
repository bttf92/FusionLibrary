using GTA;

namespace FusionLibrary
{
    public delegate void OnCompleted();

    public abstract class Player
    {
        public Entity Entity { get; protected set; }

        public OnCompleted OnCompleted { get; set; }

        public bool IsPlaying { get; protected set; }

        public abstract void Play();

        public abstract void Process();

        public abstract void Stop();

        public abstract void Dispose();
    }
}
