﻿using FusionLibrary.Memory;

namespace FusionLibrary
{
    // Credits to Dot. for this!!
    public static unsafe class RainPuddleEditor
    {
        private static readonly float* pPuddleLevel;

        static RainPuddleEditor()
        {
            byte* address = MemoryFunctions.FindPattern("\x75\x08\xF3\x0F\x10\x35\x00\x00\x00\x00\xF3\x0F\x10\x05\x00\x00\x00\x00", "xxxxxx????xxxx????") + 2;
            pPuddleLevel = (float*)(*(int*)(address + 4) + address + 8);

        }

        public static float Level
        {
            set => *pPuddleLevel = value;

            get => *pPuddleLevel;
        }
    }
}
