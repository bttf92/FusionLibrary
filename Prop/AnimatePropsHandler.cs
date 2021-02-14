using GTA;
using System.Collections.Generic;
using System.Linq;
using static FusionLibrary.Enums;

namespace FusionLibrary
{
    public delegate void OnSequenceCompleted(bool isLooped);

    public class AnimatePropsHandler
    {
        internal static List<AnimatePropsHandler> GlobalAnimatePropsHandlerList = new List<AnimatePropsHandler>();

        internal static void ProcessAll()
        {
            GlobalAnimatePropsHandlerList.ForEach(x => x.Process());
        }

        public event OnSequenceCompleted OnSequenceCompleted;
        public event OnAnimCompleted OnAnimCompleted;
        public List<AnimateProp> Props = new List<AnimateProp>();

        public bool SequenceSpawn { get; set; }
        public int SequenceInterval { get; set; }
        public bool IsSequenceLooped { get; set; }
        public bool IsSequencePlaying { get; private set; }

        private int nextSequenceTime;
        private int nextSequenceProp;

        public AnimatePropsHandler()
        {
            GlobalAnimatePropsHandlerList.Add(this);
        }

        public bool Visible
        {
            get => Props[0].Visible;
            set => Props.ForEach(x => x.Visible = value);
        }

        public bool IsSpawned => Props.TrueForAll(x => x.IsSpawned);

        public bool IsPlaying => Props.Any(x => x.IsPlaying);

        public void Add(AnimateProp animateProp)
        {
            Props.Add(animateProp);
            animateProp.OnAnimCompleted += AnimateProp_OnAnimCompleted;
        }

        internal void Process()
        {
            if (!IsSequencePlaying || Game.GameTime < nextSequenceTime)
                return;

            Play();
        }

        private void AnimateProp_OnAnimCompleted(AnimationStep animationStep)
        {
            foreach (AnimateProp prop in Props)
                if (prop[AnimationType.Offset][animationStep].Coordinates.Any(x => x.Update) || prop[AnimationType.Rotation][animationStep].Coordinates.Any(x => x.Update))
                    return;

            OnAnimCompleted?.Invoke(animationStep);
        }

        public void SaveAnimation()
        {
            Props.ForEach(x => x.SaveAnimation());
        }

        public void RestoreAnimation()
        {
            Props.ForEach(x => x.RestoreAnimation());
        }

        public void SpawnProp()
        {
            Props.ForEach(x => x.SpawnProp());
        }

        public void Play(bool instant = false, bool spawnAndRestore = false)
        {
            Play(AnimationStep.First, instant, spawnAndRestore);
        }

        public void Play(AnimationStep animationStep, bool instant = false, bool playInstantPreviousSteps = false, bool spawnAndRestore = false)
        {
            if (SequenceSpawn)
            {
                if (nextSequenceProp == Props.Count - 1)
                {
                    if (IsSequenceLooped)
                    {
                        Props[nextSequenceProp]?.Delete();
                        nextSequenceProp = 0;

                        OnSequenceCompleted?.Invoke(true);
                    }
                    else
                    {
                        Delete();

                        OnSequenceCompleted?.Invoke(false);
                        return;
                    }
                }

                IsSequencePlaying = true;

                Props[nextSequenceProp + 1]?.SpawnProp();
                Props[nextSequenceProp]?.Delete();

                nextSequenceProp++;

                nextSequenceTime = Game.GameTime + SequenceInterval;
            }
            else
                Props.ForEach(x => x.Play(animationStep, instant, playInstantPreviousSteps, spawnAndRestore));
        }

        public void Stop()
        {
            IsSequencePlaying = false;
            nextSequenceProp = 0;

            Props.ForEach(x => x.Stop());
        }

        public void setOffset(Coordinate coordinate, float value, bool currentOffset = false)
        {
            Props.ForEach(x => x.setOffset(coordinate, value, currentOffset));
        }

        public void setRotation(Coordinate coordinate, float value, bool currentRotation = false)
        {
            Props.ForEach(x => x.setRotation(coordinate, value, currentRotation));
        }

        public void setCoordinateAt(bool maximum, AnimationType animationType, AnimationStep animationStep, Coordinate coordinate)
        {
            Props.ForEach(x => x.setCoordinateAt(maximum, animationType, animationStep, coordinate));
        }

        public void setInstantAnimationStep(AnimationStep animationStep)
        {
            Props.ForEach(x => x.setInstantAnimationStep(animationStep));
        }

        public void Delete(bool keepProp = false)
        {
            IsSequencePlaying = false;
            nextSequenceProp = 0;

            Props.ForEach(x => x.Delete(keepProp));
        }

        public void Detach()
        {
            Props.ForEach(x => x.Detach());
        }

        public void ScatterProp(float ForceMultiplier = 1f)
        {
            Props.ForEach(x => x.ScatterProp(ForceMultiplier));
        }

        public void Dispose(bool keepProp = false)
        {
            Props.ForEach(x => x.Dispose(keepProp));
            Props.Clear();

            GlobalAnimatePropsHandlerList.Remove(this);
        }

        public AnimateProp this[int propIndex]
        {
            get => Props[propIndex];
            set => Props[propIndex] = value;
        }
    }
}