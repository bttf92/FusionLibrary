using GTA;
using GTA.Math;
using GTA.Native;
using System.Collections.Generic;
using static FusionLibrary.FusionEnums;

namespace FusionLibrary
{
    public class ParticlePlayer
    {
        internal static List<ParticlePlayer> GlobalParticlePlayerList = new List<ParticlePlayer>();

        internal static void TickAll()
        {
            for (int i = 0; i < GlobalParticlePlayerList.Count; i++)
                GlobalParticlePlayerList[i].Tick();
        }

        /// <summary>
        /// Handle of looped particle.
        /// </summary>
        public int Handle { get; protected set; }

        /// <summary>
        /// Asset name of particle.
        /// </summary>
        public string AssetName { get; }

        /// <summary>
        /// Effect name of particle.
        /// </summary>
        public string EffectName { get; }

        /// <summary>
        /// Particle type.
        /// </summary>
        public ParticleType ParticleType { get; }

        /// <summary>
        /// World position or offset of the particle.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Rotation of the particle.
        /// </summary>
        public Vector3 Rotation { get; set; }

        /// <summary>
        /// Size of the particle.
        /// </summary>
        public float Size { get; set; }

        /// <summary>
        /// Interval between particle spawns.
        /// </summary>
        public int Interval { get; set; }

        /// <summary>
        /// List of evolution parameters.
        /// </summary>
        public Dictionary<string, float> EvolutionParams { get; } = new Dictionary<string, float>();

        /// <summary>
        /// Owner of the particle.
        /// </summary>
        public Entity Entity { get; }

        /// <summary>
        /// Bone of entity where the particle is spawned.
        /// </summary>
        public EntityBone Bone { get; }

        /// <summary>
        /// If particle is spawned attached to an entity's bone.
        /// </summary>
        public bool ToBone { get; }

        /// <summary>
        /// If particle is spawned attached to an entity.
        /// </summary>
        public bool ToEntity { get; }

        /// <summary>
        /// If particle is currently being spawned.
        /// </summary>
        public bool IsPlaying { get; private set; }

        private int gameTime;

        /// <summary>
        /// Creates a new particle.
        /// </summary>
        /// <param name="assetName">Asset name</param>
        /// <param name="effectName">Effect name</param>
        /// <param name="particleType"><see cref="ParticleType"/></param>
        /// <param name="position">Position</param>
        /// <param name="rotation">Rotation</param>
        /// <param name="size">Size</param>
        public ParticlePlayer(string assetName, string effectName, ParticleType particleType, Vector3 position, Vector3 rotation, float size)
        {
            AssetName = assetName;
            EffectName = effectName;
            ParticleType = particleType;
            Position = position;
            Rotation = rotation;
            Size = size;
            AssetName = assetName;
            EffectName = effectName;

            Request();

            GlobalParticlePlayerList.Add(this);
        }

        /// <summary>
        /// Creates a new particle attached to an <paramref name="entity"/>.
        /// </summary>
        /// <param name="assetName">Asset name</param>
        /// <param name="effectName">Effect name</param>
        /// <param name="particleType"><see cref="ParticleType"/></param>
        /// <param name="entity"></param>
        /// <param name="offset">Offset</param>
        /// <param name="rotation">Rotation</param>
        /// <param name="size">Size</param>
        public ParticlePlayer(string assetName, string effectName, ParticleType particleType, Entity entity, Vector3 offset, Vector3 rotation, float size) : this(assetName, effectName, particleType, offset, rotation, size)
        {
            Entity = entity;
            ToEntity = true;
        }

        /// <summary>
        /// Creates a new particle attached to <paramref name="boneName"/> of <paramref name="entity"/>.
        /// </summary>
        /// <param name="assetName">Asset name</param>
        /// <param name="effectName">Effect name</param>
        /// <param name="particleType"><see cref="ParticleType"/></param>
        /// <param name="entity"><see cref="Entity"/> instance</param>
        /// <param name="boneName">Bone's name of <paramref name="entity"/>.</param>
        /// <param name="offset">Offset</param>
        /// <param name="rotation">Rotation</param>
        /// <param name="size">Size</param>
        public ParticlePlayer(string assetName, string effectName, ParticleType particleType, Entity entity, string boneName, Vector3 offset, Vector3 rotation, float size) : this(assetName, effectName, particleType, entity, offset, rotation, size)
        {
            Bone = Entity.Bones[boneName];
            ToBone = true;
        }

        /// <summary>
        /// Creates a new particle attached to <paramref name="boneIndex"/> of <paramref name="entity"/>.
        /// </summary>
        /// <param name="assetName">Asset name</param>
        /// <param name="effectName">Effect name</param>
        /// <param name="particleType"><see cref="ParticleType"/></param>
        /// <param name="entity"><see cref="Entity"/></param>
        /// <param name="boneIndex">Bone's index of <paramref name="entity"/>.</param>
        /// <param name="offset">Offset</param>
        /// <param name="rotation">Rotation</param>
        /// <param name="size">Size</param>
        public ParticlePlayer(string assetName, string effectName, ParticleType particleType, Entity entity, int boneIndex, Vector3 offset, Vector3 rotation, float size) : this(assetName, effectName, particleType, entity, offset, rotation, size)
        {
            Bone = Entity.Bones[boneIndex];
            ToBone = true;
        }

        /// <summary>
        /// Requests the particle.
        /// </summary>
        public void Request()
        {
            Function.Call(Hash.REQUEST_NAMED_PTFX_ASSET, AssetName);

            while (!Function.Call<bool>(Hash.HAS_NAMED_PTFX_ASSET_LOADED, AssetName))
                Script.Yield();
        }

        internal void Tick()
        {
            if (!IsPlaying || ParticleType != ParticleType.LoopedManually || Game.GameTime < gameTime)
                return;

            Spawn();

            gameTime = Game.GameTime + Interval;
        }

        /// <summary>
        /// Sets the evolution param with <paramref name="key"/> and <paramref name="value"/>.
        /// </summary>
        /// <param name="key">Name of the parameter.</param>
        /// <param name="value">Value of the parameter.</param>
        public void SetEvolutionParam(string key, float value)
        {
            EvolutionParams[key] = value;

            Function.Call(Hash.SET_PARTICLE_FX_LOOPED_EVOLUTION, Handle, key, value, true);
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
        /// Spawns the particle and loops it if meant to.
        /// </summary>
        public void Play()
        {
            Spawn();

            if (ParticleType == ParticleType.SingleSpawn)
                return;

            IsPlaying = true;
        }

        /// <summary>
        /// Stops looping the particle.
        /// </summary>
        /// <param name="instant"><c>true</c> particle will be instantly removed from game's world.</param>
        public void Stop(bool instant)
        {
            IsPlaying = false;

            if (ParticleType != ParticleType.Looped)
                return;

            if (instant)
                Function.Call(Hash.REMOVE_PARTICLE_FX, Handle, 0);
            else
                Function.Call(Hash.STOP_PARTICLE_FX_LOOPED, Handle, 0);
        }

        /// <summary>
        /// Checks if the looped particle exists.
        /// </summary>
        /// <returns><c>true</c> if particle exists; otherwise <c>false</c>.</returns>
        public bool Exists()
        {
            if (ParticleType != ParticleType.Looped)
                return false;

            return Function.Call<bool>(Hash.DOES_PARTICLE_FX_LOOPED_EXIST, Handle);
        }

        /// <summary>
        /// Disposes the particle.
        /// </summary>
        public void Dispose()
        {
            Stop(true);

            GlobalParticlePlayerList.Remove(this);
        }

        /// <summary>
        /// Spawns manually a copy of the particle.
        /// </summary>
        public void Spawn()
        {
            Request();

            Function.Call(Hash.USE_PARTICLE_FX_ASSET, AssetName);

            switch (ParticleType)
            {
                case ParticleType.SingleSpawn:
                case ParticleType.LoopedManually:

                    if (!ToEntity && !ToBone)
                        Function.Call(Hash.START_PARTICLE_FX_NON_LOOPED_AT_COORD, EffectName, Position.X, Position.Y, Position.Z, Rotation.X, Rotation.Y, Rotation.Z, Size, false, false, false);
                    else if (ToEntity && !ToBone)
                        Function.Call(Hash.START_PARTICLE_FX_NON_LOOPED_ON_ENTITY, EffectName, Entity.Handle, Position.X, Position.Y, Position.Z, Rotation.X, Rotation.Y, Rotation.Z, Size, false, false, false);
                    else
                    {
                        Vector3 position = Bone.GetRelativeOffsetPosition(Position);

                        Function.Call(Hash.START_PARTICLE_FX_NON_LOOPED_ON_ENTITY, EffectName, Entity.Handle, position.X, position.Y, position.Z, Rotation.X, Rotation.Y, Rotation.Z, Size, false, false, false);
                    }

                    break;
                case ParticleType.Looped:

                    if (!ToEntity && !ToBone)
                        Handle = Function.Call<int>(Hash.START_PARTICLE_FX_LOOPED_AT_COORD, EffectName, Position.X, Position.Y, Position.Z, Rotation.X, Rotation.Y, Rotation.Z, Size, false, false, false);
                    else if (ToEntity && !ToBone)
                        Handle = Function.Call<int>(Hash.START_PARTICLE_FX_NON_LOOPED_ON_ENTITY, EffectName, Entity.Handle, Position.X, Position.Y, Position.Z, Rotation.X, Rotation.Y, Rotation.Z, Size, false, false, false);
                    else
                        Handle = Function.Call<int>(Hash.START_PARTICLE_FX_LOOPED_ON_ENTITY_BONE, EffectName, Entity.Handle, Position.X, Position.Y, Position.Z, Rotation.X, Rotation.Y, Rotation.Z, Bone.Index, Size, false, false, false);

                    break;
            }

            if (ParticleType != ParticleType.Looped)
                return;

            foreach (KeyValuePair<string, float> entry in EvolutionParams)
                Function.Call(Hash.SET_PARTICLE_FX_LOOPED_EVOLUTION, Handle, entry.Key, entry.Value, true);
        }

        public void Color(float r, float g, float b)
        {
            if (ParticleType == ParticleType.Looped)
                Function.Call(Hash.SET_​PARTICLE_​FX_​LOOPED_​COLOUR, Handle, r, g, b, 0);
            else
                Function.Call(Hash.SET_PARTICLE_FX_NON_LOOPED_COLOUR, r, g, b);
        }
    }
}
