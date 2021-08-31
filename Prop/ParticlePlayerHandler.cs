using GTA;
using GTA.Math;
using System.Collections.Generic;
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
        /// <c>true</c> if any particle is playing; otherwise <c>false</c>.
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
        public ParticlePlayer Add(string assetName, string effectName, ParticleType particleType, Vector3 position, Vector3 rotation, float size = 1f)
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
        public ParticlePlayer Add(string assetName, string effectName, ParticleType particleType, Entity entity, Vector3 offset, Vector3 rotation, float size = 1f)
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
        public ParticlePlayer Add(string assetName, string effectName, ParticleType particleType, Entity entity, string boneName, Vector3 offset, Vector3 rotation, float size = 1f)
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
        public ParticlePlayer Add(string assetName, string effectName, ParticleType particleType, Entity entity, int boneIndex, Vector3 offset, Vector3 rotation, float size = 1f)
        {
            ParticlePlayer particlePlayer = new ParticlePlayer(assetName, effectName, particleType, entity, boneIndex, offset, rotation, size);

            ParticlePlayers.Add(particlePlayer);

            return particlePlayer;
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
                return value;

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
        /// <param name="instant"><c>true</c> particle will be instantly removed from game's world.</param>
        public void Stop(bool instant = false)
        {
            ParticlePlayers.ForEach(x => x.Stop(instant));
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
