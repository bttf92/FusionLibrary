using FusionLibrary.Extensions;
using GTA;
using System;
using System.Collections.Generic;
using System.IO;
using static FusionLibrary.FusionEnums;

namespace FusionLibrary
{
    /// <summary>
    /// Defines all the properties for an animation.
    /// </summary>
    [Serializable]
    public class Animation
    {
        public List<AnimationTypeSettings> AnimationTypeSettings { get; } = new List<AnimationTypeSettings>();

        public Animation()
        {
            foreach (AnimationType animationType in Enum.GetValues(typeof(AnimationType)))
            {
                AnimationTypeSettings.Add(new AnimationTypeSettings(animationType));
            }
        }

        /// <summary>
        /// Clones the <see cref="Animation"/>.
        /// </summary>
        /// <returns>Cloned instance.</returns>
        public Animation Clone()
        {
            MemoryStream stream = new MemoryStream();

            FusionUtils.BinaryFormatter.Serialize(stream, this);

            MemoryStream stream2 = new MemoryStream(stream.ToArray());

            return (Animation)FusionUtils.BinaryFormatter.Deserialize(stream2);
        }

        /// <summary>
        /// Returns the <see cref="FusionLibrary.AnimationTypeSettings"/> of the <paramref name="animationType"/> type.
        /// </summary>
        /// <param name="animationType">Wanted <see cref="FusionEnums.AnimationType"/>.</param>
        /// <returns><see cref="FusionLibrary.AnimationTypeSettings"/> of <paramref name="animationType"/>.</returns>
        public AnimationTypeSettings this[AnimationType animationType] => AnimationTypeSettings[(int)animationType];

        /// <summary>
        /// Returns the <see cref="FusionLibrary.AnimationTypeSettings"/> of the <paramref name="animationType"/> type.
        /// </summary>
        /// <param name="animationType">Wanted <see cref="FusionEnums.AnimationType"/>.</param>
        /// <returns><see cref="FusionLibrary.AnimationTypeSettings"/> of <paramref name="animationType"/>.</returns>
        public AnimationTypeSettings this[int animationType] => AnimationTypeSettings[animationType];
    }

    /// <summary>
    /// Defines the settings for <see cref="AnimationType"/>.
    /// </summary>
    [Serializable]
    public class AnimationTypeSettings
    {
        public List<AnimationStepSettings> AnimationStepSettings { get; } = new List<AnimationStepSettings>();

        /// <summary>
        /// Type of this <see cref="AnimationTypeSettings"/>.
        /// </summary>
        public AnimationType Type { get; }

        internal AnimationTypeSettings(AnimationType animationType)
        {
            Type = animationType;

            foreach (AnimationStep animationStep in Enum.GetValues(typeof(AnimationStep)))
            {
                AnimationStepSettings.Add(new AnimationStepSettings(animationStep, animationType));
            }
        }

        /// <summary>
        /// Returns the <see cref="FusionLibrary.AnimationStepSettings"/> of the <paramref name="animationStep"/> step.
        /// </summary>
        /// <param name="animationStep">Wanted <see cref="FusionEnums.AnimationStep"/>.</param>
        /// <returns><see cref="FusionLibrary.AnimationStepSettings"/> of the <paramref name="animationStep"/> step.</returns>
        public AnimationStepSettings this[AnimationStep animationStep] => AnimationStepSettings[(int)animationStep];

        /// <summary>
        /// Returns the <see cref="FusionLibrary.AnimationStepSettings"/> of the <paramref name="animationStep"/> step.
        /// </summary>
        /// <param name="animationStep">Wanted <see cref="FusionEnums.AnimationStep"/>.</param>
        /// <returns><see cref="FusionLibrary.AnimationStepSettings"/> of the <paramref name="animationStep"/> step.</returns>
        public AnimationStepSettings this[int animationStep] => AnimationStepSettings[animationStep];
    }

    /// <summary>
    /// Defines the settings for <see cref="AnimationStep"/>.
    /// </summary>
    [Serializable]
    public class AnimationStepSettings
    {
        public List<CoordinateSetting> CoordinateSettings { get; } = new List<CoordinateSetting>();

        /// <summary>
        /// Step of this <see cref="AnimationStepSettings"/>.
        /// </summary>
        public AnimationStep Step { get; }

        /// <summary>
        /// <see cref="AnimationType"/> associated to parent <see cref="AnimationTypeSettings"/>.
        /// </summary>
        public AnimationType Type { get; }

        internal AnimationStepSettings(AnimationStep animationStep, AnimationType animationType)
        {
            Step = animationStep;
            Type = animationType;

            foreach (Coordinate coordinate in Enum.GetValues(typeof(Coordinate)))
            {
                CoordinateSettings.Add(new CoordinateSetting(coordinate, animationType, animationStep));
            }
        }

        /// <summary>
        /// Sets <see cref="CoordinateSetting.Update"/> to <paramref name="value"/>.
        /// </summary>
        /// <param name="value">New value.</param>
        public void SetAllUpdate(bool value)
        {
            CoordinateSettings.ForEach(x =>
            {
                if (x.IsSetted)
                {
                    x.Update = value;
                }
            });
        }

        /// <summary>
        /// Sets <see cref="CoordinateSetting.Maximum"/> to <paramref name="value"/>.
        /// </summary>
        /// <param name="value">New value.</param>
        public void SetAllMaximum(float value)
        {
            CoordinateSettings.ForEach(x =>
            {
                if (x.IsSetted)
                {
                    x.Maximum = value;
                }
            });
        }

        /// <summary>
        /// Sets <see cref="CoordinateSetting.Minimum"/> to <paramref name="value"/>.
        /// </summary>
        /// <param name="value">New value.</param>
        public void SetAllMinimum(float value)
        {
            CoordinateSettings.ForEach(x =>
            {
                if (x.IsSetted)
                {
                    x.Minimum = value;
                }
            });
        }

        /// <summary>
        /// Sets <see cref="CoordinateSetting.Step"/> to <paramref name="value"/>.
        /// </summary>
        /// <param name="value">New value.</param>
        public void SetAllStep(float value)
        {
            CoordinateSettings.ForEach(x =>
            {
                if (x.IsSetted)
                {
                    x.Step = value;
                }
            });
        }

        /// <summary>
        /// Sets <see cref="CoordinateSetting.StepRatio"/> to <paramref name="value"/>.
        /// </summary>
        /// <param name="value">New value.</param>
        public void SetAllStepRatio(float value)
        {
            CoordinateSettings.ForEach(x =>
            {
                if (x.IsSetted)
                {
                    x.StepRatio = value;
                }
            });
        }

        /// <summary>
        /// Sets <see cref="CoordinateSetting.IsIncreasing"/> to <paramref name="value"/>.
        /// </summary>
        /// <param name="value">New value.</param>
        public void SetAllIncreasing(bool value)
        {
            CoordinateSettings.ForEach(x =>
            {
                if (x.IsSetted)
                {
                    x.IsIncreasing = value;
                }
            });
        }

        /// <summary>
        /// Inverts the directions of all setted coordinates.
        /// </summary>
        public void InvertAll()
        {
            CoordinateSettings.ForEach(x =>
            {
                if (x.IsSetted && !x.DoNotInvert)
                {
                    x.IsIncreasing = !x.IsIncreasing;
                }
            });
        }

        /// <summary>
        /// Sets <see cref="CoordinateSetting.Stop"/> to <paramref name="value"/>.
        /// </summary>
        /// <param name="value">New value.</param>
        public void SetAllStop(bool value)
        {
            CoordinateSettings.ForEach(x =>
            {
                if (x.IsSetted)
                {
                    x.Stop = value;
                }
            });
        }

        /// <summary>
        /// Sets <see cref="CoordinateSetting.MaxMinRatio"/> to <paramref name="value"/>.
        /// </summary>
        /// <param name="value">New value.</param>
        public void SetAllMaxMinRatio(float value)
        {
            CoordinateSettings.ForEach(x =>
            {
                if (x.IsSetted)
                {
                    x.MaxMinRatio = value;
                }
            });
        }

        /// <summary>
        /// Returns the <see cref="CoordinateSetting"/> of the <paramref name="coordinate"/>.
        /// </summary>
        /// <param name="coordinate">Wanted <see cref="Coordinate"/>.</param>
        /// <returns><see cref="CoordinateSetting"/> of the <paramref name="coordinate"/>.</returns>
        public CoordinateSetting this[Coordinate coordinate] => CoordinateSettings[(int)coordinate];

        /// <summary>
        /// Returns the <see cref="CoordinateSetting"/> of the <paramref name="coordinate"/>.
        /// </summary>
        /// <param name="coordinate">Wanted <see cref="Coordinate"/>.</param>
        /// <returns><see cref="CoordinateSetting"/> of the <paramref name="coordinate"/>.</returns>
        public CoordinateSetting this[int coordinate] => CoordinateSettings[coordinate];
    }

    [Serializable]
    public class CoordinateSetting
    {
        /// <summary>
        /// if <see langword="true"/> this <see cref="CoordinateSetting"/> was setup.
        /// </summary>
        public bool IsSetted { get; private set; }

        /// <summary>
        /// <see cref="FusionEnums.Coordinate"/> of this setting.
        /// </summary>
        public Coordinate Coordinate { get; }

        /// <summary>
        /// <see cref="AnimationType"/> associated to parent <see cref="AnimationTypeSettings"/>.
        /// </summary>
        public AnimationType Type { get; }

        /// <summary>
        /// <see cref="FusionEnums.AnimationStep"/> associated to parent <see cref="AnimationStepSettings"/>.
        /// </summary>
        public AnimationStep AnimationStep { get; }

        /// <summary>
        /// Whether this <see cref="CoordinateSetting"/> is played.
        /// </summary>
        public bool Update { get; set; }

        /// <summary>
        /// Gets or sets if this <see cref="Coordinate"/> should increase or not.
        /// </summary>
        public bool IsIncreasing { get; set; }

        /// <summary>
        /// Minimum value that can be reached.
        /// </summary>
        public float Minimum { get; set; } = 0;

        /// <summary>
        /// Maximum value that can be reached.
        /// </summary>
        public float Maximum { get; set; } = 0;

        private float _maxMinRatio = 1;
        /// <summary>
        /// Ratio applied to max/min values. Clamped between 0 and 1.
        /// </summary>
        public float MaxMinRatio
        {
            get => _maxMinRatio;

            set => _maxMinRatio = value.Clamp(0, 1);
        }

        /// <summary>
        /// Step value.
        /// </summary>
        public float Step { get; set; } = 0;

        private float _stepRatio = 1;
        /// <summary>
        /// Ratio applied to step value. Clamped between 0 and 1.
        /// </summary>
        public float StepRatio
        {
            get => _stepRatio;

            set => _stepRatio = value.Clamp(0, 1);
        }

        /// <summary>
        /// Whether animation of this <see cref="Coordinate"/> should stop at reaching max or min value.
        /// </summary>
        public bool Stop { get; set; }

        /// <summary>
        /// Whether direction of this <see cref="Coordinate"/> should be inverted when reaches a stop.
        /// </summary>
        public bool DoNotInvert { get; set; }

        /// <summary>
        /// Smoothes the end of the animation.
        /// </summary>
        public bool SmoothEnd { get; set; }

        /// <summary>
        /// End value of this <see cref="Coordinate"/> = <c>(IsIncreasing ? Maximum : Minimum) * MaxMinRatio</c>.
        /// </summary>
        public float EndValue => (IsIncreasing ? Maximum : Minimum) * MaxMinRatio;

        /// <summary>
        /// Step value of this <see cref="Coordinate"/> = <c>Step * StepRatio</c>.
        /// </summary>
        public float StepValue => Step * StepRatio;

        internal CoordinateSetting(Coordinate coordinate, AnimationType type, AnimationStep animationStep)
        {
            Coordinate = coordinate;
            Type = type;
            AnimationStep = animationStep;
        }

        /// <summary>
        /// Setups the current <see cref="Coordinate"/> animation.
        /// </summary>
        /// <param name="stop">Whether animation should stop at reaching max or min value.</param>
        /// <param name="isIncreasing">If this <see cref="Coordinate"/> value should increase or not.</param>
        /// <param name="minimum">Maximum value.</param>
        /// <param name="maximum">Minimum value.</param>
        /// <param name="maxMinRatio">Max/min ratio value.</param>
        /// <param name="step">Step value</param>
        /// <param name="stepRatio">Step ratio value.</param>
        /// <param name="smoothEnd">Smoothes the end of the animation.</param>
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

        /// <summary>
        /// Clamps <paramref name="value"/> between <see cref="Minimum"/> and <see cref="Maximum"/>. <see cref="MaxMinRatio"/> is considered.
        /// </summary>
        /// <param name="value">Value to be clamped.</param>
        /// <returns>Clamped value.</returns>
        public float Clamp(float value)
        {
            return value.Clamp(Minimum * MaxMinRatio, Maximum * MaxMinRatio);
        }

        /// <summary>
        /// Gets the progress based on <paramref name="value"/>.
        /// </summary>
        /// <param name="value">Current value.</param>
        /// <returns>Progress value.</returns>
        public float Progress(float value)
        {
            float ret = value / ((Maximum - Minimum) * MaxMinRatio);

            if (ret < 0)
            {
                return -ret;
            }

            return 1 - ret;
        }

        /// <summary>
        /// Updates <paramref name="current"/> value based on <see cref="IsIncreasing"/>, <see cref="StepValue"/> and <see cref="EndValue"/>.
        /// </summary>
        /// <param name="current">Current value.</param>
        /// <returns>Updated value.</returns>
        internal float UpdateValue(float current)
        {
            float newValue = current;

            if (IsIncreasing)
            {
                newValue += StepValue;
            }
            else
            {
                newValue -= StepValue;
            }

            float modifier = 1;

            if (SmoothEnd)
            {
                float progress = Progress(current);

                if (IsIncreasing && progress <= 0.25f)
                {
                    modifier = progress.Remap(0.25f, 0, 1, 0.05f);
                }

                if (!IsIncreasing && progress >= 0.75f)
                {
                    modifier = progress.Remap(0.75f, 1f, 1, 0.05f);
                }
            }

            current = Clamp(FusionUtils.Lerp(current, newValue, Game.LastFrameTime * modifier));

            if (current == EndValue)
            {
                if (!DoNotInvert)
                {
                    IsIncreasing = !IsIncreasing;
                }

                if (Stop)
                {
                    Update = false;
                }
            }

            return current;
        }
    }
}