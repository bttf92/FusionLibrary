using FusionLibrary.Extensions;
using GTA;
using GTA.Math;
using GTA.Native;
using System.Drawing;

namespace FusionLibrary
{
    public class Light
    {
        public Light(Vector3 position, Vector3 direction, Color color, float distance, float brightness, float roundness, float radius, float fadeout)
        {
            Position = position;
            Direction = direction;

            Distance = distance;
            Brightness = brightness;
            Roundness = roundness;
            Radius = radius;
            Fadeout = fadeout;

            Color = color;
        }

        public Light(string sourceBone, string directionBone, Color color, float distance, float brightness, float roundness, float radius, float fadeout)
        {
            SourceBone = sourceBone;
            DirectionBone = directionBone;

            Distance = distance;
            Brightness = brightness;
            Roundness = roundness;
            Radius = radius;
            Fadeout = fadeout;

            Color = color;

            useBones = true;
        }

        private readonly bool useBones;

        public string SourceBone { get; set; }
        public string DirectionBone { get; set; }

        public Vector3 Position { get; set; }
        public Vector3 Direction { get; set; }

        public float Distance { get; set; }
        public float Brightness { get; set; }
        public float Roundness { get; set; }
        public float Radius { get; set; }
        public float Fadeout { get; set; }
        public Color Color { get; set; }
        public bool IsEnabled { get; set; } = true;

        public void Draw(Entity Entity, float shadowId)
        {
            if (!IsEnabled)
            {
                return;
            }

            Vector3 pos;
            Vector3 dir;

            if (useBones)
            {
                pos = Entity.Bones[SourceBone].Position;

                if (DirectionBone != null)
                {
                    dir = Entity.Bones[SourceBone].Position.GetDirectionTo(Entity.Bones[DirectionBone].Position);
                }
                else
                {
                    dir = Direction;
                }
            }
            else
            {
                pos = Entity.GetOffsetPosition(Position);
                dir = Direction;
            }

            Function.Call(Hash._DRAW_SPOT_LIGHT_WITH_SHADOW, pos.X, pos.Y, pos.Z, dir.X, dir.Y, dir.Z, Color.R, Color.G, Color.B, Distance, Brightness, Roundness, Radius, Fadeout, shadowId);
        }
    }
}
