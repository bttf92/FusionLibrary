using GTA;
using GTA.Math;
using System.Collections.Generic;
using static FusionLibrary.FusionEnums;

namespace FusionLibrary
{
    /// <summary>
    /// 
    /// </summary>
    public class CustomCameraHandler
    {
        /// <summary>
        /// 
        /// </summary>
        public List<CustomCamera> Cameras { get; private set; } = new List<CustomCamera>();
        /// <summary>
        /// 
        /// </summary>
        public int CurrentCameraIndex { get; private set; } = -1;
        /// <summary>
        /// 
        /// </summary>
        public bool CycleCameras { get; set; } = false;

        private int _cycleInterval = 10000;

        private int _duration = -1;
        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="positionOffset"></param>
        /// <param name="pointAtOffset"></param>
        /// <param name="fieldOfView"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public CustomCamera Add(Entity entity, Vector3 positionOffset, Vector3 pointAtOffset, float fieldOfView, int duration = -1)
        {
            CustomCamera ret = new CustomCamera(entity, positionOffset, pointAtOffset, fieldOfView, duration);

            Cameras.Add(ret);

            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="positionOffset"></param>
        /// <param name="pointAtOffset"></param>
        /// <param name="fieldOfView"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public CustomCamera Add(Vehicle vehicle, Vector3 positionOffset, Vector3 pointAtOffset, float fieldOfView, int duration = -1)
        {
            CustomCamera ret = new CustomCamera(vehicle, positionOffset, pointAtOffset, fieldOfView, duration);

            Cameras.Add(ret);

            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="positionBone"></param>
        /// <param name="pointAtBone"></param>
        /// <param name="fieldOfView"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public CustomCamera Add(Vehicle vehicle, string positionBone, string pointAtBone, float fieldOfView, int duration = -1)
        {
            CustomCamera ret = new CustomCamera(vehicle, positionBone, pointAtBone, fieldOfView, duration);

            Cameras.Add(ret);

            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="cameraSwitchType"></param>
        /// <param name="duration"></param>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cameraSwitchType"></param>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cameraSwitchType"></param>
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

        /// <summary>
        /// 
        /// </summary>
        public void Stop()
        {
            if (CurrentCameraIndex > -1)
            {
                CurrentCamera.Stop();
                CurrentCameraIndex = -1;
            }

            _duration = -1;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Abort()
        {
            Stop();

            Cameras.ForEach(x =>
            {
                x.Abort();
            });

            World.DestroyAllCameras();
        }

        /// <summary>
        /// 
        /// </summary>
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

            if (CurrentCameraIndex > -1)
                CurrentCamera.Tick();

            if (_duration > -1 && Game.GameTime >= _duration)
                Stop();

            if (CurrentCameraIndex > -1 && !CurrentCamera.Camera.IsActive)
                Stop();
        }
    }
}
