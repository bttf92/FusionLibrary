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

    public class AnimateProp : IDisposable
    {
        /// <summary>
        /// This event is fired up when an <see cref="Animation"/> is completed.
        /// </summary>
        public event OnAnimCompleted OnAnimCompleted;

        internal static List<AnimateProp> GlobalAnimatePropList = new List<AnimateProp>();

        internal static void TickAll()
        {
            for (int i = 0; i < GlobalAnimatePropList.Count; i++)
            {
                GlobalAnimatePropList[i].Tick();
            }
        }

        /// <summary>
        /// <see cref="GTA.Entity"/> at which this <see cref="AnimateProp"/> is attached to.
        /// </summary>
        public Entity Entity { get; protected set; }

        /// <summary>
        /// Whether any animation of this <see cref="AnimateProp"/> is playing.
        /// </summary>
        public bool IsPlaying { get; private set; }

        /// <summary>
        /// The underlying <see cref="GTA.Prop"/> of this <see cref="AnimateProp"/>.
        /// </summary>
        public Prop Prop { get; private set; }

        /// <summary>
        /// The <see cref="CustomModel"/> of the <see cref="Prop"/>.
        /// </summary>
        public CustomModel Model { get; private set; }

        /// <summary>
        /// Whether use physical attach type.
        /// </summary>
        public bool UsePhysicalAttach { get; set; }

        /// <summary>
        /// If <see langword="false"/> the <see cref="Entity"/> vector is ignored.
        /// </summary>
        public bool UseFixedRot { get; set; } = true;

        /// <summary>
        /// If value is > 0 then prop will be deleted after specified time (in milliseconds).
        /// </summary>
        public float Duration { get; set; } = 0;

        /// <summary>
        /// Whether the <see cref="Prop"/> is spawned or not.
        /// </summary>
        public bool IsSpawned { get; private set; }

        /// <summary>
        /// Original offset of the <see cref="Prop"/>.
        /// </summary>
        public Vector3 Offset { get; }

        /// <summary>
        /// Original rotation of the <see cref="Prop"/>.
        /// </summary>
        public Vector3 Rotation { get; }

        /// <summary>
        /// Second offset of the <see cref="Prop"/>.
        /// </summary>
        public Vector3 SecondOffset = Vector3.Zero;

        /// <summary>
        /// Second rotation of the <see cref="Prop"/>.
        /// </summary>
        public Vector3 SecondRotation = Vector3.Zero;

        /// <summary>
        /// <see cref="Offset"/> + <see cref="SecondOffset"/>.
        /// </summary>
        public Vector3 CurrentOffset => Offset + SecondOffset;

        /// <summary>
        /// <see cref="Rotation"/> + <see cref="SecondRotation"/>.
        /// </summary>
        public Vector3 CurrentRotation => Rotation + SecondRotation;

        /// <summary>
        /// Current playing <see cref="FusionEnums.AnimationStep"/>.
        /// </summary>
        public AnimationStep AnimationStep { get; private set; } = AnimationStep.Off;

        /// <summary>
        /// Whether this <see cref="AnimateProp"/> is detached from <see cref="Entity"/>.
        /// </summary>
        public bool IsDetached { get; private set; }

        /// <summary>
        /// <see cref="FusionLibrary.Animation"/> of this <see cref="AnimateProp"/>.
        /// </summary>
        public Animation Animation { get; private set; } = new Animation();

        /// <summary>
        /// Saved <see cref="FusionLibrary.Animation"/> by <see cref="SaveAnimation"/>.
        /// </summary>
        public Animation SavedAnimation { get; private set; } = new Animation();

        /// <summary>
        /// Saved <see cref="SecondOffset"/> by <see cref="SaveAnimation"/>.
        /// </summary>
        public Vector3 SavedOffset { get; private set; } = new Vector3();

        /// <summary>
        /// Saved <see cref="SecondRotation"/> by <see cref="SaveAnimation"/>.
        /// </summary>
        public Vector3 SavedRotation { get; private set; } = new Vector3();

        /// <summary>
        /// Whether next <see cref="FusionEnums.AnimationStep"/>s should be automatically played.
        /// </summary>
        public bool PlayNextSteps { get; set; }

        /// <summary>
        /// Whether <see cref="Animation"/> should be played in reverse after first playing.
        /// </summary>
        public bool PlayReverse { get; set; }

        /// <summary>
        /// Whether <see cref="Prop"/> keeps collision when attached to <see cref="Entity"/>.
        /// </summary>
        public bool KeepCollision { get; set; } = true;

        /// <summary>
        /// If <see cref="Visible"/> is set to <see langword="false"/>, the <see cref="Prop"/> will be deleted.
        /// </summary>
        public bool UseDeleteInsteadOfHide { get; set; } = false;

        /// <summary>
        /// Can be anything.
        /// </summary>
        public object Tag { get; set; }

        private float _currentTime = 0;
        private AnimationStep _lastStep;
        private bool _playReverse;
        private bool _toBone;
        private EntityBone _bone;

        /// <summary>
        /// Creates a new <see cref="AnimateProp"/> instance.
        /// </summary>
        /// <param name="model"><see cref="CustomModel"/> of the <see cref="GTA.Prop"/>.</param>
        /// <param name="entity"><see cref="GTA.Entity"/> at which the <see cref="GTA.Prop"/> will be attached.</param>
        /// <param name="entityBone"><see cref="EntityBone"/> of <paramref name="entity"/>.</param>
        /// <param name="offset">Offset relative to <paramref name="entityBone"/>. Default <see cref="Vector3.Zero"/>.</param>
        /// <param name="rotation">Rotation relative to <paramref name="entityBone"/>. Default <see cref="Vector3.Zero"/>.</param>
        /// <param name="keepCollision">Whether keep collision when attached to <paramref name="entity"/>.</param>
        public AnimateProp(CustomModel model, Entity entity, EntityBone entityBone, Vector3 offset = default, Vector3 rotation = default, bool keepCollision = true)
        {
            Model = model;
            Entity = entity;
            _bone = entityBone;
            Offset = offset;
            Rotation = rotation;
            _toBone = true;
            KeepCollision = keepCollision;

            GlobalAnimatePropList.Add(this);
        }

        /// <summary>
        /// Creates a new <see cref="AnimateProp"/> instance.
        /// </summary>
        /// <param name="model"><see cref="CustomModel"/> of the <see cref="GTA.Prop"/>.</param>
        /// <param name="entity"><see cref="GTA.Entity"/> at which the <see cref="GTA.Prop"/> will be attached.</param>
        /// <param name="boneName">Bone's name of <paramref name="entity"/>.</param>
        /// <param name="offset">Offset relative to <paramref name="boneName"/>. Default <see cref="Vector3.Zero"/>.</param>
        /// <param name="rotation">Rotation relative to <paramref name="boneName"/>. Default <see cref="Vector3.Zero"/>.</param>
        /// <param name="keepCollision">>Whether keep collision when attached to <paramref name="entity"/>.</param>
        public AnimateProp(CustomModel model, Entity entity, string boneName, Vector3 offset = default, Vector3 rotation = default, bool keepCollision = true) : this(model, entity, entity.Bones[boneName], offset, rotation, keepCollision)
        {

        }

        /// <summary>
        /// Creates a new <see cref="AnimateProp"/> instance.
        /// </summary>
        /// <param name="model"><see cref="CustomModel"/> of the <see cref="GTA.Prop"/>.</param>
        /// <param name="entity"><see cref="GTA.Entity"/> at which the <see cref="GTA.Prop"/> will be attached.</param>
        /// <param name="offset">Offset relative to <paramref name="entity"/>. Default <see cref="Vector3.Zero"/>.</param>
        /// <param name="rotation">Rotation relative to <paramref name="entity"/>. Default <see cref="Vector3.Zero"/>.</param>
        /// <param name="keepCollision">>Whether keep collision when attached to <paramref name="entity"/>.</param>
        public AnimateProp(CustomModel model, Entity entity, Vector3 offset = default, Vector3 rotation = default, bool keepCollision = true)
        {
            Model = model;
            Entity = entity;
            Offset = offset;
            Rotation = rotation;
            KeepCollision = keepCollision;

            GlobalAnimatePropList.Add(this);
        }

        /// <summary>
        /// Whether the <see cref="Prop"/> is visible or not.
        /// </summary>
        public bool Visible
        {
            get
            {
                if (Prop.NotNullAndExists())
                {
                    return Prop.IsVisible;
                }

                return false;
            }
            set
            {
                if (Prop.NotNullAndExists())
                {
                    if (UseDeleteInsteadOfHide && !value)
                        Delete();
                    else
                        Prop.IsVisible = value;
                }
                else if (value)
                {
                    SpawnProp();
                }
            }
        }

        /// <summary>
        /// Gets the position relative to the <see cref="Entity"/>.
        /// </summary>
        public Vector3 RelativePosition => _toBone ? _bone.GetRelativeOffsetPosition(CurrentOffset) : CurrentOffset;

        /// <summary>
        /// Gets the position in world coordinates.
        /// </summary>
        public Vector3 Position => _toBone ? _bone.GetOffsetPosition(CurrentOffset) : Entity.GetOffsetPosition(CurrentOffset);

        /// <summary>
        /// Gets the position in world coordinates of the <see cref="Prop"/>.
        /// </summary>
        public Vector3 WorldPosition
        {
            get
            {
                if (!Prop.NotNullAndExists())
                {
                    return Vector3.Zero;
                }

                return Prop.Position;
            }
        }

        /// <summary>
        /// Saves the current <see cref="Animation"/>, <see cref="SecondOffset"/> and <see cref="SecondRotation"/>.
        /// </summary>
        public void SaveAnimation()
        {
            SavedAnimation = Animation.Clone();
            SavedOffset = SecondOffset;
            SavedRotation = SecondRotation;
        }

        /// <summary>
        /// Restores the <see cref="SavedAnimation"/>, <see cref="SavedOffset"/> and <see cref="SecondRotation"/>.
        /// </summary>
        public void RestoreAnimation()
        {
            Animation = SavedAnimation.Clone();
            SecondOffset = SavedOffset;
            SecondRotation = SavedRotation;

            Attach();
        }

        /// <summary>
        /// Sets the specified <see cref="Coordinate"/> of the offset to <paramref name="value"/>.
        /// </summary>
        /// <param name="coordinate">Wanted <see cref="Coordinate"/></param>
        /// <param name="value">Value of the <paramref name="coordinate"/>.</param>
        /// <param name="isCurrent">If <see langword="true"/> is applied to <see cref="CurrentOffset"/> otherwise to <see cref="SecondOffset"/>.</param>
        public void SetOffset(Coordinate coordinate, float value, bool isCurrent = false)
        {
            if (isCurrent)
            {
                SecondOffset[(int)coordinate] = value - Offset[(int)coordinate];
            }
            else
            {
                SecondOffset[(int)coordinate] = value;
            }
        }

        /// <summary>
        /// Sets the offset.
        /// </summary>
        /// <param name="value">New value.</param>
        /// <param name="isCurrent">if <see langword="true"/> is applied to <see cref="CurrentOffset"/> otherwise to <see cref="SecondOffset"/>.</param>
        public void SetOffset(Vector3 value, bool isCurrent = false)
        {
            if (isCurrent)
            {
                SecondOffset = value - Offset;
            }
            else
            {
                SecondOffset = value;
            }
        }

        /// <summary>
        /// Sets the specified <see cref="Coordinate"/> of the rotation to <paramref name="value"/>.
        /// </summary>
        /// <param name="coordinate">Wanted <see cref="Coordinate"/></param>
        /// <param name="value">Value of the <paramref name="coordinate"/>.</param>
        /// <param name="isCurrent">If <see langword="true"/> is applied to <see cref="CurrentRotation"/> otherwise to <see cref="SecondRotation"/>.</param>
        public void SetRotation(Coordinate coordinate, float value, bool isCurrent = false)
        {
            if (isCurrent)
            {
                SecondRotation[(int)coordinate] = value - Rotation[(int)coordinate];
            }
            else
            {
                SecondRotation[(int)coordinate] = value;
            }
        }

        /// <summary>
        /// Sets the rotation.
        /// </summary>
        /// <param name="value">New value.</param>
        /// <param name="isCurrent">If <see langword="true"/> is applied to <see cref="CurrentRotation"/> otherwise to <see cref="SecondRotation"/>.</param>
        public void SetRotation(Vector3 value, bool isCurrent = false)
        {
            if (isCurrent)
            {
                SecondRotation = value - Rotation;
            }
            else
            {
                SecondRotation = value;
            }
        }

        /// <summary>
        /// Moves the prop to new offset and rotation.
        /// </summary>
        /// <param name="offset">New offset.</param>
        /// <param name="rotation">New rotation.</param>
        /// <param name="isCurrent">If <see langword="true"/> is applied to "Current" offset and rotation otherwise to "Second".</param>
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
            {
                SpawnProp();
            }
        }

        /// <summary>
        /// Swaps the <see cref="CustomModel"/> of the <see cref="Prop"/> and respawns it if it was already spawned.
        /// </summary>
        /// <param name="model">New <see cref="CustomModel"/>.</param>
        public void SwapModel(CustomModel model)
        {
            Model = model;
            Prop?.Delete();

            if (Visible)
            {
                SpawnProp();
            }
        }

        /// <summary>
        /// Transfers <see cref="Prop"/> to <paramref name="entity"/>.
        /// </summary>
        /// <param name="entity">New <see cref="GTA.Entity"/> instance.</param>
        public void TransferTo(Entity entity)
        {
            Entity = entity;
            _toBone = false;
            Attach();
        }

        /// <summary>
        /// Transfers <see cref="Prop"/> to <paramref name="boneName"/> of <paramref name="entity"/>.
        /// </summary>
        /// <param name="entity">New <see cref="GTA.Entity"/> instance.</param>
        /// <param name="boneName">Bone's name.</param>
        public void TransferTo(Entity entity, string boneName)
        {
            Entity = entity;
            _bone = entity.Bones[boneName];
            _toBone = true;
            TransferTo(entity);
        }

        /// <summary>
        /// Transfers <see cref="Prop"/> to <paramref name="entityBone"/> of <paramref name="entity"/>.
        /// </summary>
        /// <param name="entity">New <see cref="GTA.Entity"/> instance.</param>
        /// <param name="entityBone"><see cref="EntityBone"/> of <paramref name="entity"/>.</param>
        public void TransferTo(Entity entity, EntityBone entityBone)
        {
            Entity = entity;
            _bone = entityBone;
            _toBone = true;
            TransferTo(entity);
        }

        /// <summary>
        /// Toggles <see cref="Prop"/> spawn status.
        /// </summary>
        /// <param name="state"><see langword="true"/> prop is spawned; otherwise deleted.</param>
        public void SetState(bool state)
        {
            if (state)
            {
                SpawnProp();
            }
            else
            {
                Delete();
            }
        }

        /// <summary>
        /// Sets the specified <paramref name="coordinate"/> to the maximum or minimum value.
        /// </summary>
        /// <param name="animationType">Wanted <see cref="FusionEnums.AnimationType"/>.</param>
        /// <param name="animationStep"><see cref="FusionEnums.AnimationStep"/> of <paramref name="animationType"/>.</param>
        /// <param name="coordinate"><see cref="FusionEnums.Coordinate"/> of <paramref name="animationStep"/>.</param>
        /// <param name="maximum"><see langword="true"/> sets <paramref name="coordinate"/> to maximum value; otherwise minimum.</param>
        public void SetCoordinateAt(AnimationType animationType, AnimationStep animationStep, Coordinate coordinate, bool maximum)
        {
            CoordinateSetting coordinateSetting = Animation[animationType][animationStep][coordinate];

            if (animationType == AnimationType.Offset)
            {
                SecondOffset[(int)coordinate] = (maximum ? coordinateSetting.Maximum : coordinateSetting.Minimum) * coordinateSetting.MaxMinRatio - Offset[(int)coordinate];
            }
            else
            {
                SecondRotation[(int)coordinate] = (maximum ? coordinateSetting.Maximum : coordinateSetting.Minimum) * coordinateSetting.MaxMinRatio - Rotation[(int)coordinate];
            }

            Attach();
        }

        /// <summary>
        /// Plays specified <paramref name="animationStep"/> instantly.
        /// </summary>
        /// <param name="animationStep">Wanted <see cref="FusionEnums.AnimationStep"/>.</param>
        public void SetInstantAnimationStep(AnimationStep animationStep)
        {
            List<CoordinateSetting> offsetSettings = Animation[AnimationType.Offset][animationStep].CoordinateSettings.Where(x => x.IsSetted).ToList();
            List<CoordinateSetting> rotationSettings = Animation[AnimationType.Rotation][animationStep].CoordinateSettings.Where(x => x.IsSetted).ToList();

            offsetSettings.ForEach(x =>
            {
                float val;

                if (x.IsIncreasing)
                {
                    val = x.Maximum * x.MaxMinRatio;
                }
                else
                {
                    val = x.Minimum * x.MaxMinRatio;
                }

                x.IsIncreasing = !x.IsIncreasing;

                SecondOffset[(int)x.Coordinate] = val - Offset[(int)x.Coordinate];
            });

            rotationSettings.ForEach(x =>
            {
                float val;

                if (x.IsIncreasing)
                {
                    val = x.Maximum * x.MaxMinRatio;
                }
                else
                {
                    val = x.Minimum * x.MaxMinRatio;
                }

                x.IsIncreasing = !x.IsIncreasing;

                SecondRotation[(int)x.Coordinate] = val - Rotation[(int)x.Coordinate];
            });

            Attach();
        }

        /// <summary>
        /// Starts <see cref="Animation"/> of this <see cref="AnimateProp"/>.
        /// </summary>
        public void Play()
        {
            Play(_playReverse ? _lastStep : AnimationStep.First);
        }

        /// <summary>
        /// Starts <see cref="Animation"/> of this <see cref="AnimateProp"/>.
        /// </summary>
        /// <param name="instant">Whether play the animation instant.</param>
        /// <param name="spawnAndRestore">Whether restore the <see cref="SavedAnimation"/> before playing.</param>
        public void Play(bool instant = false, bool spawnAndRestore = false)
        {
            Play(_playReverse ? _lastStep : AnimationStep.First, instant, false, spawnAndRestore);
        }

        /// <summary>
        /// Starts <see cref="Animation"/> of this <see cref="AnimateProp"/>.
        /// </summary>
        /// <param name="animationStep"><see cref="FusionEnums.AnimationStep"/> to be played.</param>
        /// <param name="instant">Whether play the animation instant.</param>
        /// <param name="playInstantPreviousSteps">Whether play previous steps.</param>
        /// <param name="spawnAndRestore">Whether restore the <see cref="SavedAnimation"/> before playing.</param>
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
                {
                    SetInstantAnimationStep(prevStep);
                }
            }

            if (instant)
            {
                SetInstantAnimationStep(animationStep);
                return;
            }

            Animation[AnimationType.Offset][animationStep].SetAllUpdate(true);
            Animation[AnimationType.Rotation][animationStep].SetAllUpdate(true);
            AnimationStep = animationStep;
            IsPlaying = true;
        }

        internal void Tick()
        {
            if (!IsSpawned)
            {
                return;
            }

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
            {
                return;
            }

            if (IsPlaying)
            {
                List<CoordinateSetting> offsetSettings = Animation[AnimationType.Offset][AnimationStep].CoordinateSettings.Where(x => x.IsSetted && x.Update).ToList();
                List<CoordinateSetting> rotationSettings = Animation[AnimationType.Rotation][AnimationStep].CoordinateSettings.Where(x => x.IsSetted && x.Update).ToList();

                offsetSettings.ForEach(x => UpdateCoordinate(x));
                rotationSettings.ForEach(x => UpdateCoordinate(x));

                Attach();

                if (rotationSettings.Count == 0 && offsetSettings.Count == 0)
                {
                    if (PlayNextSteps && ((!_playReverse && AnimationStep != AnimationStep.Seventh) || (_playReverse && AnimationStep != AnimationStep.First)))
                    {
                        AnimationStep nextStep = AnimationStep;

                        if (_playReverse)
                        {
                            nextStep--;
                        }
                        else
                        {
                            nextStep++;
                        }

                        Animation[AnimationType.Offset][nextStep].SetAllUpdate(true);
                        Animation[AnimationType.Rotation][nextStep].SetAllUpdate(true);
                        AnimationStep = nextStep;

                        return;
                    }

                    IsPlaying = false;

                    if (PlayNextSteps && PlayReverse)
                    {
                        _playReverse = !_playReverse;
                        _lastStep = AnimationStep;
                    }

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
            {
                return;
            }

            int i = (int)coordinateSetting.Coordinate;

            float current;

            if (coordinateSetting.Type == AnimationType.Offset)
            {
                current = CurrentOffset[i];
            }
            else
            {
                current = CurrentRotation[i];
            }

            float newValue = coordinateSetting.UpdateValue(current);

            if (coordinateSetting.Type == AnimationType.Offset)
            {
                SecondOffset[i] = newValue - Offset[i];
            }
            else
            {
                SecondRotation[i] = newValue - Rotation[i];
            }
        }

        /// <summary>
        /// Spawns the <see cref="Prop"/>.
        /// </summary>
        public void SpawnProp()
        {
            if (Prop.NotNullAndExists())
            {
                IsSpawned = true;
                return;
            }

            if (!Entity.NotNullAndExists())
            {
                return;
            }

            Prop = World.CreateProp(Model, Entity.Position, false, false);
            Prop.ApplyForce(Vector3.UnitX);
            Prop.IsPersistent = true;

            IsSpawned = true;

            Attach();
        }

        private void Attach()
        {
            if (!Prop.NotNullAndExists() || !Entity.NotNullAndExists())
            {
                return;
            }

            if (_toBone)
            {
                if (UsePhysicalAttach)
                {
                    Function.Call(Hash.ATTACH_ENTITY_TO_ENTITY_PHYSICALLY, Prop.Handle, Entity.Handle, 0, _bone.Index, CurrentOffset.X, CurrentOffset.Y, CurrentOffset.Z, 0, 0, 0, CurrentRotation.X, CurrentRotation.Y, CurrentRotation.Z, 1000000.0f, UseFixedRot, true, true, true, 2);
                }
                else
                {
                    Function.Call(Hash.ATTACH_ENTITY_TO_ENTITY, Prop.Handle, Entity.Handle, _bone.Index, CurrentOffset.X, CurrentOffset.Y, CurrentOffset.Z, CurrentRotation.X, CurrentRotation.Y, CurrentRotation.Z, false, false, KeepCollision, false, 0, UseFixedRot);
                }
            }
            else
            {
                if (UsePhysicalAttach)
                {
                    Function.Call(Hash.ATTACH_ENTITY_TO_ENTITY_PHYSICALLY, Prop.Handle, Entity.Handle, 0, 0, CurrentOffset.X, CurrentOffset.Y, CurrentOffset.Z, 0, 0, 0, CurrentRotation.X, CurrentRotation.Y, CurrentRotation.Z, 1000000.0f, UseFixedRot, true, true, true, 2);
                }
                else
                {
                    Function.Call(Hash.ATTACH_ENTITY_TO_ENTITY, Prop.Handle, Entity.Handle, 0, CurrentOffset.X, CurrentOffset.Y, CurrentOffset.Z, CurrentRotation.X, CurrentRotation.Y, CurrentRotation.Z, false, false, KeepCollision, false, 0, UseFixedRot);
                }
            }
        }

        /// <summary>
        /// Deletes the <see cref="Prop"/>.
        /// </summary>
        /// <param name="keepProp">Whether keep the <see cref="Prop"/> in the world.</param>
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
            {
                Prop?.Delete();
            }
        }

        /// <summary>
        /// Detaches the <see cref="Prop"/> from <see cref="Entity"/>.
        /// </summary>
        public void Detach()
        {
            Prop.Detach();
            Prop.IsPositionFrozen = false;
            IsDetached = true;
        }

        /// <summary>
        /// Detaches and scatters the <see cref="Prop"/> from <see cref="Entity"/>.
        /// </summary>
        /// <param name="ForceMultiplier">Force value to be applied. Default <c>1</c>.</param>
        public void ScatterProp(float ForceMultiplier = 1f)
        {
            Detach();
            Prop.ApplyForce(Vector3.RandomXYZ() * ForceMultiplier, Vector3.RandomXYZ() * ForceMultiplier);
        }

        /// <summary>
        /// Stops the current playing <see cref="Animation"/>.
        /// </summary>
        public void Stop()
        {
            Animation[AnimationType.Offset][AnimationStep].SetAllUpdate(false);
            Animation[AnimationType.Rotation][AnimationStep].SetAllUpdate(false);

            AnimationStep = AnimationStep.Off;
            IsPlaying = false;
        }

        /// <summary>
        /// Disposes the <see cref="Prop"/>.
        /// </summary>
        /// <param name="keepProp">>Whether keep the <see cref="Prop"/> in the world.</param>
        public void Dispose(bool keepProp = false)
        {
            Delete(keepProp);
            GlobalAnimatePropList.Remove(this);
        }

        /// <summary>
        /// Disposes the <see cref="Prop"/>.
        /// </summary>
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

        public AnimationTypeSettings this[AnimationType animationType] => Animation[animationType];
    }
}