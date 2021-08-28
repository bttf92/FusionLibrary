using FusionLibrary.Extensions;
using GTA;
using System;
using System.Collections.Generic;
using static FusionLibrary.FusionEnums;

namespace FusionLibrary
{
    /// <summary>
    /// Creates a controller for <see cref="InteractiveProp"/>.
    /// </summary>
    public class InteractiveController
    {
        internal static List<InteractiveController> GlobalInteractiveControllerList = new List<InteractiveController>();

        internal static void TickAll()
        {
            for (int i = 0; i < GlobalInteractiveControllerList.Count; i++)
                GlobalInteractiveControllerList[i].Tick();
        }

        /// <summary>
        /// This event is invoked when a <see cref="InteractiveProp"/> is completed.
        /// </summary>
        public event EventHandler<InteractiveProp> OnInteractionComplete;

        /// <summary>
        /// List of <see cref="InteractiveProp"/>.
        /// </summary>
        public List<InteractiveProp> InteractiveProps { get; } = new List<InteractiveProp>();

        /// <summary>
        /// Index of the selected <see cref="InteractiveProp"/>.
        /// </summary>
        public int CurrentInteractiveID { get; private set; } = -1;

        /// <summary>
        /// Returns true if this <see cref="InteractiveController"/> is playing.
        /// </summary>
        public bool IsPlaying { get; private set; }

        /// <summary>
        /// Gets or sets if camera should be locked while interacting with <see cref="InteractiveProp"/>.
        /// </summary>
        public bool LockCamera { get; set; }

        /// <summary>
        /// Gets or sets if release of a lever should be smoothed.
        /// </summary>
        public bool SmoothRelease { get; set; }

        /// <summary>
        /// Returns the selected <see cref="InteractiveProp"/>.
        /// </summary>
        public InteractiveProp CurrentInteractiveProp
        {
            get
            {
                if (CurrentInteractiveID == -1)
                    return null;

                return InteractiveProps[CurrentInteractiveID];
            }
        }

        public bool UseAltControl 
        { 
            get
            {
                return InteractiveProps[0].UseAltControl;
            } 
            set
            {
                for (int i = 0; i < InteractiveProps.Count; i++)
                    InteractiveProps[i].UseAltControl = value;
            }
        }

        /// <summary>
        /// ID for hover <see cref="InteractiveProp"/>.
        /// </summary>
        private int _hoverId = -1;

        /// <summary>
        /// Creates a new instance of <see cref="InteractiveController"/>.
        /// </summary>
        /// <param name="lockCamera">If camera should be locked while interacting with <see cref="InteractiveProp"/>.</param>
        /// <param name="smoothRelease">if release of a lever should be smoothed.</param>
        public InteractiveController(bool lockCamera = false, bool smoothRelease = false)
        {            
            LockCamera = lockCamera;
            SmoothRelease = smoothRelease;

            GlobalInteractiveControllerList.Add(this);
        }

        /// <summary>
        /// Creates a new <see cref="InteractiveProp"/>.
        /// </summary>
        /// <param name="model"><see cref="CustomModel"/> to be used for the <see cref="AnimateProp"/>.</param>
        /// <param name="entity"><see cref="Entity"/> at which the <see cref="AnimateProp"/>.</param>
        /// <param name="boneName">Bone name of <paramref name="entity"/> used for attach.</param>
        /// <param name="interactionType"><see cref="InteractionType"/> for this <see cref="InteractiveProp"/>.</param>
        /// <param name="movementType"><see cref="AnimationType"/> for this <see cref="InteractiveProp"/>.</param>
        /// <param name="coordinateInteraction"><see cref="Coordinate"/> of the axis</param>
        /// <param name="control"><see cref="Control"/> which fires up this <see cref="InteractiveProp"/>.</param>
        /// <param name="invert">Inverts the reading of the <paramref name="control"/> value.</param>
        /// <param name="min">Minimum value.</param>
        /// <param name="max">Maximum value.</param>
        /// <param name="startValue">Starting value.</param>
        /// <param name="sensitivityMultiplier">Sensitivity multiplier for <paramref name="control"/> value.</param>
        /// <returns>New instance of <see cref="InteractiveProp"/>.</returns>
        public InteractiveProp Add(CustomModel model, Entity entity, string boneName, InteractionType interactionType, AnimationType movementType, Coordinate coordinateInteraction, Control control, bool invert, int min, int max, float startValue = 0f, float sensitivityMultiplier = 1f)
        {
            InteractiveProp interactionProp;

            InteractiveProps.Add(interactionProp = new InteractiveProp(this, model, entity, boneName, interactionType, movementType, coordinateInteraction, control, invert, min, max, startValue, sensitivityMultiplier));

            interactionProp.OnInteractionComplete += InteractionProp_OnInteractionComplete;

            interactionProp.AnimateProp.Prop.Decorator().InteractableEntity = true;
            interactionProp.AnimateProp.Prop.Decorator().InteractableId = InteractiveProps.IndexOf(interactionProp);

            return interactionProp;
        }

        private void InteractionProp_OnInteractionComplete(object sender, InteractiveProp e)
        {
            OnInteractionComplete?.Invoke(sender, e);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            for (int i = 0; i < InteractiveProps.Count; i++)
                InteractiveProps[i].Dispose();

            GlobalInteractiveControllerList.Remove(this);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Play()
        {
            IsPlaying = true;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Stop()
        {
            StopInteraction();
            StopHover();

            IsPlaying = false;
        }

        internal void Tick()
        {
            if (!IsPlaying)
                return;

            UpdateInteraction();

            if (SmoothRelease)
                for (int i = 0; i < InteractiveProps.Count; i++)
                    InteractiveProps[i].Tick();
            else
                CurrentInteractiveProp?.Tick();
        }

        private void UpdateInteraction()
        {
            if (CurrentInteractiveID == -1)
            {
                if (FusionUtils.PlayerPed.Weapons.Current.Model != 0)
                {
                    StopHover();

                    return;
                }

                RaycastResult raycast;

                if (GameplayCamera.IsRendering)
                    raycast = World.Raycast(GameplayCamera.Position, GameplayCamera.Direction, 10, IntersectFlags.Objects, FusionUtils.PlayerPed);
                else
                    raycast = World.Raycast(World.RenderingCamera.Position, World.RenderingCamera.Direction, 10, IntersectFlags.Objects, FusionUtils.PlayerPed);

                if (!raycast.DidHit || !raycast.HitEntity.NotNullAndExists() || raycast.HitEntity.Decorator().InteractableEntity == false)
                {
                    StopHover();

                    return;
                }

                int id = raycast.HitEntity.Decorator().InteractableId;

                if (InteractiveProps[id] != raycast.HitEntity || InteractiveProps[id].AnimateProp.IsPlaying)
                {
                    StopHover();

                    return;
                }

                raycast.HitEntity.SetAlpha(AlphaLevel.L4);
                _hoverId = id;

                if (Game.IsControlPressed(Control.Attack))
                {
                    StopHover();

                    CurrentInteractiveID = id;
                    CurrentInteractiveProp?.Play();
                }
                else if (Game.IsControlJustReleased(Control.Attack))
                    StopHover();
            }
            else if (Game.IsControlJustReleased(Control.Attack) || FusionUtils.PlayerPed.Weapons.Current.Model != 0)
                StopInteraction();
        }

        private void StopInteraction()
        {
            if (CurrentInteractiveID == -1)
                return;

            CurrentInteractiveProp?.Stop();
            CurrentInteractiveID = -1;
        }

        private void StopHover()
        {
            if (_hoverId == -1)
                return;

            InteractiveProps[_hoverId].AnimateProp.Prop.SetAlpha(AlphaLevel.L5);
            _hoverId = -1;
        }

        /// <summary>
        /// Returns the <see cref="InteractiveProp"/> with this <paramref name="index"/>.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public InteractiveProp this[int index] => InteractiveProps[index];
    }
}