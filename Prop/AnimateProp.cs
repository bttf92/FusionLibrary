using FusionLibrary.Extensions;
using GTA;
using GTA.Math;
using GTA.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using static FusionLibrary.FusionEnums;

namespace FusionLibrary
{
    public delegate void OnAnimCompleted(AnimationStep animationStep);

    public class AnimateProp
    {
        public event OnAnimCompleted OnAnimCompleted;

        internal static List<AnimateProp> GlobalAnimatePropList = new List<AnimateProp>();

        internal static void TickAll()
        {
            for (int i = 0; i < GlobalAnimatePropList.Count; i++)
                GlobalAnimatePropList[i].Tick();
        }

        public Entity Entity { get; protected set; }

        public bool IsPlaying { get; private set; }

        public Prop Prop { get; private set; }
        public CustomModel Model { get; private set; }
        public bool UsePhysicalAttach { get; set; }
        public bool UseFixedRot { get; set; } = true;
        public float Duration { get; set; } = 0;

        public bool IsSpawned { get; private set; }

        public Vector3 Offset { get; }
        public Vector3 Rotation { get; }

        public Vector3 SecondOffset = Vector3.Zero;
        public Vector3 SecondRotation = Vector3.Zero;

        public Vector3 CurrentOffset => Offset + SecondOffset;
        public Vector3 CurrentRotation => Rotation + SecondRotation;

        public AnimationStep AnimationStep { get; private set; } = AnimationStep.Off;

        private bool ToBone;
        private EntityBone Bone;
        public bool IsDetached { get; private set; }
        private float _currentTime = 0;

        public Animation Animation { get; private set; } = new Animation();

        public Animation SavedAnimation { get; private set; } = new Animation();
        public Vector3 SavedOffset { get; private set; } = new Vector3();
        public Vector3 SavedRotation { get; private set; } = new Vector3();

        public bool SmoothEnd { get; set; }

        public AnimateProp(CustomModel pModel, Entity pEntity, EntityBone entityBone, Vector3 pOffset, Vector3 pRotation, bool smoothEnd = false)
        {
            Model = pModel;
            Entity = pEntity;
            Bone = entityBone;
            Offset = pOffset;
            Rotation = pRotation;
            ToBone = true;
            SmoothEnd = smoothEnd;

            GlobalAnimatePropList.Add(this);
        }

        public AnimateProp(CustomModel pModel, Entity pEntity, EntityBone entityBone, bool smoothEnd = false) : this(pModel, pEntity, entityBone, Vector3.Zero, Vector3.Zero, smoothEnd)
        {

        }

        public AnimateProp(CustomModel pModel, Entity pEntity, string boneName, bool smoothEnd = false) : this(pModel, pEntity, pEntity.Bones[boneName], Vector3.Zero, Vector3.Zero, smoothEnd)
        {

        }

        public AnimateProp(CustomModel pModel, Entity pEntity, string boneName, Vector3 pOffset, Vector3 pRotation, bool smoothEnd = false) : this(pModel, pEntity, pEntity.Bones[boneName], pOffset, pRotation, smoothEnd)
        {

        }

        public AnimateProp(CustomModel pModel, Entity pEntity, Vector3 pOffset, Vector3 pRotation, bool smoothEnd = false)
        {
            Model = pModel;
            Entity = pEntity;
            Offset = pOffset;
            Rotation = pRotation;
            SmoothEnd = smoothEnd;

            GlobalAnimatePropList.Add(this);
        }

        public AnimateProp(CustomModel pModel, Entity pEntity, bool smoothEnd = false) : this(pModel, pEntity, Vector3.Zero, Vector3.Zero, smoothEnd)
        {

        }

        public bool Visible
        {
            get
            {
                if (Prop != null && Prop.Exists())
                    return Prop.IsVisible;

                return false;
            }
            set
            {
                if (Prop != null && Prop.Exists())
                    Prop.IsVisible = value;
            }
        }

        public Vector3 RelativePosition => ToBone ? Bone.GetRelativeOffsetPosition(CurrentOffset) : CurrentOffset;

        public Vector3 Position => ToBone ? Bone.GetOffsetPosition(CurrentOffset) : Entity.GetOffsetPosition(CurrentOffset);

        public Vector3 WorldPosition
        {
            get
            {
                if (!Prop.NotNullAndExists())
                    return Vector3.Zero;

                return Prop.Position;
            }
        }

        public void SaveAnimation()
        {
            SavedAnimation = Animation.Clone();
            SavedOffset = SecondOffset;
            SavedRotation = SecondRotation;
        }

        public void RestoreAnimation()
        {
            Animation = SavedAnimation.Clone();
            SecondOffset = SavedOffset;
            SecondRotation = SavedRotation;

            Attach();
        }

        public void setOffset(Coordinate coordinate, float value, bool isCurrent = false)
        {
            if (isCurrent)
                SecondOffset[(int)coordinate] = value - Offset[(int)coordinate];
            else
                SecondOffset[(int)coordinate] = value;
        }

        public void setOffset(Vector3 value, bool isCurrent = false)
        {
            if (isCurrent)
                SecondOffset = value - Offset;
            else
                SecondOffset = value;
        }

        public void setRotation(Coordinate coordinate, float value, bool isCurrent = false)
        {
            if (isCurrent)
                SecondRotation[(int)coordinate] = value - Rotation[(int)coordinate];
            else
                SecondRotation[(int)coordinate] = value;
        }

        public void setRotation(Vector3 value, bool isCurrent = false)
        {
            if (isCurrent)
                SecondRotation = value - Rotation;
            else
                SecondRotation = value;
        }

        public void MoveProp(Vector3 offset, Vector3 rotation, bool isCurrent = true)
        {
            if (isCurrent)
            {
                SecondOffset = offset - Offset;
                SecondRotation = rotation - Rotation;
            }
            else
            {
                SecondOffset = offset;
                SecondRotation = rotation;
            }

            if (!IsSpawned)
                SpawnProp();
        }

        public void SwapModel(CustomModel model)
        {
            Model = model;
            Prop?.Delete();

            if (IsSpawned)
                SpawnProp();
        }

        public void TransferTo(Entity entity)
        {
            Entity = entity;
            Attach();
        }

        public void TransferTo(Entity entity, string boneName)
        {
            Bone = entity.Bones[boneName];
            TransferTo(entity);
        }

        public void TransferTo(Entity entity, EntityBone entityBone)
        {
            Bone = entityBone;
            TransferTo(entity);
        }

        public void SetState(bool state)
        {
            if (state)
                SpawnProp();
            else
                Delete();
        }

        public void setCoordinateAt(bool maximum, AnimationType animationType, AnimationStep animationStep, Coordinate coordinate)
        {
            CoordinateSetting coordinateSetting = Animation[animationType][animationStep][coordinate];

            if (animationType == AnimationType.Offset)
                SecondOffset[(int)coordinate] = (maximum ? coordinateSetting.Maximum : coordinateSetting.Minimum) * coordinateSetting.MaxMinRatio - Offset[(int)coordinate];
            else
                SecondRotation[(int)coordinate] = (maximum ? coordinateSetting.Maximum : coordinateSetting.Minimum) * coordinateSetting.MaxMinRatio - Rotation[(int)coordinate];

            Attach();
        }

        public void setInstantAnimationStep(AnimationStep animationStep)
        {
            List<CoordinateSetting> offsetSettings = Animation[AnimationType.Offset][animationStep].Coordinates.Where(x => x.IsSetted).ToList();
            List<CoordinateSetting> rotationSettings = Animation[AnimationType.Rotation][animationStep].Coordinates.Where(x => x.IsSetted).ToList();

            offsetSettings.ForEach(x =>
            {
                float val;

                if (x.IsIncreasing)
                    val = x.Maximum * x.MaxMinRatio;
                else
                    val = x.Minimum * x.MaxMinRatio;

                x.IsIncreasing = !x.IsIncreasing;

                SecondOffset[(int)x.Coordinate] = val - Offset[(int)x.Coordinate];
            });

            rotationSettings.ForEach(x =>
            {
                float val;

                if (x.IsIncreasing)
                    val = x.Maximum * x.MaxMinRatio;
                else
                    val = x.Minimum * x.MaxMinRatio;

                x.IsIncreasing = !x.IsIncreasing;

                SecondRotation[(int)x.Coordinate] = val - Rotation[(int)x.Coordinate];
            });

            Attach();
        }

        public void Play()
        {
            Play(AnimationStep.First);
        }

        public void Play(bool instant = false, bool spawnAndRestore = false)
        {
            Play(AnimationStep.First, instant, false, spawnAndRestore);
        }

        public void Play(AnimationStep animationStep, bool instant = false, bool playInstantPreviousSteps = false, bool spawnAndRestore = false)
        {
            if (spawnAndRestore)
            {
                RestoreAnimation();
                SpawnProp();
            }

            if (playInstantPreviousSteps)
            {
                for (AnimationStep prevStep = AnimationStep.First; prevStep < animationStep; prevStep++)
                    setInstantAnimationStep(prevStep);
            }

            if (instant)
            {
                setInstantAnimationStep(animationStep);
                return;
            }

            Animation[AnimationType.Offset][animationStep].setAllUpdate(true);
            Animation[AnimationType.Rotation][animationStep].setAllUpdate(true);
            AnimationStep = animationStep;
            IsPlaying = true;
        }

        internal void Tick()
        {
            if (!IsSpawned)
                return;

            if (!Entity.NotNullAndExists() | !Prop.NotNullAndExists())
            {
                Delete();
                return;
            }

            if (Duration > 0)
            {
                _currentTime += Game.LastFrameTime;

                if (_currentTime >= Duration)
                {
                    Delete();
                    return;
                }
            }

            if (IsDetached)
                return;

            if (IsPlaying)
            {
                List<CoordinateSetting> offsetSettings = Animation[AnimationType.Offset][AnimationStep].Coordinates.Where(x => x.IsSetted && x.Update).ToList();
                List<CoordinateSetting> rotationSettings = Animation[AnimationType.Rotation][AnimationStep].Coordinates.Where(x => x.IsSetted && x.Update).ToList();

                offsetSettings.ForEach(x => UpdateCoordinate(x));
                rotationSettings.ForEach(x => UpdateCoordinate(x));

                Attach();

                if (rotationSettings.Count == 0 && offsetSettings.Count == 0)
                {
                    IsPlaying = false;

                    AnimationStep animationStep = AnimationStep;
                    AnimationStep = AnimationStep.Off;

                    OnAnimCompleted?.Invoke(animationStep);
                }

                return;
            }

            Attach();
        }

        private void UpdateCoordinate(CoordinateSetting coordinateSetting)
        {
            if (!coordinateSetting.Update)
                return;

            int i = (int)coordinateSetting.Coordinate;

            float current;

            if (coordinateSetting.Type == AnimationType.Offset)
                current = CurrentOffset[i];
            else
                current = CurrentRotation[i];

            if (SmoothEnd)
            {
                float newValue = coordinateSetting.IsIncreasing ? coordinateSetting.Maximum : coordinateSetting.Minimum;

                newValue *= coordinateSetting.MaxMinRatio;

                current = FusionUtils.Lerp(current, newValue, Game.LastFrameTime * coordinateSetting.Step * coordinateSetting.StepRatio);

                if (coordinateSetting.Type == AnimationType.Offset)
                    SecondOffset[i] = current - Offset[i];
                else
                    SecondRotation[i] = current - Rotation[i];

                if (Math.Abs(current).Near(Math.Abs(newValue), 0.1f))
                {
                    if (!coordinateSetting.DoNotInvert)
                        coordinateSetting.IsIncreasing = !coordinateSetting.IsIncreasing;

                    if (coordinateSetting.Stop)
                        coordinateSetting.Update = false;
                }
            }
            else
            {
                float newValue = current;
                float step = coordinateSetting.Step * coordinateSetting.StepRatio;

                if (coordinateSetting.IsIncreasing)
                    newValue += step;
                else
                    newValue -= step;

                float endValue = coordinateSetting.IsIncreasing ? coordinateSetting.Maximum : coordinateSetting.Minimum;

                endValue *= coordinateSetting.MaxMinRatio;

                current = FusionUtils.Lerp(current, newValue, Game.LastFrameTime).Clamp(coordinateSetting.Minimum * coordinateSetting.MaxMinRatio, coordinateSetting.Maximum * coordinateSetting.MaxMinRatio);

                if (coordinateSetting.Type == AnimationType.Offset)
                    SecondOffset[i] = current - Offset[i];
                else
                    SecondRotation[i] = current - Rotation[i];

                if (current == endValue)
                {
                    if (!coordinateSetting.DoNotInvert)
                        coordinateSetting.IsIncreasing = !coordinateSetting.IsIncreasing;

                    if (coordinateSetting.Stop)
                        coordinateSetting.Update = false;
                }
            }
        }

        public void SpawnProp()
        {
            if (Prop.NotNullAndExists())
            {
                IsSpawned = true;
                return;
            }

            if (!Entity.NotNullAndExists())
                return;

            Prop = World.CreateProp(Model, Entity.Position, false, false);
            Prop.ApplyForce(Vector3.UnitX);
            Prop.IsPersistent = true;

            IsSpawned = true;

            Attach();
        }

        private void Attach()
        {
            if (!Prop.NotNullAndExists() || !Entity.NotNullAndExists())
                return;

            if (ToBone)
            {
                if (UsePhysicalAttach)
                    Function.Call(Hash.ATTACH_ENTITY_TO_ENTITY_PHYSICALLY, Prop.Handle, Entity.Handle, 0, Bone.Index, CurrentOffset.X, CurrentOffset.Y, CurrentOffset.Z, 0, 0, 0, CurrentRotation.X, CurrentRotation.Y, CurrentRotation.Z, 1000000.0f, UseFixedRot, true, true, true, 2);
                else
                    Function.Call(Hash.ATTACH_ENTITY_TO_ENTITY, Prop.Handle, Entity.Handle, Bone.Index, CurrentOffset.X, CurrentOffset.Y, CurrentOffset.Z, CurrentRotation.X, CurrentRotation.Y, CurrentRotation.Z, false, false, true, false, 0, UseFixedRot);
            }
            else
            {
                if (UsePhysicalAttach)
                    Function.Call(Hash.ATTACH_ENTITY_TO_ENTITY_PHYSICALLY, Prop.Handle, Entity.Handle, 0, 0, CurrentOffset.X, CurrentOffset.Y, CurrentOffset.Z, 0, 0, 0, CurrentRotation.X, CurrentRotation.Y, CurrentRotation.Z, 1000000.0f, UseFixedRot, true, true, true, 2);
                else
                    Function.Call(Hash.ATTACH_ENTITY_TO_ENTITY, Prop.Handle, Entity.Handle, 0, CurrentOffset.X, CurrentOffset.Y, CurrentOffset.Z, CurrentRotation.X, CurrentRotation.Y, CurrentRotation.Z, false, false, true, false, 0, UseFixedRot);
            }
        }

        public void Delete(bool keepProp = false)
        {
            IsSpawned = false;
            IsDetached = false;
            _currentTime = 0;
            AnimationStep = AnimationStep.Off;

            if (keepProp && Prop.NotNullAndExists())
            {
                Detach();
                Prop.IsPersistent = false;
            }
            else
                Prop?.Delete();
        }

        public void Detach()
        {
            Prop.Detach();
            Prop.IsPositionFrozen = false;
            IsDetached = true;
        }

        public void ScatterProp(float ForceMultiplier = 1f)
        {
            Detach();
            Prop.ApplyForce(Vector3.RandomXYZ() * ForceMultiplier, Vector3.RandomXYZ() * ForceMultiplier);
        }

        public void Stop()
        {
            Animation[AnimationType.Offset][AnimationStep].setAllUpdate(false);
            Animation[AnimationType.Rotation][AnimationStep].setAllUpdate(false);

            AnimationStep = AnimationStep.Off;
            IsPlaying = false;
        }

        public void Dispose(bool keepProp = false)
        {
            Delete(keepProp);
            GlobalAnimatePropList.Remove(this);
        }

        public void Dispose()
        {
            Delete(false);
            GlobalAnimatePropList.Remove(this);
        }

        public static implicit operator Prop(AnimateProp animateProp)
        {
            return animateProp.Prop;
        }

        public static implicit operator Entity(AnimateProp animateProp)
        {
            return animateProp.Prop;
        }

        public AnimationSettings this[AnimationType animationType] => Animation[animationType];
    }
}