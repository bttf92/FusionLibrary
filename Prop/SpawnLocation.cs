using FusionLibrary.Extensions;
using GTA;
using GTA.Math;
using GTA.Native;
using System.Collections.Generic;
using static FusionLibrary.Enums;

namespace FusionLibrary
{
    public class SpawnLocationHandler 
    {
        public List<SpawnLocation> Locations { get; } = new List<SpawnLocation>();

        public void Add(Vector3 position, bool direction)
        {
            Locations.Add(new SpawnLocation(this, position, direction));
        }

        public void Add(Vector3 position, Vector3 cameraPos, Vector3 cameraDir, bool direction = true)
        {
            Locations.Add(new SpawnLocation(this, position, cameraPos, cameraDir, direction));
        }
    }

    public class SpawnLocation
    {
        public Vector3 Position { get; }
        public Vector3 CameraPos { get; } = Vector3.Zero;
        public Vector3 CameraDir { get; } = Vector3.Zero;
        public bool Direction { get; }
        public string Name { get; }

        private static Camera LocationCamera;
        private SpawnLocationHandler SpawnLocationHandler;

        public SpawnLocation(SpawnLocationHandler spawnLocationHandler, Vector3 position, bool direction)
        {
            SpawnLocationHandler = spawnLocationHandler;

            Position = position;
            Direction = direction;
            Name = World.GetZoneLocalizedName(position);
        }

        public SpawnLocation(SpawnLocationHandler spawnLocationHandler, Vector3 position, Vector3 cameraPos, Vector3 cameraDir, bool direction = true)
        {
            SpawnLocationHandler = spawnLocationHandler;

            Position = position;
            Direction = direction;
            Name = World.GetZoneLocalizedName(position);
            CameraPos = cameraPos;
            CameraDir = cameraDir;
        }

        public override string ToString()
        {
            return SpawnLocationHandler.Locations.IndexOf(this).ToString();
        }

        public void ShowLocation()
        {
            Function.Call(Hash.NEW_LOAD_SCENE_START_SPHERE, Position.X, Position.Y, Position.Z, 100, 0);

            LocationCamera?.Delete();

            if (CameraPos != Vector3.Zero && CameraDir != Vector3.Zero)
            {
                LocationCamera = World.CreateCamera(CameraPos, Vector3.Zero, 75);
                LocationCamera.Direction = CameraDir;
            }
            else
            {
                LocationCamera = World.CreateCamera(Position.GetSingleOffset(Coordinate.Z, 10).GetSingleOffset(Coordinate.Y, 10), Vector3.Zero, 75);
                LocationCamera.PointAt(Position);
            }

            World.RenderingCamera = LocationCamera;

            Function.Call(Hash.LOCK_MINIMAP_POSITION, Position.X, Position.Y);
        }

        public static void ResetCamera()
        {
            LocationCamera?.Delete();

            Function.Call(Hash.UNLOCK_MINIMAP_POSITION);

            World.DestroyAllCameras();
            World.RenderingCamera = null;
        }
    }
}
