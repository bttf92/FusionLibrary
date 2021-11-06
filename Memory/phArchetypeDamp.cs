namespace FusionLibrary.Memory
{
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct phArchetypeDamp
    {
        [FieldOffset(0x0020)] public phBoundComposite* bounds;
        [FieldOffset(0x0158)] public CEntitySkeleton* skeleton;
    }
}
