using FusionLibrary.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using static FusionLibrary.FusionEnums;

namespace FusionLibrary
{
    [Serializable]
    public class Animation
    {
        public List<AnimationSettings> AnimationSettings { get; } = new List<AnimationSettings>();

        public Animation()
        {
            foreach (AnimationType animationType in Enum.GetValues(typeof(AnimationType)))
                AnimationSettings.Add(new AnimationSettings(animationType));
        }

        public Animation Clone()
        {
            MemoryStream stream = new MemoryStream();

            FusionUtils.BinaryFormatter.Serialize(stream, this);

            MemoryStream stream2 = new MemoryStream(stream.ToArray());

            return (Animation)FusionUtils.BinaryFormatter.Deserialize(stream2);
        }

        public AnimationSettings this[AnimationType animationType] => AnimationSettings[(int)animationType];

        public AnimationSettings this[int animationType] => AnimationSettings[animationType];
    }

    [Serializable]
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

    [Serializable]
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

    [Serializable]
    public class CoordinateSetting
    {
        public bool IsSetted { get; private set; }
        public Coordinate Coordinate { get; }
        public AnimationType Type { get; }
        public AnimationStep AnimationStep { get; }

        public bool Update { get; set; }
        public bool IsIncreasing { get; set; }
        public float Minimum { get; set; } = 0;
        public float Maximum { get; set; } = 0;
        public float MaxMinRatio { get; set; } = 1;
        public float Step { get; set; } = 0;
        public float StepRatio { get; set; } = 1;
        public bool Stop { get; set; }
        public bool DoNotInvert { get; set; }
        public bool SmoothEnd { get; set; }

        public float EndValue => (IsIncreasing ? Maximum : Minimum) * MaxMinRatio;
        public float StepValue => Step * StepRatio;

        public CoordinateSetting(Coordinate coordinate, AnimationType type, AnimationStep animationStep)
        {
            Coordinate = coordinate;
            Type = type;
            AnimationStep = animationStep;
        }

        public void Setup(bool stop, bool isIncreasing, float minimum, float maximum, float maxMinRatio, float step, float stepRatio, bool smoothEnd)
        {
            IsSetted = true;

            IsIncreasing = isIncreasing;
            Minimum = minimum;
            Maximum = maximum;
            MaxMinRatio = maxMinRatio;
            Step = step;
            StepRatio = stepRatio;
            Stop = stop;
            SmoothEnd = smoothEnd;
        }

        public float Clamp(float value)
        {
            return value.Clamp(Minimum * MaxMinRatio, Maximum * MaxMinRatio);
        }

        public float Progress(float value)
        {
            float ret = value / ((Maximum - Minimum) * MaxMinRatio);

            if (ret < 0)
                return -ret;

            return 1 - ret;
        }
    }
}
