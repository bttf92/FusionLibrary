using FusionLibrary.Extensions;
using GTA;
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
        /// Shortcut for <see cref="FusionLibrary.CoordinateSetting"/> of <see cref="AnimateProp"/>.
        /// </summary>
        private CoordinateSetting CoordinateSetting => AnimateProp[MovementType][AnimationStep.First][CoordinateInteraction];

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

                return value.Remap(CoordinateSetting.Minimum, CoordinateSetting.Maximum, 0, 1);
            }
        }

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
        /// Sensitivity modifier.
        /// </summary>
        private float _sensitivity = 14;

        /// <summary>
        /// Owner of this <see cref="InteractionProp"/>.
        /// </summary>
        private InteractionController _controller;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="model"></param>
        /// <param name="entity"></param>
        /// <param name="boneName"></param>
        /// <param name="interactionType"></param>
        /// <param name="movementType"></param>
        /// <param name="coordinateInteraction"></param>
        /// <param name="control"></param>
        /// <param name="invert"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="startValue"></param>
        /// <param name="step"></param>
        /// <param name="sensitivityMultiplier"></param>
        internal InteractionProp(InteractionController controller, CustomModel model, Entity entity, string boneName, InteractionType interactionType, AnimationType movementType, Coordinate coordinateInteraction, Control control, bool invert, int min, int max, float startValue, float step, float sensitivityMultiplier)
        {
            InteractionType = interactionType;
            CoordinateInteraction = coordinateInteraction;
            MovementType = movementType;
            Control = control;

            _controller = controller;
            _invert = invert;
            _sensitivity *= sensitivityMultiplier;

            AnimateProp = new AnimateProp(model, entity, boneName);

            CoordinateSetting.Setup(true, true, min, max, 1, step, 1);
            
            if (startValue != 0)
            {
                if (movementType == AnimationType.Offset)
                    AnimateProp.setOffset(CoordinateInteraction, startValue);
                else
                    AnimateProp.setRotation(CoordinateInteraction, startValue);
            }

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
            float controlValue = Game.GetControlValueNormalized(Control) * _sensitivity;

            if (_invert)
            {
                if (controlValue > 0 && CoordinateSetting.IsIncreasing)
                    CoordinateSetting.IsIncreasing = false;

                if (controlValue < 0 && !CoordinateSetting.IsIncreasing)
                    CoordinateSetting.IsIncreasing = true;
            }
            else
            {
                if (controlValue > 0 && !CoordinateSetting.IsIncreasing)
                    CoordinateSetting.IsIncreasing = true;

                if (controlValue < 0 && CoordinateSetting.IsIncreasing)
                    CoordinateSetting.IsIncreasing = false;
            }

            CoordinateSetting.StepRatio = Math.Abs(controlValue);
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
