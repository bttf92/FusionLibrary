using GTA;
using GTA.Math;
using System.Collections.Generic;
using static FusionLibrary.FusionEnums;

namespace FusionLibrary
{
    public class CustomCameraHandler
    {
        public List<CustomCamera> Cameras { get; private set; } = new List<CustomCamera>();
        public int CurrentCameraIndex { get; private set; } = -1;

        public bool CycleCameras { get; set; } = false;

        private int _cycleInterval = 10000;

        private int _duration = -1;

        public int CycleInterval
        {
            get => _cycleInterval;
            set
            {
                if (CycleCameras)
                {
                    nextChange -= _cycleInterval;
                    nextChange += value;
                }

                _cycleInterval = value;
            }
        }

        private int nextChange = 0;

        public CustomCamera CurrentCamera
        {
            get
            {
                if (CurrentCameraIndex == -1)
                    return null;
                else
                    return Cameras[CurrentCameraIndex];
            }
        }

        public CustomCamera Add(Entity entity, Vector3 positionOffset, Vector3 pointAtOffset, float fieldOfView)
        {
            CustomCamera ret = new CustomCamera(entity, positionOffset, pointAtOffset, fieldOfView);

            Cameras.Add(ret);

            return ret;
        }

        public CustomCamera Add(Vehicle vehicle, Vector3 positionOffset, Vector3 pointAtOffset, float fieldOfView)
        {
            CustomCamera ret = new CustomCamera(vehicle, positionOffset, pointAtOffset, fieldOfView);

            Cameras.Add(ret);

            return ret;
        }

        public CustomCamera Add(Vehicle vehicle, string positionBone, string pointAtBone, float fieldOfView)
        {
            CustomCamera ret = new CustomCamera(vehicle, positionBone, pointAtBone, fieldOfView);

            Cameras.Add(ret);

            return ret;
        }

        public void Show(int index, CameraSwitchType cameraSwitchType = CameraSwitchType.Instant, int duration = -1)
        {
            if (Cameras.Count == 0)
                return;

            if (index == -1 && CurrentCameraIndex > -1)
            {
                Stop();
                return;
            }

            if (index > Cameras.Count - 1 || index < 0)
                return;

            if (duration != -1)
                _duration = Game.GameTime + duration;

            CustomCamera customCamera = CurrentCamera;

            Cameras[index].Show(ref customCamera, cameraSwitchType);
            CurrentCameraIndex = index;
        }

        public void ShowNext(CameraSwitchType cameraSwitchType = CameraSwitchType.Instant)
        {
            if (Cameras.Count == 0)
                return;

            if (CurrentCameraIndex == Cameras.Count - 1)
                Abort();
            else
            {
                CustomCamera customCamera = CurrentCamera;

                Cameras[CurrentCameraIndex + 1].Show(ref customCamera, cameraSwitchType);
                CurrentCameraIndex += 1;
            }
        }

        public void ShowPrevious(CameraSwitchType cameraSwitchType = CameraSwitchType.Instant)
        {
            if (Cameras.Count == 0)
                return;

            if (CurrentCameraIndex <= 0)
                Abort();
            else
            {
                CustomCamera customCamera = CurrentCamera;

                Cameras[CurrentCameraIndex - 1].Show(ref customCamera, cameraSwitchType);
                CurrentCameraIndex -= 1;
            }
        }

        public void Stop()
        {
            if (CurrentCameraIndex > -1)
            {
                CurrentCamera.Stop();
                CurrentCameraIndex = -1;
            }

            _duration = -1;
        }

        public void Abort()
        {
            Stop();

            Cameras.ForEach(x =>
            {
                x.Abort();
            });

            World.DestroyAllCameras();
        }

        public void Tick()
        {
            if (CycleCameras)
            {
                if (nextChange < Game.GameTime)
                {
                    ShowNext();
                    nextChange = Game.GameTime + CycleInterval;
                }
            }

            if (_duration > -1 && Game.GameTime >= _duration)
                Stop();

            if (CurrentCameraIndex > -1 && !CurrentCamera.Camera.IsActive)
                Stop();
        }
    }
}
