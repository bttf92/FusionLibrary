﻿namespace FusionLibrary.Memory
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct CrSkeletonData
    {
        [FieldOffset(0x0020)] public CrSkeletonBoneData* bones;
        [FieldOffset(0x0028)] public NativeMatrix4x4* bonesTransformationsInverted;
        [FieldOffset(0x0030)] public NativeMatrix4x4* bonesTransformations;
        [FieldOffset(0x0038)] public ushort* bonesParentIndices;
        [FieldOffset(0x005E)] public ushort bonesCount;

        public string GetBoneNameForIndex(uint index)
        {
            if (index >= bonesCount)
            {
                return null;
            }

            return bones[index].GetName();
        }
    }

    [StructLayout(LayoutKind.Explicit, Size = 0x50)]
    public unsafe struct CrSkeletonBoneData
    {
        [FieldOffset(0x0000)] public NativeVector4 rotation;
        [FieldOffset(0x0010)] public NativeVector3 translation;

        [FieldOffset(0x0032)] public ushort parentIndex;

        [FieldOffset(0x0038)] public IntPtr namePtr;

        [FieldOffset(0x0042)] public ushort index;

        public string GetName()
        {
            return namePtr == null ? null : Marshal.PtrToStringAnsi(namePtr);
        }
    }
}
