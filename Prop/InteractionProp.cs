using FusionLibrary.Extensions;
using GTA;
using System;
using static FusionLibrary.FusionEnums;

namespace FusionLibrary
{
    public class InteractionProp
    {
        public event EventHandler<InteractionProp> OnInteractionComplete;

        /// <summary>
        /// Interactable prop.
        /// </summary>
        public AnimateProp AnimateProp { get; }

        public InteractionType InteractionType { get; }

        public Control Control { get; }

        public AnimationType MovementType { get; }

        public Coordinate CoordinateInteraction { get; }

        private CoordinateSetting CoordinateSetting => AnimateProp[MovementType][AnimationStep.First][CoordinateInteraction];

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

        public int ID => _controller.InteractionProps.IndexOf(this);

        public bool IsPlaying { get; private set; }

        /// <summary>
        /// Invert control or not.
        /// </summary>
        private bool _invert;

        private float _sensitivity = 14;

        private InteractionController _controller;

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
