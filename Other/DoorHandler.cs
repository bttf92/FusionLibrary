using GTA.Math;
using GTA.Native;
using static FusionLibrary.FusionEnums;

namespace FusionLibrary
{
    public static class DoorHandler
    {
        public static void RegisterDoor(Hash door, Hash model, Vector3 position, bool scriptDoor = false, bool isLocal = false)
        {
            Function.Call(Hash.ADD_DOOR_TO_SYSTEM, door, model, position.X, position.Y, position.Z, false, scriptDoor, isLocal);
        }

        public static void RegisterDoor(GarageDoor garageDoor, Hash model, Vector3 position, bool scriptDoor = false, bool isLocal = false)
        {
            Function.Call(Hash.ADD_DOOR_TO_SYSTEM, garageDoor, model, position.X, position.Y, position.Z, false, scriptDoor, isLocal);
        }

        public static void RemoveDoor(GarageDoor garageDoor)
        {
            Function.Call(Hash.REMOVE_DOOR_FROM_SYSTEM, garageDoor);
        }

        public static void RemoveDoor(Hash door)
        {
            Function.Call(Hash.REMOVE_DOOR_FROM_SYSTEM, door);
        }

        public static bool IsDoorRegistered(GarageDoor garageDoor)
        {
            return Function.Call<bool>(Hash.IS_DOOR_REGISTERED_WITH_SYSTEM, garageDoor);
        }

        public static bool IsDoorRegistered(Hash door)
        {
            return Function.Call<bool>(Hash.IS_DOOR_REGISTERED_WITH_SYSTEM, door);
        }

        public static bool IsDoorClosed(GarageDoor door)
        {
            return Function.Call<bool>(Hash.IS_DOOR_CLOSED, door);
        }

        public static bool IsDoorClosed(Hash door)
        {
            return Function.Call<bool>(Hash.IS_DOOR_CLOSED, door);
        }

        public static float GetDoorOpenRatio(Hash door)
        {
            return Function.Call<float>(Hash.DOOR_SYSTEM_GET_OPEN_RATIO, door);
        }

        public static float GetDoorOpenRatio(GarageDoor garageDoor)
        {
            return Function.Call<float>(Hash.DOOR_SYSTEM_GET_OPEN_RATIO, garageDoor);
        }

        public static DoorState GetDoorState(GarageDoor garageDoor)
        {
            return Function.Call<DoorState>(Hash.DOOR_SYSTEM_GET_DOOR_STATE, garageDoor);
        }

        public static DoorState GetDoorState(Hash door)
        {
            return Function.Call<DoorState>(Hash.DOOR_SYSTEM_GET_DOOR_STATE, door);
        }

        public static DoorState GetDoorPendingState(GarageDoor garageDoor)
        {
            return Function.Call<DoorState>(Hash.DOOR_SYSTEM_GET_DOOR_PENDING_STATE, garageDoor);
        }

        public static DoorState GetDoorPendingState(Hash door)
        {
            return Function.Call<DoorState>(Hash.DOOR_SYSTEM_GET_DOOR_PENDING_STATE, door);
        }

        public static void SetDoorState(GarageDoor garageDoor, DoorState doorState, bool requestDoor = false, bool forceUpdate = false)
        {
            Function.Call(Hash.DOOR_SYSTEM_SET_DOOR_STATE, garageDoor, doorState, requestDoor, forceUpdate);
        }

        public static void SetDoorState(Hash door, DoorState doorState, bool requestDoor = false, bool forceUpdate = false)
        {
            Function.Call(Hash.DOOR_SYSTEM_SET_DOOR_STATE, door, doorState, requestDoor, forceUpdate);
        }

        public static unsafe Hash FindDoor(Vector3 position, Hash model)
        {
            int ret;

            Function.Call<bool>(Hash.DOOR_SYSTEM_FIND_EXISTING_DOOR, position.X, position.Y, position.Z, model, &ret);

            return (Hash)ret;
        }
    }
}
