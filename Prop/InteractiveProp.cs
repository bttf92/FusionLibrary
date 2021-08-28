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
        public event EventHandler<InteractiveProp> OnInteractionComplete;

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
        public int ID => _controller.InteractiveProps.IndexOf(this);

        /// <summary>
        /// Returns true if this <see cref="AnimateProp"/> is interaction mode.
        /// </summary>
        public bool IsPlaying { get; private set; }

        private bool _invert;
        private Vector3 _axis => FusionUtils.GetUnitVector(Coordinate);
        private float _currentValue;
        private float _toValue;
        private float _sensitivity = 10;
        private bool _goBack;
        private bool _buttonOk;
        private InteractiveController _controller;

        internal InteractiveProp(InteractiveController controller, CustomModel model, Entity entity, string boneName, InteractionType interactionType, AnimationType movementType, Coordinate coordinateInteraction, Control control, bool invert, int min, int max, float startValue, float sensitivityMultiplier)
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
                AnimateProp.setOffset(Coordinate, startValue);
            else
                AnimateProp.setRotation(Coordinate, startValue);

            AnimateProp.SpawnProp();

            AnimateProp.OnAnimCompleted += AnimateProp_OnAnimCompleted;
        }

        public void SetupButton(float step, float stepRatio, bool isIncreasing)
        {
            if (InteractionType != InteractionType.Button)
                return;

            AnimateProp[MovementType][AnimationStep.First][Coordinate].Setup(true, isIncreasing, Min, Max, 1, step, stepRatio);

            _buttonOk = true;
        }

        private void AnimateProp_OnAnimCompleted(AnimationStep animationStep)
        {
            OnInteractionComplete?.Invoke(_controller, this);

            if (_buttonOk && _goBack)
            {
                _goBack = false;
                AnimateProp.Play();                
            }
        }

        internal void Play()
        {
            if (InteractionType == InteractionType.Lever)
                UpdateLeverAnimation();

            if (_buttonOk)
            {
                _goBack = true;
                AnimateProp.Play();
            }
                
            IsPlaying = true;
        }

        /// <summary>
        /// Sets value of axis <see cref="Coordinate"/> of the <see cref="AnimateProp"/>.
        /// </summary>
        /// <param name="value">Value in 0.0 - 1.0 range</param>
        public void SetValue(float value)
        {
            if (InteractionType != InteractionType.Lever)
                return;

            _toValue = value.Clamp(0, 1).Remap(0, 1, Min, Max);

            _currentValue = FusionUtils.Lerp(_currentValue, (int)_toValue, 0.1f);

            AnimateProp.SecondRotation = _axis * _currentValue;
        }

        internal void Tick()
        {
            if (!IsPlaying || InteractionType != InteractionType.Lever)
                return;

            UpdateLeverAnimation();
        }

        private void UpdateLeverAnimation()
        {
            if (_controller.LockCamera)
            {
                Game.DisableControlThisFrame(Control.LookUpDown);
                Game.DisableControlThisFrame(Control.LookLeftRight);
            }
            
            float controlInput;

            int _control = (int)Control;

            if (_controller.LockCamera && ((_control >= 1 && _control <= 6) || (_control >= 270 && _control <= 273)))
                controlInput = Game.GetDisabledControlValueNormalized(Control);
            else
                controlInput = Game.GetControlValueNormalized(Control);

            if (!_invert)
                controlInput *= -1;

            _toValue += controlInput * _sensitivity;
            _toValue = _toValue.Clamp(Min, Max);

            _currentValue = FusionUtils.Lerp(_currentValue, (int)_toValue, 0.1f);

            AnimateProp.SecondRotation = _axis * _currentValue;
        }

        internal void Stop()
        {
            //AnimateProp.Stop();

            IsPlaying = false;
        }

        internal void Dispose()
        {
            AnimateProp?.Dispose();
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
