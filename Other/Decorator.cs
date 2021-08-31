using FusionLibrary.Memory;
using GTA;
using GTA.Native;
using System;
using static FusionLibrary.FusionEnums;

namespace FusionLibrary
{
    public class Decorator
    {
        private const string dDoNotDelete = "FusionProp_DoNotDelete";

        private const string dInteractableEntity = "FusionProp_InteractableEntity";
        private const string dInteractableId = "FusionProp_InteractableId";

        internal static void Initialize()
        {
            Register(dDoNotDelete, DecorType.Bool);
            Register(dInteractableEntity, DecorType.Bool);
            Register(dInteractableId, DecorType.Int);
            Lock();
        }

        private Entity Entity;

        public Decorator(Entity entity)
        {
            Entity = entity;
        }

        public bool DoNotDelete
        {
            get => Exists(dDoNotDelete) && GetBool(dDoNotDelete);
            set => SetBool(dDoNotDelete, value);
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

        public bool Exists(string propertyName)
        {
            return Function.Call<bool>(Hash.DECOR_EXIST_ON, Entity, propertyName);
        }

        public bool Remove(string propertyName)
        {
            if (IsLocked.HasValue && IsLocked.Value)
                Unlock();

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

        public static bool IsRegistered(string propertyName, DecorType decorType)
        {
            return Function.Call<bool>(Hash.DECOR_IS_REGISTERED_AS_TYPE, propertyName, (int)decorType);
        }

        public static bool Register(string propertyName, DecorType decorType)
        {
            if (IsRegistered(propertyName, decorType))
                return true;

            if (IsLocked.HasValue && IsLocked.Value)
                Unlock();

            Function.Call(Hash.DECOR_REGISTER, propertyName, (int)decorType);

            return IsRegistered(propertyName, decorType);
        }

        private static IntPtr lockAddress;
        private static unsafe byte* g_bIsDecorRegisterLockedPtr;

        public static bool? IsLocked
        {
            get
            {
                unsafe
                {
                    if (lockAddress == IntPtr.Zero)
                        return null;

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
                    g_bIsDecorRegisterLockedPtr = (byte*)(lockAddress + *(int*)(lockAddress + 8) + 13);
            }
        }

        public static void Unlock()
        {
            unsafe
            {
                if (lockAddress != IntPtr.Zero)
                    *g_bIsDecorRegisterLockedPtr = 0;
            }
        }

        public static void Lock()
        {
            unsafe
            {
                if (lockAddress != IntPtr.Zero)
                    *g_bIsDecorRegisterLockedPtr = 1;
            }
        }

    }
}
