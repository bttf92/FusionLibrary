﻿namespace FusionLibrary.Memory
{
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct CEntityCustomization
    {
        [FieldOffset(0x0370)] public CWheelCustomization* wheelCustomization;
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct CWheelCustomization
    {
        [FieldOffset(0x0008)] public float wheelSize;
    }
}
