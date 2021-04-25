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
            Function.Call((Hash)0x464D8E1427156FE4, garageDoor);
        }

        public static void RemoveDoor(Hash door)
        {
            Function.Call((Hash)0x464D8E1427156FE4, door);
        }

        public static bool IsDoorRegistered(GarageDoor garageDoor)
        {
            return Function.Call<bool>((Hash)0xC153C43EA202C8C1, garageDoor);
        }

        public static bool IsDoorRegistered(Hash door)
        {
            return Function.Call<bool>((Hash)0xC153C43EA202C8C1, door);
        }

        public static bool IsDoorClosed(GarageDoor door)
        {
            return Function.Call<bool>((Hash)0xC531EE8A1145A149, door);
        }

        public static bool IsDoorClosed(Hash door)
        {
            return Function.Call<bool>((Hash)0xC531EE8A1145A149, door);
        }

        public static float GetDoorOpenRatio(Hash door)
        {
            return Function.Call<float>((Hash)0x65499865FCA6E5EC, door);
        }

        public static float GetDoorOpenRatio(GarageDoor garageDoor)
        {
            return Function.Call<float>((Hash)0x65499865FCA6E5EC, garageDoor);
        }

        public static DoorState GetDoorState(GarageDoor garageDoor)
        {
            return Function.Call<DoorState>((Hash)0x160AA1B32F6139B8, garageDoor);
        }

        public static DoorState GetDoorState(Hash door)
        {
            return Function.Call<DoorState>((Hash)0x160AA1B32F6139B8, door);
        }

        public static DoorState GetDoorPendingState(GarageDoor garageDoor)
        {
            return Function.Call<DoorState>((Hash)0x4BC2854478F3A749, garageDoor);
        }

        public static DoorState GetDoorPendingState(Hash door)
        {
            return Function.Call<DoorState>((Hash)0x4BC2854478F3A749, door);
        }

        public static void SetDoorState(GarageDoor garageDoor, DoorState doorState, bool requestDoor = false, bool forceUpdate = false)
        {
            Function.Call((Hash)0x6BAB9442830C7F53, garageDoor, doorState, requestDoor, forceUpdate);
        }

        public static void SetDoorState(Hash door, DoorState doorState, bool requestDoor = false, bool forceUpdate = false)
        {
            Function.Call((Hash)0x6BAB9442830C7F53, door, doorState, requestDoor, forceUpdate);
        }

        public static unsafe Hash FindDoor(Vector3 position, Hash model)
        {
            int ret;

            Function.Call<bool>((Hash)0x589F80B325CC82C5, position.X, position.Y, position.Z, model, &ret);

            return (Hash)ret;
        }
    }
}
