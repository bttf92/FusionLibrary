using System;
using System.Collections.Generic;
using static FusionLibrary.Enums;

namespace FusionLibrary
{
    public class Animation
    {
        public List<AnimationSettings> AnimationSettings { get; } = new List<AnimationSettings>();

        public Animation()
        {
            foreach (AnimationType animationType in Enum.GetValues(typeof(AnimationType)))
                AnimationSettings.Add(new AnimationSettings(animationType));
        }

        public AnimationSettings this[AnimationType animationType] => AnimationSettings[(int)animationType];

        public AnimationSettings this[int animationType] => AnimationSettings[animationType];
    }

    public class AnimationSettings
    {
        public List<AnimationStepSettings> AnimationStepSettings { get; } = new List<AnimationStepSettings>();

        public AnimationSettings(AnimationType animationType)
        {
            foreach (AnimationStep animationStep in Enum.GetValues(typeof(AnimationStep)))
                AnimationStepSettings.Add(new AnimationStepSettings(animationStep, animationType));
        }

        public AnimationStepSettings this[AnimationStep animationStep] => AnimationStepSettings[(int)animationStep];

        public AnimationStepSettings this[int animationStep] => AnimationStepSettings[animationStep];
    }

    public class AnimationStepSettings
    {
        public List<CoordinateSetting> Coordinates { get; } = new List<CoordinateSetting>();
        public AnimationStep AnimationStep { get; }
        public AnimationType Type { get; }

        public AnimationStepSettings(AnimationStep animationStep, AnimationType type)
        {
            AnimationStep = animationStep;
            Type = type;

            foreach (Coordinate coordinate in Enum.GetValues(typeof(Coordinate)))
                Coordinates.Add(new CoordinateSetting(coordinate, type, animationStep));
        }

        public void setAllUpdate(bool value)
        {
            Coordinates.ForEach(x =>
            {
                if (x.IsSetted)
                    x.Update = value;
            });
        }

        public void setAllMaximum(float value)
        {
            Coordinates.ForEach(x =>
            {
                if (x.IsSetted)
                    x.Maximum = value;
            });
        }

        public void setAllMinimum(float value)
        {
            Coordinates.ForEach(x =>
            {
                if (x.IsSetted)
                    x.Minimum = value;
            });
        }

        public void setAllStep(float value)
        {
            Coordinates.ForEach(x =>
            {
                if (x.IsSetted)
                    x.Step = value;
            });
        }

        public void setAllStepRatio(float value)
        {
            Coordinates.ForEach(x =>
            {
                if (x.IsSetted)
                    x.StepRatio = value;
            });
        }

        public void setAllIncreasing(bool value)
        {
            Coordinates.ForEach(x =>
            {
                if (x.IsSetted)
                    x.IsIncreasing = value;
            });
        }

        public void setAllStop(bool value)
        {
            Coordinates.ForEach(x =>
            {
                if (x.IsSetted)
                    x.Stop = value;
            });
        }

        public void setAllMaxMinRatio(float value)
        {
            Coordinates.ForEach(x =>
            {
                if (x.IsSetted)
                    x.MaxMinRatio = value;
            });
        }

        public CoordinateSetting this[Coordinate coordinate] => Coordinates[(int)coordinate];

        public CoordinateSetting this[int coordinate] => Coordinates[coordinate];
    }

    public class CoordinateSetting
    {
        public bool IsSetted { get; private set; }
        public Coordinate Coordinate { get; }
        public AnimationType Type { get; }
        public AnimationStep AnimationStep { get; }

        public bool Update = false;
        public bool IsIncreasing = true;
        public float Minimum = 0;
        public float Maximum = 0;
        public float MaxMinRatio = 1;
        public float Step = 0;
        public float StepRatio = 1;
        public bool Stop = false;
        public bool DoNotInvert = false;
        public CoordinateSetting(Coordinate coordinate, AnimationType type, AnimationStep animationStep)
        {
            IsSetted = false;
            Coordinate = coordinate;
            Type = type;
            AnimationStep = animationStep;
        }

        public void Setup(bool update, bool stop, bool isIncreasing, float minimum, float maximum, float maxMinRatio, float step, float stepRatio)
        {
            IsSetted = true;
            Update = update;
            IsIncreasing = isIncreasing;
            Minimum = minimum;
            Maximum = maximum;
            MaxMinRatio = maxMinRatio;
            Step = step;
            StepRatio = stepRatio;
            Stop = stop;
        }
    }
}
