using GTA;
using GTA.Math;
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
            if (!Props.Any(x => x.IsPlaying))
                OnAnimCompleted?.Invoke(animationStep);
        }

        public void SpawnProp()
        {
            Props.ForEach(x => x.SpawnProp());
        }

        public void Play()
        {
            Play(AnimationStep.First);
        }

        public void Play(AnimationStep animationStep)
        {
            Props.ForEach(x => x.Play(animationStep));            
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