using FusionLibrary.Extensions;
using GTA;
using GTA.Math;
using static FusionLibrary.FusionEnums;

namespace FusionLibrary
{
    /// <summary>
    /// 
    /// </summary>
    public class CustomCamera
    {
        /// <summary>
        /// <see cref="GTA.Entity"/> at which the <see cref="Camera"/> is attached.
        /// </summary>
        public Entity Entity { get; private set; }

        /// <summary>
        /// Offset relative to <see cref="Entity"/> where <see cref="Camera"/> is attached.
        /// </summary>
        public Vector3 PositionOffset { get; private set; }

        /// <summary>
        /// Offset relative to <see cref="Entity"/> at which <see cref="Camera"/> points.
        /// </summary>
        public Vector3 PointAtOffset { get; private set; }

        /// <summary>
        /// Field of view of <see cref="Camera"/>.
        /// </summary>
        public float FieldOfView { get; private set; }

        private Vector3 CurrentPositionOffset;
        private float positionSpeed;
        private Vector3 CurrentPointAtOffset;
        private float pointAtSpeed;
        private float CurrentFieldOfView;
        private float fovSpeed;

        /// <summary>
        /// Offset relative to <see cref="Entity"/>. End position of <see cref="Camera"/>.
        /// </summary>
        public Vector3 PositionEndOffset { get; private set; }

        /// <summary>
        /// Offset relative to <see cref="Entity"/>. End point-at of <see cref="Camera"/>.
        /// </summary>
        public Vector3 PointAtEndOffset { get; private set; }

        /// <summary>
        /// End value of field of view of <see cref="Camera"/>.
        /// </summary>
        public float FieldOfViewEnd { get; private set; }

        /// <summary>
        /// Wait time before start moving the <see cref="Camera"/>.
        /// </summary>
        public int Wait { get; private set; }

        /// <summary>
        /// Duration of the movement of the <see cref="Camera"/>.
        /// </summary>
        public int SwitchDuration { get; private set; }

        /// <summary>
        /// Total duration of <see cref="Camera"/> existence.
        /// </summary>
        public int Duration { get; private set; }

        /// <summary>
        /// Whether this <see cref="Camera"/> moves.
        /// </summary>
        public bool Moving { get; private set; }

        /// <summary>
        /// Underlying <see cref="GTA.Camera"/> instance.
        /// </summary>
        public Camera Camera { get; private set; }

        private int waitTime;
        private int gameTime;
        private readonly bool isVehicle;

        internal CustomCamera(Entity entity, Vector3 positionOffset, Vector3 pointAtOffset, float fieldOfView, int duration = -1)
        {
            Entity = entity;
            PositionOffset = positionOffset;
            PointAtOffset = pointAtOffset;
            FieldOfView = fieldOfView;
            Duration = duration;
        }

        internal CustomCamera(Vehicle vehicle, Vector3 positionOffset, Vector3 pointAtOffset, float fieldOfView, int duration = -1) : this((Entity)vehicle, positionOffset, pointAtOffset, fieldOfView, duration)
        {
            isVehicle = true;
            PointAtOffset = FusionUtils.DirectionToRotation(PositionOffset, PointAtOffset, 0);
        }

        internal CustomCamera(Vehicle vehicle, string positionBone, string pointAtBone, float fieldOfView, int duration = -1) : this(vehicle, vehicle.Bones[positionBone].RelativePosition, vehicle.Bones[pointAtBone].RelativePosition, fieldOfView, duration)
        {

        }

        /// <summary>
        /// Sets end movement properties.
        /// </summary>
        /// <param name="positionOffset"></param>
        /// <param name="pointAtOffset"></param>
        /// <param name="fieldOfView"></param>
        /// <param name="wait"></param>
        /// <param name="switchDuration"></param>
        public void SetEnd(Vector3 positionOffset, Vector3 pointAtOffset, float fieldOfView, int wait, int switchDuration)
        {
            PositionEndOffset = positionOffset;

            if (isVehicle)
            {
                PointAtEndOffset = FusionUtils.DirectionToRotation(positionOffset, pointAtOffset, 0);
            }
            else
            {
                PointAtEndOffset = pointAtOffset;
            }

            FieldOfViewEnd = fieldOfView;
            Wait = wait;
            SwitchDuration = switchDuration;

            Moving = true;
        }

        internal void Show(ref CustomCamera OldCamera, CameraSwitchType cameraSwitchType = CameraSwitchType.Instant)
        {
            if (Camera == null || Camera.Exists() == false)
            {
                Camera = World.CreateCamera(Entity.Position, Entity.Rotation, FieldOfView);

                if (!isVehicle)
                {
                    Camera.AttachTo(Entity, PositionOffset);
                    Camera.PointAt(Entity, PointAtOffset);
                }
                else
                {
                    Camera.AttachToVehicle((Vehicle)Entity, "", PositionOffset, PointAtOffset);
                }
            }

            if (OldCamera == null || OldCamera.Camera == null || OldCamera.Camera.Exists() == false)
            {
                World.RenderingCamera = Camera;
            }
            else
            {
                Camera.IsActive = true;
                OldCamera.Camera.IsActive = false;

                if (cameraSwitchType == CameraSwitchType.Animated)
                {
                    OldCamera.Camera.InterpTo(Camera, 900, 1, 1);
                }
                else
                {
                    World.RenderingCamera = Camera;
                }
            }

            if (Duration > -1)
            {
                gameTime = Game.GameTime + Duration;
            }
            else
            {
                gameTime = -1;
            }

            if (!Moving)
            {
                return;
            }

            CurrentPositionOffset = PositionOffset;
            CurrentPointAtOffset = PointAtOffset;
            CurrentFieldOfView = FieldOfView;

            positionSpeed = 1000 * PositionOffset.DistanceTo(PositionEndOffset) / SwitchDuration;
            pointAtSpeed = 1000 * PointAtOffset.DistanceTo(PointAtEndOffset) / SwitchDuration;
            fovSpeed = 1000 * (FieldOfViewEnd - FieldOfView) / SwitchDuration;

            waitTime = Game.GameTime + Wait;
        }

        internal void Tick()
        {
            if (Camera == null || !Camera.IsActive)
            {
                return;
            }

            if (gameTime > -1 && Game.GameTime > gameTime)
            {
                Stop();
            }

            if (Game.GameTime >= waitTime && Game.GameTime <= (waitTime + SwitchDuration))
            {
                Vector3 dir = CurrentPositionOffset.GetDirectionTo(PositionEndOffset) * Game.LastFrameTime * positionSpeed;
                CurrentPositionOffset += dir;

                dir = CurrentPointAtOffset.GetDirectionTo(PointAtEndOffset) * Game.LastFrameTime * pointAtSpeed;
                CurrentPointAtOffset += dir;

                CurrentFieldOfView += Game.LastFrameTime * fovSpeed;

                if (!isVehicle)
                {
                    Camera.AttachTo(Entity, CurrentPositionOffset);
                    Camera.PointAt(Entity, CurrentPointAtOffset);
                }
                else
                {
                    Camera.AttachToVehicle((Vehicle)Entity, "", CurrentPositionOffset, CurrentPointAtOffset);
                }

                Camera.FieldOfView = CurrentFieldOfView;
            }
        }

        internal void Stop()
        {
            if (Camera == null || !Camera.IsActive)
            {
                return;
            }

            Camera.IsActive = false;

            if (!isVehicle)
            {
                Camera.AttachTo(Entity, PositionOffset);
                Camera.PointAt(Entity, PointAtOffset);
            }
            else
            {
                Camera.AttachToVehicle((Vehicle)Entity, "", PositionOffset, PointAtOffset);
            }

            Camera.FieldOfView = FieldOfView;

            World.RenderingCamera = null;
        }

        internal void Abort()
        {
            Camera?.Delete();
            Camera = null;
        }
    }
}
