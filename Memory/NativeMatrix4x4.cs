﻿namespace FusionLibrary.Memory
{
    using GTA.Math;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct NativeMatrix4x4
    {
        public float M11;
        public float M12;
        public float M13;
        public float M14;

        public float M21;
        public float M22;
        public float M23;
        public float M24;

        public float M31;
        public float M32;
        public float M33;
        public float M34;

        public float M41;
        public float M42;
        public float M43;
        public float M44;

        public static implicit operator NativeMatrix4x4(Matrix matrix)
        {
            return new NativeMatrix4x4()
            {
                M11 = matrix.M11,
                M12 = matrix.M12,
                M13 = matrix.M13,
                M14 = matrix.M14,

                M21 = matrix.M21,
                M22 = matrix.M22,
                M23 = matrix.M23,
                M24 = matrix.M24,

                M31 = matrix.M31,
                M32 = matrix.M32,
                M33 = matrix.M33,
                M34 = matrix.M34,

                M41 = matrix.M41,
                M42 = matrix.M42,
                M43 = matrix.M43,
                M44 = matrix.M44,
            };
        }

        public static implicit operator Matrix(NativeMatrix4x4 matrix)
        {
            return new Matrix()
            {
                M11 = matrix.M11,
                M12 = matrix.M12,
                M13 = matrix.M13,
                M14 = matrix.M14,

                M21 = matrix.M21,
                M22 = matrix.M22,
                M23 = matrix.M23,
                M24 = matrix.M24,

                M31 = matrix.M31,
                M32 = matrix.M32,
                M33 = matrix.M33,
                M34 = matrix.M34,

                M41 = matrix.M41,
                M42 = matrix.M42,
                M43 = matrix.M43,
                M44 = matrix.M44,
            };
        }
    }
}
