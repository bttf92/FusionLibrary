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
        /// <summary>
        /// Checks if <paramref name="camera"/> is not null and <see cref="Camera.Exists"/>.
        /// </summary>
        /// <param name="camera">Instance of a <see cref="Camera"/>.</param>
        /// <returns><see langword="true"/> if <paramref name="camera"/> is not null and <see cref="Camera.Exists"/>; otherwise <see langword="false"/>.</returns>
        public static bool NotNullAndExists(this Camera camera)
        {
            return camera != null && camera.Exists();
        }

        /// <summary>
        /// Attaches <paramref name="camera"/> to <paramref name="bone"/> of <paramref name="vehicle"/>.
        /// </summary>
        /// <param name="camera">Instance of a <see cref="Camera"/>.</param>
        /// <param name="vehicle">Instance of a <see cref="Vehicle"/>.</param>
        /// <param name="bone">Bone's name.</param>
        /// <param name="offset">Offset relative to <paramref name="bone"/>.</param>
        /// <param name="rotation">Rotation of the <see cref="Camera"/>.</param>
        /// <param name="relativeRotation">Sets if rotation is relative to <paramref name="bone"/>.</param>
        /// <param name="fixedDirection">Sets if direction is relative to <paramref name="vehicle"/>.</param>
        public static void AttachToVehicle(this Camera camera, Vehicle vehicle, string bone, Vector3 offset, Vector3 rotation, bool relativeRotation = true, bool fixedDirection = true)
        {
            if (!camera.NotNullAndExists() || !vehicle.NotNullAndExists())
            {
                return;
            }

            Function.Call(Hash._ATTACH_CAM_TO_VEHICLE_BONE, camera, vehicle, vehicle.Bones[bone].Index, relativeRotation, rotation.X, rotation.Y, rotation.Z, offset.X, offset.Y, offset.Z, fixedDirection);
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
            {
                throw new ArgumentNullException(nameof(s));
            }

            if (partLength <= 0)
            {
                throw new ArgumentException("Part length has to be positive.", nameof(partLength));
            }

            for (int i = 0; i < s.Length; i += partLength)
            {
                yield return s.Substring(i, Math.Min(partLength, s.Length - i));
            }
        }

        /// <summary>
        /// Gets next value of enum.
        /// </summary>
        /// <typeparam name="T">Enum type.</typeparam>
        /// <param name="src">Enum.</param>
        /// <returns>Next value of enum.</returns>
        public static T Next<T>(this T src) where T : struct
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException(string.Format("Argument {0} is not an Enum", typeof(T).FullName));

            T[] Arr = (T[])Enum.GetValues(src.GetType());
            int j = Array.IndexOf<T>(Arr, src) + 1;
            return (Arr.Length == j) ? Arr[0] : Arr[j];
        }

        /// <summary>
        /// Returns a random element from an <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <typeparam name="T"><see cref="Type"/> of the element.</typeparam>
        /// <param name="sequence"><see cref="IEnumerable{T}"/></param>
        /// <returns>Random <typeparamref name="T"/> element from <paramref name="sequence"/>.</returns>
        public static T SelectRandomElement<T>(this IEnumerable<T> sequence) where T : class
        {
            if (sequence.Count() != 0)
            {
                return sequence.ElementAt(FusionUtils.Random.Next(0, sequence.Count()));
            }

            return null;
        }

        /// <summary>
        /// Returns a random element from <see cref="IDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <typeparam name="R"><see cref="Type"/> of key of <paramref name="dictionary"/>.</typeparam>
        /// <typeparam name="T"><see cref="Type"/> of value of <paramref name="dictionary"/>.</typeparam>
        /// <param name="dictionary">Instance of a <see cref="IDictionary{TKey, TValue}"/>.</param>
        /// <returns></returns>
        public static T SelectRandomElement<R, T>(this IDictionary<R, T> dictionary) where T : class
        {
            if (dictionary.Count != 0)
            {
                return dictionary.ElementAt(FusionUtils.Random.Next(0, dictionary.Count)).Value;
            }

            return null;
        }

        /// <summary>
        /// Checks if <paramref name="obj"/> implements interface of <typeparamref name="T"/> type.
        /// </summary>
        /// <typeparam name="T">Type of interface.</typeparam>
        /// <param name="obj">Any object.</param>
        /// <returns><see langword="true"/> if <paramref name="obj"/> impelments <typeparamref name="T"/> interface.</returns>
        public static bool Implements<T>(this object obj)
        {
            return obj.GetType().GetInterfaces().Contains(typeof(T));
        }

        /// <summary>
        /// Returns a list with <paramref name="count"/> random elements from <paramref name="sequence"/>.
        /// </summary>
        /// <typeparam name="T"><see cref="Type"/> of the element.</typeparam>
        /// <param name="sequence"><see cref="IEnumerable{T}"/></param>
        /// <param name="count">Number of random elements.</param>
        /// <returns>List of random elements.</returns>
        public static List<T> SelectRandomElements<T>(this IEnumerable<T> sequence, int count) where T : class
        {
            List<T> ret = new List<T>();

            if (count > sequence.Count())
                return sequence.ToList();

            if (count < 1)
                return null;

            while(ret.Count < count)
            {
                var select = sequence.SelectRandomElement();

                if (!ret.Contains(select))
                    ret.Add(select);
            }

            return ret;
        }
    }
}
