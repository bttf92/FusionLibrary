using System.Collections.Generic;
using System.Linq;
using static FusionLibrary.Enums;

namespace FusionLibrary
{
    public class AnimatePropsHandler
    {
        public event OnAnimCompleted OnAnimCompleted;
        public List<AnimateProp> Props = new List<AnimateProp>();

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

        private void AnimateProp_OnAnimCompleted(AnimationStep animationStep)
        {
            foreach(AnimateProp prop in Props)
                if (prop[AnimationType.Offset][animationStep].Coordinates.Any(x => x.Update) || prop[AnimationType.Rotation][animationStep].Coordinates.Any(x => x.Update))
                    return;

            OnAnimCompleted?.Invoke(animationStep);
        }

        public void SpawnProp()
        {
            Props.ForEach(x => x.SpawnProp());
        }

        public void Play(bool instant = false)
        {
            Play(AnimationStep.First, instant);
        }

        public void Play(AnimationStep animationStep, bool instant = false, bool playInstantPreviousSteps = false)
        {
            Props.ForEach(x => x.Play(animationStep, instant, playInstantPreviousSteps));            
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

        public void Delete()
        {
            Props.ForEach(x => x.Delete());
        }

        public void Detach()
        {
            Props.ForEach(x => x.Detach());
        }

        public void ScatterProp(float ForceMultiplier = 1f)
        {
            Props.ForEach(x => x.ScatterProp(ForceMultiplier));
        }

        public void Dispose()
        {
            Props.ForEach(x => x.Dispose());
            Props.Clear();
        }

        public AnimateProp this[int propIndex]
        {
            get { return Props[propIndex]; }
            set { Props[propIndex] = value; }
        }
    }
}