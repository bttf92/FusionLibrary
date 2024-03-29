﻿namespace FusionLibrary.Memory
{
    using GTA.Math;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct NativeVector3
    {
        public float X;
        public float Y;
        public float Z;

        public static implicit operator Vector3(NativeVector3 value)
        {
            return new Vector3(value.X, value.Y, value.Z);
        }

        public static implicit operator NativeVector3(Vector3 value)
        {
            return new NativeVector3 { X = value.X, Y = value.Y, Z = value.Z };
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct NativeVector4
    {
        public float X;
        public float Y;
        public float Z;
        public float W;

        public static implicit operator Quaternion(NativeVector4 value)
        {
            return new Quaternion(value.X, value.Y, value.Z, value.W);
        }

        public static implicit operator NativeVector4(Quaternion value)
        {
            return new NativeVector4 { X = value.X, Y = value.Y, Z = value.Z, W = value.W };
        }
    }
}
