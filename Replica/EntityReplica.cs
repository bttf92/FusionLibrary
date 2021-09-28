using GTA;
using GTA.Math;
using System;

namespace FusionLibrary
{
    [Serializable]
    public abstract class EntityReplica
    {
        public int ModelHash { get; set; }
        public Model Model
        {
            get
            {
                return new Model(ModelHash);
            }

            set
            {
                ModelHash = value.Hash;
            }
        }
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; }
        public Vector3 Velocity { get; }
        public float Heading { get; }
        public float Speed { get; }
        public bool IsVisible { get; }
        public float Health { get; }

        public EntityReplica(Entity entity)
        {
            Model = entity.Model;
            Position = entity.Position;
            Rotation = entity.Rotation;
            Velocity = entity.Velocity;
            Heading = entity.Heading;
            Speed = entity.Speed;
            IsVisible = entity.IsVisible;
            Health = entity.HealthFloat;
        }
    }
}
