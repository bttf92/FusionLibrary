using GTA;

namespace FusionLibrary
{
    public delegate void OnPlayerCompleted();

    public abstract class Player
    {
        public Entity Entity { get; protected set; }

        public OnPlayerCompleted OnPlayerCompleted { get; set; }

        public bool IsPlaying { get; protected set; }

        public abstract void Play();

        public abstract void Process();

        public abstract void Stop();

        public abstract void Dispose();
    }
}
