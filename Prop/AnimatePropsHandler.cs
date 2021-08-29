using FusionLibrary.Extensions;
using GTA;
using System.Collections.Generic;
using System.Linq;
using static FusionLibrary.FusionEnums;

namespace FusionLibrary
{
    public delegate void OnSequenceCompleted(bool isLooped);

    public class AnimatePropsHandler
    {
        internal static List<AnimatePropsHandler> GlobalAnimatePropsHandlerList = new List<AnimatePropsHandler>();

        internal static void TickAll()
        {
            for (int i = 0; i < GlobalAnimatePropsHandlerList.Count; i++)
                GlobalAnimatePropsHandlerList[i].Tick();
        }

        public event OnSequenceCompleted OnSequenceCompleted;
        public event OnAnimCompleted OnAnimCompleted;
        public List<AnimateProp> Props = new List<AnimateProp>();

        public bool SequenceSpawn { get; set; }
        public int SequenceInterval { get; set; }
        public bool IsSequenceLooped { get; set; }
        public bool IsSequencePlaying { get; private set; }
        public bool IsSequenceRandom { get; set; }

        private int nextSequenceTime;
        private int currentSequenceProp = -1;
        private List<int> playedProps = new List<int>();

        public AnimatePropsHandler()
        {
            GlobalAnimatePropsHandlerList.Add(this);
        }

        public int Count => Props.Count();

        public bool Visible
        {
            get => Props[0].Visible;
            set => Props.ForEach(x => x.Visible = value);
        }

        public bool UseFixedRot
        {
            get
            {
                return Props.TrueForAll(x => x.UseFixedRot);
            }
            set
            {
                Props.ForEach(x => x.UseFixedRot = value);
            }
        }

        public bool UsePhysicalAttach
        {
            get
            {
                return Props.TrueForAll(x => x.UsePhysicalAttach);
            }
            set
            {
                Props.ForEach(x => x.UsePhysicalAttach = value);
            }
        }

        public bool SmoothEnd
        {
            get
            {
                return Props.TrueForAll(x => x.SmoothEnd);
            }
            set
            {
                Props.ForEach(x => x.SmoothEnd = value);
            }
        }

        public bool IsSpawned => Props.TrueForAll(x => x.IsSpawned);

        public bool IsPlaying => Props.Any(x => x.IsPlaying);

        public void Add(AnimateProp animateProp)
        {
            Props.Add(animateProp);
            animateProp.OnAnimCompleted += AnimateProp_OnAnimCompleted;
        }

        internal void Tick()
        {
            if (!IsSequencePlaying || Game.GameTime < nextSequenceTime)
                return;

            if (IsSequenceRandom)
            {
                if (playedProps.Count == Count)
                {
                    if (IsSequenceLooped)
                    {
                        playedProps.Clear();

                        OnSequenceCompleted?.Invoke(true);
                    }
                    else
                    {
                        Delete();

                        OnSequenceCompleted?.Invoke(false);
                        return;
                    }
                }

                if (currentSequenceProp > -1)
                    Props[currentSequenceProp]?.Delete();

                if (playedProps.Count == 0)
                    currentSequenceProp = FusionUtils.Random.NextExcept(0, Count, currentSequenceProp);
                else
                    currentSequenceProp = FusionUtils.Random.NextExcept(0, Count, currentSequenceProp);

                Props[currentSequenceProp]?.SpawnProp();

                playedProps.Add(currentSequenceProp);

                nextSequenceTime = Game.GameTime + SequenceInterval;

                return;
            }

            if (playedProps.Count == Count)
            {
                if (IsSequenceLooped)
                {
                    Props[currentSequenceProp]?.Delete();

                    currentSequenceProp = -1;
                    playedProps.Clear();
                }
                else
                {
                    Delete();

                    OnSequenceCompleted?.Invoke(false);
                    return;
                }
            }

            if (currentSequenceProp > -1)
                Props[currentSequenceProp]?.Delete();

            currentSequenceProp++;

            Props[currentSequenceProp]?.SpawnProp();

            playedProps.Add(currentSequenceProp);

            nextSequenceTime = Game.GameTime + SequenceInterval;
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
                IsSequencePlaying = true;
                return;
            }

            Props.ForEach(x => x.Play(animationStep, instant, playInstantPreviousSteps, spawnAndRestore));
        }

        public void Stop()
        {
            IsSequencePlaying = false;
            //currentSequenceProp = -1;
            //playedProps.Clear();

            Props.ForEach(x => x.Stop());
        }

        public void TransferTo(Entity entity)
        {
            Props.ForEach(x => x.TransferTo(entity));
        }

        public void TransferTo(Entity entity, string boneName)
        {
            Props.ForEach(x => x.TransferTo(entity, boneName));
        }

        public void TransferTo(Entity entity, EntityBone entityBone)
        {
            Props.ForEach(x => x.TransferTo(entity, entityBone));
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
            currentSequenceProp = -1;
            playedProps.Clear();

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