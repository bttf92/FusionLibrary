using GTA;
using GTA.Math;
using GTA.Native;
using System.Collections.Generic;
using System.Drawing;
using static FusionLibrary.FusionEnums;

namespace FusionLibrary
{
    public class ParticlePlayer
    {
        internal static List<ParticlePlayer> GlobalParticlePlayerList = new List<ParticlePlayer>();

        internal static void TickAll()
        {
            for (int i = 0; i < GlobalParticlePlayerList.Count; i++)
            {
                GlobalParticlePlayerList[i].Tick();
            }
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

        private Vector3 _position;
        /// <summary>
        /// World position or offset of the particle.
        /// </summary>
        public Vector3 Position
        {
            get => _position;

            set
            {
                _position = value;

                if (!Exists())
                {
                    return;
                }

                Function.Call(Hash.SET_PARTICLE_FX_LOOPED_OFFSETS, Handle, Position.X, Position.Y, Position.Z, Rotation.X, Rotation.Y, Rotation.Z);
            }
        }

        private Vector3 _rotation;
        /// <summary>
        /// Rotation of the particle.
        /// </summary>
        public Vector3 Rotation
        {
            get => _rotation;

            set
            {
                _rotation = value;

                if (!Exists())
                {
                    return;
                }

                Function.Call(Hash.SET_PARTICLE_FX_LOOPED_OFFSETS, Handle, Position.X, Position.Y, Position.Z, Rotation.X, Rotation.Y, Rotation.Z);
            }
        }

        private float _size;
        /// <summary>
        /// Size of the particle.
        /// </summary>
        public float Size
        {
            get => _size;

            set
            {
                _size = value;

                if (!Exists())
                {
                    return;
                }

                Function.Call(Hash.SET_PARTICLE_FX_LOOPED_SCALE, Handle, Size);
            }
        }

        /// <summary>
        /// Interval between particle spawns.
        /// </summary>
        public int Interval { get; set; } = 30;

        /// <summary>
        /// Gets or sets the duration of the particle.
        /// </summary>
        public int Duration { get; set; } = -1;

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
        /// Name of the <see cref="Bone"/>.
        /// </summary>
        public string BoneName { get; }

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

        /// <summary>
        /// Gets or sets if color of particle needs to be setted.
        /// </summary>
        public bool SetColor { get; set; }

        private Color _color;
        /// <summary>
        /// Color of the particle.
        /// </summary>
        public Color Color
        {
            get => _color;

            set
            {
                _color = value;
                SetColor = true;

                ISetColor();
            }
        }

        private int gameTime;
        private int durationTime;

        /// <summary>
        /// Creates a new particle.
        /// </summary>
        /// <param name="assetName">Asset name.</param>
        /// <param name="effectName">Effect name.</param>
        /// <param name="particleType"><see cref="ParticleType"/>.</param>
        /// <param name="position">Position.</param>
        /// <param name="rotation">Rotation.</param>
        /// <param name="size">Size.</param>
        public ParticlePlayer(string assetName, string effectName, ParticleType particleType, Vector3 position = default, Vector3 rotation = default, float size = 1f)
        {
            AssetName = assetName;
            EffectName = effectName;
            ParticleType = particleType;

            _position = position;
            _rotation = rotation;
            _size = size;

            Request();

            GlobalParticlePlayerList.Add(this);
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
        public ParticlePlayer(string assetName, string effectName, ParticleType particleType, Entity entity, Vector3 offset = default, Vector3 rotation = default, float size = 1f) : this(assetName, effectName, particleType, offset, rotation, size)
        {
            Entity = entity;
            ToEntity = true;
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
        public ParticlePlayer(string assetName, string effectName, ParticleType particleType, Entity entity, string boneName, Vector3 offset = default, Vector3 rotation = default, float size = 1f) : this(assetName, effectName, particleType, entity, offset, rotation, size)
        {
            BoneName = boneName;
            Bone = Entity.Bones[boneName];
            ToBone = true;
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
        public ParticlePlayer(string assetName, string effectName, ParticleType particleType, Entity entity, int boneIndex, Vector3 offset, Vector3 rotation, float size = 1f) : this(assetName, effectName, particleType, entity, offset, rotation, size)
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
            {
                Script.Yield();
            }
        }

        internal void Tick()
        {
            if (!IsPlaying)
            {
                return;
            }

            if (Duration > -1 && Game.GameTime >= durationTime)
            {
                Stop();

                return;
            }

            if (ParticleType != ParticleType.ForceLooped || Game.GameTime < gameTime)
            {
                return;
            }

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
            if (ParticleType != ParticleType.Looped)
            {
                return;
            }

            EvolutionParams[key] = value;

            if (Exists())
            {
                Function.Call(Hash.SET_PARTICLE_FX_LOOPED_EVOLUTION, Handle, key, value, true);
            }
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
        /// Spawns the particle and loops it if meant to.
        /// </summary>
        public void Play()
        {
            if (IsPlaying)
            {
                return;
            }

            Spawn();

            if (ParticleType == ParticleType.NonLooped)
            {
                return;
            }

            if (Duration > -1)
            {
                durationTime = Game.GameTime + Duration;
            }

            IsPlaying = true;
        }

        /// <summary>
        /// Stops looping the particle.
        /// </summary>
        /// <param name="instant"><see langword="true"/> particle will be instantly removed from game's world.</param>
        public void Stop(bool instant = false)
        {
            if (!IsPlaying)
            {
                return;
            }

            switch (ParticleType)
            {
                case ParticleType.Looped:
                    if (instant)
                    {
                        Function.Call(Hash.REMOVE_PARTICLE_FX, Handle, 0);
                    }
                    else
                    {
                        Function.Call(Hash.STOP_PARTICLE_FX_LOOPED, Handle, 0);
                    }
                    break;
                case ParticleType.NonLooped:
                case ParticleType.ForceLooped:
                    Entity.RemoveParticleEffects();
                    break;
            }

            IsPlaying = false;
        }

        /// <summary>
        /// Toggles play/stop state.
        /// </summary>
        /// <param name="state">State of the particle.</param>
        public void SetState(bool state)
        {
            if (state)
            {
                Play();
            }
            else
            {
                Stop();
            }
        }

        /// <summary>
        /// Checks if the looped particle exists.
        /// </summary>
        /// <returns><see langword="true"/> if particle exists; otherwise <see langword="false"/>.</returns>
        public bool Exists()
        {
            if (ParticleType != ParticleType.Looped)
            {
                return false;
            }

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

        internal void Spawn()
        {
            Request();

            Function.Call(Hash.USE_PARTICLE_FX_ASSET, AssetName);

            switch (ParticleType)
            {
                case ParticleType.NonLooped:
                case ParticleType.ForceLooped:

                    if (!ToEntity && !ToBone)
                    {
                        Function.Call(Hash.START_PARTICLE_FX_NON_LOOPED_AT_COORD, EffectName, Position.X, Position.Y, Position.Z, Rotation.X, Rotation.Y, Rotation.Z, Size, 0, 0, 0);
                    }
                    else if (ToEntity && !ToBone)
                    {
                        Function.Call(Hash.START_PARTICLE_FX_NON_LOOPED_ON_ENTITY, EffectName, Entity.Handle, Position.X, Position.Y, Position.Z, Rotation.X, Rotation.Y, Rotation.Z, Size, 0, 0, 0);
                    }
                    else
                    {
                        Vector3 position = Bone.GetRelativeOffsetPosition(Position);

                        Function.Call(Hash.START_PARTICLE_FX_NON_LOOPED_ON_ENTITY, EffectName, Entity.Handle, position.X, position.Y, position.Z, Rotation.X, Rotation.Y, Rotation.Z, Size, 0, 0, 0);
                    }

                    break;
                case ParticleType.Looped:

                    if (!ToEntity && !ToBone)
                    {
                        Handle = Function.Call<int>(Hash.START_PARTICLE_FX_LOOPED_AT_COORD, EffectName, Position.X, Position.Y, Position.Z, Rotation.X, Rotation.Y, Rotation.Z, Size, 0, 0, 0);
                    }
                    else if (ToEntity && !ToBone)
                    {
                        Handle = Function.Call<int>(Hash.START_PARTICLE_FX_LOOPED_ON_ENTITY, EffectName, Entity.Handle, Position.X, Position.Y, Position.Z, Rotation.X, Rotation.Y, Rotation.Z, Size, 0, 0, 0);
                    }
                    else
                    {
                        Handle = Function.Call<int>(Hash.START_PARTICLE_FX_LOOPED_ON_ENTITY_BONE, EffectName, Entity.Handle, Position.X, Position.Y, Position.Z, Rotation.X, Rotation.Y, Rotation.Z, Bone.Index, Size, 0, 0, 0);
                    }

                    break;
            }

            ISetColor();

            if (ParticleType != ParticleType.Looped)
            {
                return;
            }

            foreach (KeyValuePair<string, float> entry in EvolutionParams)
            {
                Function.Call(Hash.SET_PARTICLE_FX_LOOPED_EVOLUTION, Handle, entry.Key, entry.Value, true);
            }
        }

        internal void ISetColor()
        {
            if (!SetColor || !IsPlaying)
            {
                return;
            }

            if (ParticleType == ParticleType.Looped && Exists())
            {
                Function.Call(Hash.SET_​PARTICLE_​FX_​LOOPED_​COLOUR, Handle, Color.R / 255f, Color.G / 255f, Color.B / 255f, 0);
                Function.Call(Hash.SET_​PARTICLE_​FX_​LOOPED_​ALPHA, Handle, Color.A / 255f);
            }
            else
            {
                Function.Call(Hash.SET_PARTICLE_FX_NON_LOOPED_COLOUR, Color.R / 255f, Color.G / 255f, Color.B / 255f);
                Function.Call(Hash.SET_​PARTICLE_​FX_​NON_​LOOPED_​ALPHA, Color.A / 255f);
            }
        }
    }
}
