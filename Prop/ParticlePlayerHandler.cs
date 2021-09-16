using GTA;
using GTA.Math;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using static FusionLibrary.FusionEnums;

namespace FusionLibrary
{
    public class ParticlePlayerHandler
    {
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
        public bool IsPlaying => ParticlePlayers.Any(x => x.IsPlaying);

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

        /// <summary>
        /// World position or offset of the particles.
        /// </summary>
        public Vector3 Position
        {
            get => ParticlePlayers[0].Position;
            set => ParticlePlayers.ForEach(x => x.Position = value);
        }

        /// <summary>
        /// Rotation of the particles.
        /// </summary>
        public Vector3 Rotation
        {
            get => ParticlePlayers[0].Rotation;
            set => ParticlePlayers.ForEach(x => x.Rotation = value);
        }

        /// <summary>
        /// Size of the particles.
        /// </summary>
        public float Size
        {
            get => ParticlePlayers[0].Size;
            set => ParticlePlayers.ForEach(x => x.Size = value);
        }

        /// <summary>
        /// Gets or sets if color of particles needs to be setted.
        /// </summary>
        public bool SetColor
        {
            get => ParticlePlayers[0].SetColor;
            set => ParticlePlayers.ForEach(x => x.SetColor = value);
        }

        /// <summary>
        /// Color of the particles.
        /// </summary>
        public Color Color
        {
            get => ParticlePlayers[0].Color;
            set => ParticlePlayers.ForEach(x => x.Color = value);
        }

        /// <summary>
        /// Interval between particles spawns.
        /// </summary>
        public int Interval
        {
            get => ParticlePlayers[0].Interval;
            set => ParticlePlayers.ForEach(x => x.Interval = value);
        }

        /// <summary>
        /// Gets or sets the duration of the particles.
        /// </summary>
        public int Duration
        {
            get => ParticlePlayers[0].Duration;
            set => ParticlePlayers.ForEach(x => x.Duration = value);
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
        public void Play()
        {
            ParticlePlayers.ForEach(x => x.Play());
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
        }

        public ParticlePlayer this[int index] => ParticlePlayers[index];
    }
}
