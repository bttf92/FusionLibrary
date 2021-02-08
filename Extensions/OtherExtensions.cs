using GTA;
using GTA.Math;
using GTA.Native;

namespace FusionLibrary.Extensions
{
    public static class OtherExtensions
    {
        public static bool NotNullAndExists(this Camera camera)
        {
            return camera != null && camera.Exists();
        }

        public static bool IsCameraValid(this Camera camera)
        {
            return camera.NotNullAndExists() && camera.Position != Vector3.Zero;
        }

        public static void AttachTo(this Camera camera, Vehicle vehicle, string bone, Vector3 position, Vector3 rotation)
        {
            Function.Call(Hash._ATTACH_CAM_TO_VEHICLE_BONE, camera, vehicle, vehicle.Bones[bone].Index, true, rotation.X, rotation.Y, rotation.Z, position.X, position.Y, position.Z, true);
        }
    }
}
