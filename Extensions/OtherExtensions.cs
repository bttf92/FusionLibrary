using GTA;
using GTA.Math;
using GTA.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using static FusionLibrary.FusionEnums;

namespace FusionLibrary.Extensions
{
    public static class OtherExtensions
    {
        public static bool NotNullAndExists(this Camera camera)
        {
            return camera != null && camera.Exists();
        }

        public static bool IsCameraValid(this Camera camera)
        {
            return camera.NotNullAndExists() && camera.Position != Vector3.Zero;
        }

        public static void AttachTo(this Camera camera, Vehicle vehicle, string bone, Vector3 position, Vector3 rotation)
        {
            Function.Call(Hash._ATTACH_CAM_TO_VEHICLE_BONE, camera, vehicle, vehicle.Bones[bone].Index, true, rotation.X, rotation.Y, rotation.Z, position.X, position.Y, position.Z, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static (float width, float height, float depth) GetSize(this Model model)
        {
            return (model.GetSize(Coordinate.Y), model.GetSize(Coordinate.Z), model.GetSize(Coordinate.Z));
        }

        /// <summary>
        /// Returns <see cref="Coordinate"/> dimension of <paramref name="model"/>.
        /// </summary>
        /// <param name="model">Instance of a <see cref="Model"/></param>
        /// <param name="coordinate">Axis.</param>
        /// <returns>Dimension.</returns>
        public static float GetSize(this Model model, Coordinate coordinate)
        {
            (Vector3 rearBottomLeft, Vector3 frontTopRight) = model.Dimensions;

            return Math.Abs(frontTopRight[(int)coordinate] - rearBottomLeft[(int)coordinate]);
        }

        /// <summary>
        /// Splits a string in parts of length <paramref name="partLength"/>.
        /// </summary>
        /// <param name="s">String that needs to be splitted.</param>
        /// <param name="partLength">Length of the part.</param>
        /// <returns><see cref="IEnumerable{String}"/> of splitted parts.</returns>
        public static IEnumerable<string> SplitInParts(this string s, int partLength)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));
            if (partLength <= 0)
                throw new ArgumentException("Part length has to be positive.", nameof(partLength));

            for (int i = 0; i < s.Length; i += partLength)
                yield return s.Substring(i, Math.Min(partLength, s.Length - i));
        }

        public static T SelectRandomElement<T>(this IEnumerable<T> sequence) where T : class
        {
            if (sequence.Count() != 0)
                return sequence.ElementAt(FusionUtils.Random.Next(0, sequence.Count()));

            return null;
        }

        public static T SelectRandomElement<T>(this IDictionary<int, T> dictionnary) where T : class
        {
            if (dictionnary.Count != 0)
                return dictionnary[FusionUtils.Random.Next(0, dictionnary.Count)];

            return null;
        }
    }
}
