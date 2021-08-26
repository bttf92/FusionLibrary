using FusionLibrary.Extensions;
using GTA;
using GTA.Math;
using System;
using static FusionLibrary.FusionEnums;

namespace FusionLibrary
{
    public class InteractionProp
    {
        /// <summary>
        /// This event is invoked when the <see cref="InteractionProp"/> is completed.
        /// </summary>
        public event EventHandler<InteractionProp> OnInteractionComplete;

        /// <summary>
        /// Interactable <see cref="FusionLibrary.AnimateProp"/>.
        /// </summary>
        public AnimateProp AnimateProp { get; }

        /// <summary>
        /// Interaction type of the <see cref="AnimateProp"/>.
        /// </summary>
        public InteractionType InteractionType { get; }

        /// <summary>
        /// Control used for this <see cref="InteractionProp"/>.
        /// </summary>
        public Control Control { get; }

        /// <summary>
        /// Animation type of the <see cref="InteractionProp"/>.
        /// </summary>
        public AnimationType MovementType { get; }

        /// <summary>
        /// Axis for the <see cref="AnimateProp"/> interaction.
        /// </summary>
        public Coordinate CoordinateInteraction { get; }

        /// <summary>
        /// Current value of <see cref="CoordinateInteraction"/> of the <see cref="AnimateProp"/>.
        /// </summary>
        public float CurrentValue
        {
            get
            {
                float value;

                if (MovementType == AnimationType.Offset)
                    value = AnimateProp.CurrentOffset[(int)CoordinateInteraction];
                else
                    value = AnimateProp.CurrentRotation[(int)CoordinateInteraction];

                return value.Remap(Min, Max, 0, 1);
            }
        }

        /// <summary>
        /// Maximum value.
        /// </summary>
        public float Max { get; }

        /// <summary>
        /// Minimum value.
        /// </summary>
        public float Min { get; }

        /// <summary>
        /// ID of this instance in the <see cref="InteractionController"/>.
        /// </summary>
        public int ID => _controller.InteractionProps.IndexOf(this);

        /// <summary>
        /// Returns true if this <see cref="AnimateProp"/> is interaction mode.
        /// </summary>
        public bool IsPlaying { get; private set; }

        /// <summary>
        /// Invert control or not.
        /// </summary>
        private bool _invert;

        /// <summary>
        /// Shortcut for interaction axis.
        /// </summary>
        private Vector3 _axis => FusionUtils.GetUnitVector(CoordinateInteraction);

        /// <summary>
        /// Current value.
        /// </summary>
        private float _currentValue;

        /// <summary>
        /// New value.
        /// </summary>
        private float _toValue;

        /// <summary>
        /// Sensitivity modifier.
        /// </summary>
        private float _sensitivity = 14;

        /// <summary>
        /// Owner of this <see cref="InteractionProp"/>.
        /// </summary>
        private InteractionController _controller;

        internal InteractionProp(InteractionController controller, CustomModel model, Entity entity, string boneName, InteractionType interactionType, AnimationType movementType, Coordinate coordinateInteraction, Control control, bool invert, int min, int max, float startValue, float sensitivityMultiplier)
        {
            InteractionType = interactionType;
            CoordinateInteraction = coordinateInteraction;
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
                AnimateProp.setOffset(CoordinateInteraction, startValue);
            else
                AnimateProp.setRotation(CoordinateInteraction, startValue);

            AnimateProp.SpawnProp();

            AnimateProp.OnAnimCompleted += AnimateProp_OnAnimCompleted;
        }

        private void AnimateProp_OnAnimCompleted(AnimationStep animationStep)
        {
            OnInteractionComplete?.Invoke(_controller, this);
        }

        internal void Play()
        {
            if (InteractionType == InteractionType.Lever)
                UpdateLeverAnimation();

            AnimateProp.Play();

            IsPlaying = true;
        }

        internal void Tick()
        {
            if (!IsPlaying || InteractionType != InteractionType.Lever)
                return;

            UpdateLeverAnimation();
        }

        private void UpdateLeverAnimation()
        {            
            var controlInput = Game.GetControlValueNormalized(Control);

            if (_invert)
                controlInput *= -1;

            _toValue += controlInput * _sensitivity;
            _toValue = _toValue.Clamp(Min, Max);

            _currentValue = FusionUtils.Lerp(_currentValue, (int)_toValue, 0.1f);

            AnimateProp.SecondRotation = _axis * _currentValue;
        }

        internal void Stop()
        {
            AnimateProp.Stop();

            IsPlaying = false;
        }

        internal void Dispose()
        {
            AnimateProp?.Dispose();
        }

        public static implicit operator AnimateProp(InteractionProp interactionProp)
        {
            return interactionProp.AnimateProp;
        }

        public static implicit operator Prop(InteractionProp interactionProp)
        {
            return interactionProp.AnimateProp.Prop;
        }

        public static implicit operator Entity(InteractionProp interactionProp)
        {
            return interactionProp.AnimateProp.Prop;
        }
    }
}
