using GTA;
using GTA.Math;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace FusionLibrary
{
    public class LightHandler
    {
        public List<Light> Lights = new List<Light>();

        private readonly Entity Entity;

        private readonly int ShadowMulti;

        public LightHandler(Entity entity, int shadowMulti)
        {
            Entity = entity;
            ShadowMulti = shadowMulti * 10;
        }

        public Light Add(string sourceBone, string directionBone, Color color, float distance, float brightness, float roundness, float radius, float fadeout)
        {
            Lights.Add(new Light(sourceBone, directionBone, color, distance, brightness, roundness, radius, fadeout));

            return Lights.Last();
        }

        public Light Add(Vector3 position, Vector3 direction, Color color, float distance, float brightness, float roundness, float radius, float fadeout)
        {
            Lights.Add(new Light(position, direction, color, distance, brightness, roundness, radius, fadeout));
            return Lights.Last();
        }

        public void Draw()
        {
            Lights.ForEach(x =>
            {
                x.Draw(Entity);
            });
        }
    }
}
