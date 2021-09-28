using GTA;
using GTA.Math;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using static FusionLibrary.FusionEnums;

namespace FusionLibrary
{
    public delegate void OnParticleSequenceCompleted(bool isStop);

    public class ParticlePlayerHandler
    {
        internal static List<ParticlePlayerHandler> GlobalParticlePlayerHandlerList = new List<ParticlePlayerHandler>();

        internal static void TickAll()
        {
            for (int i = 0; i < GlobalParticlePlayerHandlerList.Count; i++)
            {
                GlobalParticlePlayerHandlerList[i].Tick();
            }
        }

        public event OnParticleSequenceCompleted OnParticleSequenceCompleted;

        /// <summary>
        /// List of particles.
        /// </summary>
        public List<ParticlePlayer> ParticlePlayers { get; } = new List<ParticlePlayer>();

        /// <summary>
        /// List of evolution parameters.
        /// </summary>
        public Dictionary<string, float> EvolutionParams { get; } = new Dictionary<string, float>();

        /// <summary>
        /// <see langword="true"/> if any particle is playing; otherwise <see langword="false"/>.
        /// </summary>
        public bool IsPlaying
        {
            get
            {
                return ParticlePlayers.Any(x => x.IsPlaying);
            }
        }

        /// <summary>
        /// Interval between each spawn of the sequence. Default <see langword="0"/>.
        /// </summary>
        public int SequenceInterval { get; set; } = 0;

        private int _gameTime = 0;

        private bool _stopSequence;
        private bool _fromFirst;

        private readonly FrameTimeHelper frameTimeHelper = new FrameTimeHelper();

        /// <summary>
        /// Whether use <see cref="FrameTimeHelper"/> for sequence spawn.
        /// </summary>
        public bool UseFrameTimeHelper { get; set; }

        /// <summary>
        /// <see cref="FrameTimeHelper.FPS"/>.
        /// </summary>
        public float TargetFPS
        {
            get
            {
                return frameTimeHelper.FPS;
            }

            set
            {
                frameTimeHelper.FPS = value;
            }
        }

        /// <summary>
        /// Change of single particle spawn during a sequence. Default <see langword="1"/>.
        /// </summary>
        public float ChanceOfSpawn { get; set; } = 1f;

        /// <summary>
        /// How much time wait after spawn sequence.
        /// </summary>
        public int WaitForStop { get; set; } = 0;

        /// <summary>
        /// Current spawned particle.
        /// </summary>
        public int CurrentSequenceElement { get; private set; } = -1;

        /// <summary>
        /// Whether sequence is complete.
        /// </summary>
        public bool SequenceComplete { get; private set; }

        public ParticlePlayerHandler()
        {
            GlobalParticlePlayerHandlerList.Add(this);
        }

        /// <summary>
        /// Creates a new particle.
        /// </summary>
        /// <param name="assetName">Asset name.</param>
        /// <param name="effectName">Effect name.</param>
        /// <param name="particleType"><see cref="ParticleType"/>.</param>
        /// <param name="position">Position.</param>
        /// <param name="rotation">Rotation.</param>
        /// <param name="size">Size.</param>
        public ParticlePlayer Add(string assetName, string effectName, ParticleType particleType, Vector3 position = default, Vector3 rotation = default, float size = 1f)
        {
            ParticlePlayer particlePlayer = new ParticlePlayer(assetName, effectName, particleType, position, rotation, size);

            ParticlePlayers.Add(particlePlayer);

            return particlePlayer;
        }

        /// <summary>
        /// Creates a new particle attached to an <paramref name="entity"/>.
        /// </summary>
        /// <param name="assetName">Asset name.</param>
        /// <param name="effectName">Effect name.</param>
        /// <param name="particleType"><see cref="ParticleType"/>.</param>
        /// <param name="entity"><see cref="Entity"/> instance.</param>
        /// <param name="offset">Offset.</param>
        /// <param name="rotation">Rotation.</param>
        /// <param name="size">Size.</param>
        public ParticlePlayer Add(string assetName, string effectName, ParticleType particleType, Entity entity, Vector3 offset = default, Vector3 rotation = default, float size = 1f)
        {
            ParticlePlayer particlePlayer = new ParticlePlayer(assetName, effectName, particleType, entity, offset, rotation, size);

            ParticlePlayers.Add(particlePlayer);

            return particlePlayer;
        }

        /// <summary>
        /// Creates a new particle attached to <paramref name="boneName"/> of <paramref name="entity"/>.
        /// </summary>
        /// <param name="assetName">Asset name.</param>
        /// <param name="effectName">Effect name.</param>
        /// <param name="particleType"><see cref="ParticleType"/>.</param>
        /// <param name="entity"><see cref="Entity"/> instance.</param>
        /// <param name="boneName">Bone's name of <paramref name="entity"/>.</param>
        /// <param name="offset">Offset.</param>
        /// <param name="rotation">Rotation.</param>
        /// <param name="size">Size.</param>
        public ParticlePlayer Add(string assetName, string effectName, ParticleType particleType, Entity entity, string boneName, Vector3 offset = default, Vector3 rotation = default, float size = 1f)
        {
            ParticlePlayer particlePlayer = new ParticlePlayer(assetName, effectName, particleType, entity, boneName, offset, rotation, size);

            ParticlePlayers.Add(particlePlayer);

            return particlePlayer;
        }

        /// <summary>
        /// Creates a new particle attached to <paramref name="boneIndex"/> of <paramref name="entity"/>.
        /// </summary>
        /// <param name="assetName">Asset name.</param>
        /// <param name="effectName">Effect name.</param>
        /// <param name="particleType"><see cref="ParticleType"/>.</param>
        /// <param name="entity"><see cref="Entity"/> instance.</param>
        /// <param name="boneIndex">Bone's index of <paramref name="entity"/>.</param>
        /// <param name="offset">Offset.</param>
        /// <param name="rotation">Rotation.</param>
        /// <param name="size">Size.</param>
        public ParticlePlayer Add(string assetName, string effectName, ParticleType particleType, Entity entity, int boneIndex, Vector3 offset = default, Vector3 rotation = default, float size = 1f)
        {
            ParticlePlayer particlePlayer = new ParticlePlayer(assetName, effectName, particleType, entity, boneIndex, offset, rotation, size);

            ParticlePlayers.Add(particlePlayer);

            return particlePlayer;
        }

        private bool Spawn()
        {
            CurrentSequenceElement++;

            if (FusionUtils.Random.NextDouble() <= ChanceOfSpawn)
            {
                ParticlePlayers[CurrentSequenceElement].Play();
            }

            if (CurrentSequenceElement == ParticlePlayers.Count - 1)
            {
                _gameTime = 0;
                SequenceComplete = true;
                OnParticleSequenceCompleted?.Invoke(_stopSequence);

                if (WaitForStop > 0)
                {
                    _gameTime = Game.GameTime + WaitForStop;
                    _stopSequence = true;
                    _fromFirst = true;
                    SequenceComplete = false;
                    CurrentSequenceElement = -1;
                }

                return false;
            }

            _gameTime = Game.GameTime + SequenceInterval;

            return true;
        }

        private bool Remove()
        {
            if (_fromFirst)
            {
                CurrentSequenceElement++;
            }
            else
            {
                CurrentSequenceElement--;
            }

            ParticlePlayers[CurrentSequenceElement].Stop();

            if (CurrentSequenceElement == (_fromFirst ? ParticlePlayers.Count - 1 : 0))
            {
                _gameTime = 0;
                SequenceComplete = true;
                OnParticleSequenceCompleted?.Invoke(true);

                return false;
            }

            _gameTime = Game.GameTime + SequenceInterval;

            return true;
        }

        internal void Tick()
        {
            //if (WaitForStop != 0)
            //    GTA.UI.Screen.ShowSubtitle($"{IsPlaying} {WaitForStop} {_stopSequence} {SequenceComplete} {Game.GameTime} {_gameTime} {UseFrameTimeHelper} {CurrentSequenceElement}");

            if (!IsPlaying || SequenceComplete)
            {
                return;
            }

            if (!UseFrameTimeHelper && (_gameTime == 0 || _gameTime > Game.GameTime))
            {
                return;
            }

            if (UseFrameTimeHelper)
            {
                frameTimeHelper.Tick();

                for (int i = 0; i < frameTimeHelper.Count; i++)
                {
                    if ((_stopSequence && !Remove()) || (!_stopSequence && !Spawn()))
                    {
                        frameTimeHelper.Reset();
                        break;
                    }
                }
            }
            else
            {
                if (_stopSequence)
                {
                    Remove();
                }
                else
                {
                    Spawn();
                }
            }
        }

        /// <summary>
        /// World position or offset of the particles.
        /// </summary>
        public Vector3 Position
        {
            get
            {
                return ParticlePlayers[0].Position;
            }

            set
            {
                ParticlePlayers.ForEach(x => x.Position = value);
            }
        }

        /// <summary>
        /// Rotation of the particles.
        /// </summary>
        public Vector3 Rotation
        {
            get
            {
                return ParticlePlayers[0].Rotation;
            }

            set
            {
                ParticlePlayers.ForEach(x => x.Rotation = value);
            }
        }

        /// <summary>
        /// Size of the particles.
        /// </summary>
        public float Size
        {
            get
            {
                return ParticlePlayers[0].Size;
            }

            set
            {
                ParticlePlayers.ForEach(x => x.Size = value);
            }
        }

        /// <summary>
        /// Gets or sets if color of particles needs to be setted.
        /// </summary>
        public bool SetColor
        {
            get
            {
                return ParticlePlayers[0].SetColor;
            }

            set
            {
                ParticlePlayers.ForEach(x => x.SetColor = value);
            }
        }

        /// <summary>
        /// Color of the particles.
        /// </summary>
        public Color Color
        {
            get
            {
                return ParticlePlayers[0].Color;
            }

            set
            {
                ParticlePlayers.ForEach(x => x.Color = value);
            }
        }

        /// <summary>
        /// Interval between particles spawns.
        /// </summary>
        public int Interval
        {
            get
            {
                return ParticlePlayers[0].Interval;
            }

            set
            {
                ParticlePlayers.ForEach(x => x.Interval = value);
            }
        }

        /// <summary>
        /// Gets or sets the duration of the particles.
        /// </summary>
        public int Duration
        {
            get
            {
                return ParticlePlayers[0].Duration;
            }

            set
            {
                ParticlePlayers.ForEach(x => x.Duration = value);
            }
        }

        /// <summary>
        /// Sets the evolution param with <paramref name="key"/> and <paramref name="value"/>.
        /// </summary>
        /// <param name="key">Name of the parameter.</param>
        /// <param name="value">Value of the parameter.</param>
        public void SetEvolutionParam(string key, float value)
        {
            EvolutionParams[key] = value;

            ParticlePlayers.ForEach(x => x.SetEvolutionParam(key, value));
        }

        /// <summary>
        /// Gets the value of evolution parameter.
        /// </summary>
        /// <param name="key">Name of the parameter.</param>
        /// <returns>Value of the parameter.</returns>
        public float GetEvolutionParam(string key)
        {
            if (EvolutionParams.TryGetValue(key, out float value))
            {
                return value;
            }

            return -1f;
        }

        /// <summary>
        /// Spawns the particles and loops them if meant to.
        /// </summary>
        public void Play(bool stopFromFirst = true)
        {
            _fromFirst = stopFromFirst;

            if (SequenceInterval == 0 && !UseFrameTimeHelper)
            {
                ParticlePlayers.ForEach(x => x.Play());

                if (WaitForStop > 0)
                {
                    CurrentSequenceElement = -1;

                    _stopSequence = true;
                    _gameTime = Game.GameTime + WaitForStop;
                }
            }
            else
            {
                ParticlePlayers[0].Play();

                CurrentSequenceElement = 0;

                _stopSequence = false;
                _gameTime = Game.GameTime + SequenceInterval;
            }

            SequenceComplete = false;
        }

        /// <summary>
        /// Stops looping the particles.
        /// </summary>
        /// <param name="instant"><see langword="true"/> particle will be instantly removed from game's world.</param>
        public void Stop(bool instant = false)
        {
            ParticlePlayers.ForEach(x => x.Stop(instant));
        }

        /// <summary>
        /// Starts the remove particle sequence.
        /// </summary>
        /// <param name="fromFirst">Whether start removing particles from the first one.</param>
        public void StopInSequence(bool fromFirst = true)
        {
            if (fromFirst)
            {
                CurrentSequenceElement = 0;
            }
            else
            {
                CurrentSequenceElement = ParticlePlayers.Count - 1;
            }

            ParticlePlayers[CurrentSequenceElement]?.Stop();
            SequenceComplete = false;
            _stopSequence = true;
            _fromFirst = fromFirst;
            _gameTime = Game.GameTime + SequenceInterval;
        }

        /// <summary>
        /// Toggles play/stop state.
        /// </summary>
        /// <param name="state">State of the particles.</param>
        public void SetState(bool state)
        {
            ParticlePlayers.ForEach(x => x.SetState(state));
        }

        /// <summary>
        /// Disposes the particles.
        /// </summary>
        public void Dispose()
        {
            ParticlePlayers.ForEach(x => x.Dispose());

            ParticlePlayers.Clear();

            GlobalParticlePlayerHandlerList.Remove(this);
        }

        public ParticlePlayer this[int index]
        {
            get
            {
                return ParticlePlayers[index];
            }
        }
    }
}
