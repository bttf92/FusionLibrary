using FusionLibrary.Extensions;
using GTA;
using System;
using System.Collections.Generic;
using static FusionLibrary.FusionEnums;

namespace FusionLibrary
{
    public class InteractionController
    {
        internal static List<InteractionController> GlobalInteractionControllerList = new List<InteractionController>();

        internal static void TickAll()
        {
            for (int i = 0; i < GlobalInteractionControllerList.Count; i++)
                GlobalInteractionControllerList[i].Tick();
        }

        public event EventHandler<InteractionProp> OnInteractionComplete;

        public List<InteractionProp> InteractionProps { get; } = new List<InteractionProp>();

        public int CurrentInteractionIndex { get; private set; } = -1;

        public bool IsPlaying { get; private set; }

        public InteractionProp CurrentInteractionProp
        {
            get
            {
                if (CurrentInteractionIndex == -1)
                    return null;

                return InteractionProps[CurrentInteractionIndex];
            }
        }

        private int _hoverId = -1;

        public InteractionController()
        {
            GlobalInteractionControllerList.Add(this);
        }

        public InteractionProp Add(CustomModel model, Entity entity, string boneName, InteractionType interactionType, AnimationType movementType, Coordinate coordinateInteraction, Control control, bool invert, int min, int max, float startValue = 0f, float step = 1f, float sensitivityMultiplier = 1f)
        {
            InteractionProp interactionProp;

            InteractionProps.Add(interactionProp = new InteractionProp(this, model, entity, boneName, interactionType, movementType, coordinateInteraction, control, invert, min, max, startValue, step, sensitivityMultiplier));

            interactionProp.OnInteractionComplete += InteractionProp_OnInteractionComplete;

            interactionProp.AnimateProp.Prop.Decorator().InteractableEntity = true;
            interactionProp.AnimateProp.Prop.Decorator().InteractableId = InteractionProps.IndexOf(interactionProp);

            return interactionProp;
        }

        private void InteractionProp_OnInteractionComplete(object sender, InteractionProp e)
        {
            OnInteractionComplete?.Invoke(sender, e);

            StopAnimation();
        }

        public void Dispose()
        {
            for (int i = 0; i < InteractionProps.Count; i++)
                InteractionProps[i].Dispose();

            GlobalInteractionControllerList.Remove(this);
        }

        public void Play()
        {
            IsPlaying = true;
        }

        public void Stop()
        {
            StopAnimation();

            IsPlaying = false;
        }

        internal void Tick()
        {
            if (!IsPlaying)
                return;

            if (CurrentInteractionIndex == -1)
            {
                RaycastResult raycast = World.Raycast(GameplayCamera.Position, GameplayCamera.Direction, 10, IntersectFlags.Everything, FusionUtils.PlayerPed);

                if (!raycast.DidHit || !raycast.HitEntity.NotNullAndExists() || raycast.HitEntity.Decorator().InteractableEntity == false)
                {
                    StopHover();

                    return;
                }

                int id = raycast.HitEntity.Decorator().InteractableId;

                if (InteractionProps[id] != raycast.HitEntity)
                {
                    StopHover();

                    return;
                }

                raycast.HitEntity.SetAlpha(AlphaLevel.L3);
                _hoverId = id;

                if (Game.IsControlPressed(Control.Attack))
                {
                    StopHover();

                    CurrentInteractionIndex = id;
                    CurrentInteractionProp?.Play();
                }
            }
            else if (!Game.IsControlPressed(Control.Attack))
                StopAnimation();

            CurrentInteractionProp?.Tick();
        }

        private void StopAnimation()
        {
            if (CurrentInteractionIndex == -1)
                return;

            CurrentInteractionProp?.Stop();
            CurrentInteractionIndex = -1;
        }

        private void StopHover()
        {
            if (_hoverId == -1)
                return;

            InteractionProps[_hoverId].AnimateProp.Prop.SetAlpha(AlphaLevel.L5);
            _hoverId = -1;
        }

        public InteractionProp this[int index] => InteractionProps[index];
    }
}