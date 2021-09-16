using FusionLibrary.Extensions;
using GTA;
using GTA.Math;
using System;
using static FusionLibrary.FusionEnums;

namespace FusionLibrary
{
    /// <summary>
    /// Creates an interactive <see cref="FusionLibrary.AnimateProp"/> which can be used and moved in the game's world.
    /// </summary>
    public class InteractiveProp
    {
        /// <summary>
        /// This event is invoked when the <see cref="InteractiveProp"/> is completed.
        /// </summary>
        public event EventHandler<InteractiveProp> OnInteractionEnded;

        public event EventHandler<InteractiveProp> OnInteractionStarted;

        public event EventHandler<InteractiveProp> OnHoverStarted;

        public event EventHandler<InteractiveProp> OnHoverEnded;

        /// <summary>
        /// Interactive <see cref="FusionLibrary.AnimateProp"/>.
        /// </summary>
        public AnimateProp AnimateProp { get; }

        /// <summary>
        /// Interaction type of the <see cref="AnimateProp"/>.
        /// </summary>
        public InteractionType InteractionType { get; }

        /// <summary>
        /// Control used for this <see cref="InteractiveProp"/>.
        /// </summary>
        public Control Control { get; }

        /// <summary>
        /// Animation type of the <see cref="InteractiveProp"/>.
        /// </summary>
        public AnimationType MovementType { get; }

        /// <summary>
        /// Axis for the <see cref="AnimateProp"/> interaction.
        /// </summary>
        public Coordinate Coordinate { get; }

        /// <summary>
        /// Gets or sets if alternate controls must be used.
        /// </summary>
        public bool UseAltControl { get; set; }

        /// <summary>
        /// Alternate <see cref="GTA.Control"/> used while <see cref="UseAltControl"/> is <see langword="true"/>.
        /// </summary>
        public Control AltControl { get; set; }

        /// <summary>
        /// Current value of axis <see cref="Coordinate"/> of the <see cref="AnimateProp"/>, remapped between 0 and 1.
        /// </summary>
        public float CurrentValue => _currentValue.Remap(Min, Max, 0, 1);

        /// <summary>
        /// Maximum value.
        /// </summary>
        public float Max { get; }

        /// <summary>
        /// Minimum value.
        /// </summary>
        public float Min { get; }

        /// <summary>
        /// ID of this <see cref="InteractiveProp"/> in the <see cref="InteractiveController"/>.
        /// </summary>
        public int ID => AnimateProp.Prop.Decorator().InteractableId;

        /// <summary>
        /// Returns true if this <see cref="AnimateProp"/> is interaction mode.
        /// </summary>
        public bool IsPlaying { get; private set; }

        /// <summary>
        /// Gets or sets if interaction is blocked.
        /// </summary>
        public bool Blocked { get; set; }

        private bool _altSetup;
        private bool _altInvert;
        private readonly bool _invert;
        private Vector3 _axis => FusionUtils.GetUnitVector(Coordinate);
        private float _currentValue;
        private float _toValue;
        private readonly float _sensitivity = 10;
        private bool _roundTrip;
        private bool _waitRelease;
        private readonly InteractiveController _controller;
        private CoordinateSetting _coordinateSetting => AnimateProp[MovementType][AnimationStep.First][Coordinate];

        internal InteractiveProp(InteractiveController controller, CustomModel model, Entity entity, string boneName, InteractionType interactionType, AnimationType movementType, Coordinate coordinateInteraction, Control control, bool invert, float min, float max, float startValue, float sensitivityMultiplier)
        {
            InteractionType = interactionType;
            Coordinate = coordinateInteraction;
            MovementType = movementType;
            Control = control;

            _controller = controller;
            _invert = invert;
            _currentValue = startValue;
            _toValue = startValue;
            _sensitivity *= sensitivityMultiplier;

            AnimateProp = new AnimateProp(model, entity, boneName);

            Min = min;
            Max = max;

            if (movementType == AnimationType.Offset)
            {
                AnimateProp.setOffset(Coordinate, startValue);
            }
            else
            {
                AnimateProp.setRotation(Coordinate, startValue);
            }

            AnimateProp.SpawnProp();
        }

        /// <summary>
        /// Setups the button behaviours of this <see cref="InteractiveProp"/>.
        /// </summary>
        /// <param name="step">Step of the animation.</param>
        /// <param name="stepRatio">Step ratio of the animation.</param>
        /// <param name="isIncreasing">If new value should increase or not.</param>
        /// <param name="smoothEnd">Sets if end of movement should be smoothed.</param>
        internal void SetupAnimation(float step, float stepRatio, bool isIncreasing, bool smoothEnd)
        {
            _coordinateSetting.Setup(true, isIncreasing, Min, Max, 1, step, stepRatio, smoothEnd);

            AnimateProp.OnAnimCompleted += AnimateProp_OnAnimCompleted;
        }

        /// <summary>
        /// Setups the alternate control for this <see cref="InteractiveProp"/>.
        /// </summary>
        /// <param name="control">Alternate <see cref="GTA.Control"/>.</param>
        /// <param name="invert">Inverts the reading of the <paramref name="control"/> value.</param>
        public void SetupAltControl(Control control, bool invert)
        {
            AltControl = control;
            _altInvert = invert;

            _altSetup = true;
        }

        private void AnimateProp_OnAnimCompleted(AnimationStep animationStep)
        {
            OnInteractionEnded?.Invoke(_controller, this);

            if (InteractionType == InteractionType.Button && _roundTrip)
            {
                _waitRelease = IsPlaying;

                _roundTrip = false;

                if (!_waitRelease)
                {
                    AnimateProp.Play();
                }
            }
        }

        public void Play()
        {
            if (InteractionType == InteractionType.Lever)
            {
                UpdateLeverAnimation();
            }
            else
            {
                if (AnimateProp.IsPlaying)
                {
                    _coordinateSetting.IsIncreasing = !_coordinateSetting.IsIncreasing;
                    _roundTrip = false;
                }
                else
                {
                    _roundTrip = InteractionType == InteractionType.Button;
                    AnimateProp.Play();
                }
            }

            IsPlaying = true;
            OnInteractionStarted?.Invoke(_controller, this);
        }

        /// <summary>
        /// Sets value of axis <see cref="Coordinate"/> of the <see cref="AnimateProp"/>.
        /// </summary>
        /// <param name="value">Value in 0.0 - 1.0 range</param>
        public void SetValue(float value)
        {
            if (InteractionType != InteractionType.Lever)
            {
                return;
            }

            _toValue = value.Clamp(0, 1).Remap(0, 1, Min, Max);

            _currentValue = FusionUtils.Lerp(_currentValue, (int)_toValue, 0.1f);

            AnimateProp.SecondRotation = _axis * _currentValue;
        }

        internal void Tick()
        {
            if (AnimateProp.IsPlaying)
            {
                Vector3 value;

                if (MovementType == AnimationType.Offset)
                {
                    value = AnimateProp.CurrentOffset;
                }
                else
                {
                    value = AnimateProp.CurrentRotation;
                }

                _currentValue = value[(int)Coordinate];
            }

            if (InteractionType == InteractionType.Lever)
            {
                UpdateLeverAnimation();
            }

            if (_waitRelease && !IsPlaying)
            {
                _waitRelease = false;
                AnimateProp.Play();
            }
        }

        private void UpdateLeverAnimation()
        {
            if (IsPlaying && !Blocked)
            {
                if (_controller.LockCamera)
                {
                    Game.DisableControlThisFrame(Control.LookUpDown);
                    Game.DisableControlThisFrame(Control.LookLeftRight);
                }

                float controlInput;

                int _control = _altSetup && UseAltControl ? (int)AltControl : (int)Control;

                if (_controller.LockCamera && ((_control >= 1 && _control <= 6) || (_control >= 270 && _control <= 273)))
                {
                    controlInput = Game.GetDisabledControlValueNormalized((Control)_control);
                }
                else
                {
                    controlInput = Game.GetControlValueNormalized((Control)_control);
                }

                if ((!UseAltControl && _invert) || (UseAltControl && _altSetup && _altInvert))
                {
                    controlInput *= -1;
                }

                _toValue += controlInput * _sensitivity;
                _toValue = _toValue.Clamp(Min, Max);
            }

            _currentValue = FusionUtils.Lerp(_currentValue, (int)_toValue, 0.1f);

            AnimateProp.SecondRotation = _axis * _currentValue;
        }

        public void Stop()
        {
            IsPlaying = false;
            OnInteractionEnded?.Invoke(_controller, this);
        }

        internal void Dispose()
        {
            AnimateProp?.Dispose();
        }

        internal void HoverStart()
        {
            OnHoverStarted?.Invoke(_controller, this);
        }

        internal void HoverStop()
        {
            OnHoverEnded?.Invoke(_controller, this);
        }

        /// <summary>
        /// Implicit cast to <see cref="AnimateProp"/>.
        /// </summary>
        /// <param name="interactionProp">An instance of <paramref name="interactionProp"/>.</param>
        public static implicit operator AnimateProp(InteractiveProp interactionProp)
        {
            return interactionProp.AnimateProp;
        }

        /// <summary>
        /// Implicit cast to <see cref="Prop"/>.
        /// </summary>
        /// <param name="interactionProp">An instance of <paramref name="interactionProp"/>.</param>
        public static implicit operator Prop(InteractiveProp interactionProp)
        {
            return interactionProp.AnimateProp.Prop;
        }

        /// <summary>
        /// Implicit cast to <see cref="Entity"/>.
        /// </summary>
        /// <param name="interactionProp">An instance of <paramref name="interactionProp"/>.</param>
        public static implicit operator Entity(InteractiveProp interactionProp)
        {
            return interactionProp.AnimateProp.Prop;
        }
    }
}
