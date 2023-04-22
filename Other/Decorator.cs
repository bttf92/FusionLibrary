using FusionLibrary.Memory;
using GTA;
using GTA.Math;
using GTA.Native;
using System;
using static FusionLibrary.FusionEnums;

namespace FusionLibrary
{
    public class Decorator
    {
        private const string dDoNotDelete = "FusionProp_DoNotDelete";
        private const string dDrivenByPlayer = "FusionProp_DrivenByPlayer";
        private const string dRemoveFromUsed = "FusionProp_RemoveFromUsed";
        private const string dModelSwapped = "FusionProp_ModelSwapped";
        private const string dIgnoreForSwap = "FusionProp_IgnoreForSwap";

        private const string dInteractableEntity = "FusionProp_InteractableEntity";
        private const string dInteractableId = "FusionProp_InteractableId";

        private const string dGrip = "FusionProp_Grip";

        private const string dTorque = "FusionProp_Torque";

        internal static void Initialize()
        {
            Register(dDoNotDelete, DecorType.Bool);
            Register(dDrivenByPlayer, DecorType.Bool);
            Register(dRemoveFromUsed, DecorType.Bool);
            Register(dModelSwapped, DecorType.Bool);
            Register(dIgnoreForSwap, DecorType.Bool);
            Register(dInteractableEntity, DecorType.Bool);
            Register(dInteractableId, DecorType.Int);
            Register(dGrip, DecorType.Float);
            Register(dTorque, DecorType.Float);
            Lock();
        }

        private readonly Entity Entity;

        public Decorator(Entity entity)
        {
            Entity = entity;
        }

        public bool DoNotDelete
        {
            get => Exists(dDoNotDelete) && GetBool(dDoNotDelete);

            set => SetBool(dDoNotDelete, value);
        }

        public bool DrivenByPlayer
        {
            get => Exists(dDrivenByPlayer) && GetBool(dDrivenByPlayer);

            set => SetBool(dDrivenByPlayer, value);
        }

        public bool RemoveFromUsed
        {
            get => Exists(dRemoveFromUsed) && GetBool(dRemoveFromUsed);

            set => SetBool(dRemoveFromUsed, value);
        }

        public bool ModelSwapped
        {
            get => Exists(dModelSwapped) && GetBool(dModelSwapped);

            set => SetBool(dModelSwapped, value);
        }

        public bool IgnoreForSwap
        {
            get => Exists(dIgnoreForSwap) && GetBool(dIgnoreForSwap);

            set => SetBool(dIgnoreForSwap, value);
        }

        public bool InteractableEntity
        {
            get => Exists(dInteractableEntity) && GetBool(dInteractableEntity);

            set => SetBool(dInteractableEntity, value);
        }

        public int InteractableId
        {
            get => GetInt(dInteractableId);

            set => SetInt(dInteractableId, value);
        }

        public float Grip
        {
            get => GetFloat(dGrip);

            set => SetFloat(dGrip, value);
        }

        public float Torque
        {
            get => GetFloat(dTorque);

            set => SetFloat(dTorque, value);
        }

        public bool Exists(string propertyName)
        {
            return Function.Call<bool>(Hash.DECOR_EXIST_ON, Entity, propertyName);
        }

        public bool Remove(string propertyName)
        {
            if (IsLocked.HasValue && IsLocked.Value)
            {
                Unlock();
            }

            return Function.Call<bool>(Hash.DECOR_REMOVE, Entity, propertyName);
        }

        public bool SetTime(string propertyName, int timeStamp)
        {
            return Function.Call<bool>(Hash.DECOR_SET_TIME, Entity, propertyName, timeStamp);
        }

        public bool SetInt(string propertyName, int value)
        {
            return Function.Call<bool>(Hash.DECOR_SET_INT, Entity, propertyName, value);
        }

        public int GetInt(string propertyName)
        {
            return Function.Call<int>(Hash.DECOR_GET_INT, Entity, propertyName);
        }

        public bool SetFloat(string propertyName, float value)
        {
            return Function.Call<bool>(Hash.DECOR_SET_FLOAT, Entity, propertyName, value);
        }

        public float GetFloat(string propertyName)
        {
            return Function.Call<float>(Hash.DECOR_GET_FLOAT, Entity, propertyName);
        }

        public bool SetBool(string propertyName, bool value)
        {
            return Function.Call<bool>(Hash.DECOR_SET_BOOL, Entity, propertyName, value);
        }

        public bool GetBool(string propertyName)
        {
            return Function.Call<bool>(Hash.DECOR_GET_BOOL, Entity, propertyName);
        }

        public bool SetVector3(string propertyName, Vector3 value)
        {
            bool ret = false;

            for (int i = 0; i < 3; i++)
            {
                ret = SetFloat(propertyName + i.ToString(), value[i]);

                if (!ret)
                    break;
            }

            return ret;
        }

        public Vector3 GetVector3(string propertyName)
        {
            Vector3 ret = new Vector3();

            for (int i = 0; i < 3; i++)
                ret[i] = GetFloat(propertyName + i.ToString());

            return ret;
        }

        public unsafe bool SetDateTime(string propertyName, DateTime value)
        {
            bool ret = false;

            long ticks = value.Ticks;

            ulong ticksBytes = *(ulong*)&ticks;
            uint ticksLow = (uint)ticksBytes & 0xFFFFFFFF;
            uint ticksHigh = (uint)(ticksBytes >> 32) & 0xFFFFFFFF;

            ret = SetInt(propertyName + "_LOW", (int)ticksLow);
            if (!ret)
                return ret;

            ret = SetInt(propertyName + "_HIGH", (int)ticksHigh);

            return ret;
        }

        public unsafe DateTime GetDateTime(string propertyName)
        {
            int ticksLowSigned = GetInt(propertyName + "_LOW");
            int ticksHighSigned = GetInt(propertyName + "_HIGH");

            uint ticksLow = *(uint*)&ticksLowSigned;
            uint ticksHigh = *(uint*)&ticksHighSigned;

            ulong ticksBytes = ticksLow | ((ulong)ticksHigh) << 32;

            long ticks = *(long*)&ticksBytes;

            return new DateTime(ticks);
        }

        public static bool IsRegistered(string propertyName, DecorType decorType)
        {
            if (decorType == DecorType.Vector3)
                return Function.Call<bool>(Hash.DECOR_IS_REGISTERED_AS_TYPE, propertyName, 1);
            else if (decorType == DecorType.DateTime)
                return Function.Call<bool>(Hash.DECOR_IS_REGISTERED_AS_TYPE, propertyName, 3);

            return Function.Call<bool>(Hash.DECOR_IS_REGISTERED_AS_TYPE, propertyName, (int)decorType);
        }

        public static bool Register(string propertyName, DecorType decorType)
        {
            if (IsRegistered(propertyName, decorType))
            {
                return true;
            }

            if (IsLocked.HasValue && IsLocked.Value)
            {
                Unlock();
            }

            if (decorType == DecorType.Vector3)
            {
                for (int i = 0; i < 3; i++)
                    Register(propertyName + i.ToString(), DecorType.Float);

                Register(propertyName, DecorType.Float);
            }
            else if (decorType == DecorType.DateTime)
            {
                Register(propertyName + "_LOW", DecorType.Int);
                Register(propertyName + "_HIGH", DecorType.Int);
                Register(propertyName, DecorType.Int);
            }
            else
            {
                Function.Call(Hash.DECOR_REGISTER, propertyName, (int)decorType);
            }

            return IsRegistered(propertyName, decorType);
        }

        private static readonly IntPtr lockAddress;
        private static readonly unsafe byte* g_bIsDecorRegisterLockedPtr;

        public static bool? IsLocked
        {
            get
            {
                unsafe
                {
                    if (lockAddress == IntPtr.Zero)
                    {
                        return null;
                    }

                    return Convert.ToBoolean(*g_bIsDecorRegisterLockedPtr);
                }
            }
        }

        public static bool VehicleExistsWith(string propertyName)
        {
            return Function.Call<bool>(Hash.DOES_VEHICLE_EXIST_WITH_DECORATOR, propertyName);
        }

        static Decorator()
        {
            unsafe
            {
                lockAddress = (IntPtr)MemoryFunctions.FindPattern("\x40\x53\x48\x83\xEC\x20\x80\x3D\x00\x00\x00\x00\x00\x8B\xDA\x75\x29", "xxxxxxxx????xxxxx");

                if (lockAddress != IntPtr.Zero)
                {
                    g_bIsDecorRegisterLockedPtr = (byte*)(lockAddress + *(int*)(lockAddress + 8) + 13);
                }
            }
        }

        public static void Unlock()
        {
            unsafe
            {
                if (lockAddress != IntPtr.Zero)
                {
                    *g_bIsDecorRegisterLockedPtr = 0;
                }
            }
        }

        public static void Lock()
        {
            unsafe
            {
                if (lockAddress != IntPtr.Zero)
                {
                    *g_bIsDecorRegisterLockedPtr = 1;
                }
            }
        }

    }
}
