using System;
using FusionLibrary.Extensions;
using GTA;
using GTA.Math;
using GTA.Native;
using static FusionLibrary.Enums;

namespace FusionLibrary
{
    public delegate void AnimationStopped(AnimateProp animateProp, Coordinate coordinate, CoordinateSetting coordinateSetting, bool IsRotation);    

    public class AnimateProp
    {
        public AnimationStopped AnimationStopped;

        public Prop Prop { get; private set; }
        public Model Model { get; set; }
        private EntityBone Bone;
        public Entity Entity { get; set; }
        private Vector3 pOffset;
        private Vector3 pRotation;
        public bool IsAnimationOn;
        private CoordinateSetting[] cPos = new CoordinateSetting[4];
        private CoordinateSetting[] cRot = new CoordinateSetting[4];
        private bool ToBone;
        private bool IsDetached = false;
        private float _currentTime = 0;

        public bool IsSpawned => Prop.NotNullAndExists();

        public float Duration { get; set; } = 0;

        /// <summary>
        /// Spawns a new prop with <paramref name="pModel"/> attached to <paramref name="boneName"/> of <paramref name="pEntity"/> with <paramref name="pOffset"/> and <paramref name="pRotation"/>
        /// </summary>
        /// <param name="pModel"><seealso cref="GTA.Model"/> of the prop.</param>
        /// <param name="pEntity"><seealso cref="GTA.Entity"/> owner of the <paramref name="boneName"/>.</param>
        /// <param name="boneName">Bone's name of the <paramref name="pEntity"/>.</param>
        /// <param name="pOffset">A <seealso cref="GTA.Vector3"/> indicating offset of the prop relative to the <paramref name="boneName"/>'s position.</param>
        /// <param name="pRotation">A <seealso cref="GTA.Vector3"/> indicating rotation of the prop.</param>
        public AnimateProp(Model pModel, Entity pEntity, string boneName, Vector3 pOffset, Vector3 pRotation, bool cAnimationOn = false, bool doNotSpawn = false)
        {
            this.Model = pModel;
            this.Entity = pEntity;
            Bone = pEntity.Bones[boneName];
            this.pOffset = pOffset;
            this.pRotation = pRotation;
            ToBone = true;
            IsAnimationOn = cAnimationOn;

            if (!doNotSpawn)
                SpawnProp();

            AnimatePropsHandler.GlobalPropsList.Add(this);
        }

        public AnimateProp(Model pModel, Entity pEntity, Vector3 pOffset, Vector3 pRotation, bool cAnimationOn = false, bool doNotSpawn = false)
        {
            this.Model = pModel;
            this.Entity = pEntity;            
            this.pOffset = pOffset;
            this.pRotation = pRotation;
            ToBone = false;
            IsAnimationOn = cAnimationOn;

            if (!doNotSpawn)
                SpawnProp();

            AnimatePropsHandler.GlobalPropsList.Add(this);
        }

        public AnimateProp(Entity pEntity, Model pModel, Vector3 pOffset, Vector3 pRotation) : this(pModel, pEntity, pOffset, pRotation, false, true)
        {

        }

        public AnimateProp(Entity pEntity, Model pModel, string boneName) : this(pModel, pEntity, boneName, Vector3.Zero, Vector3.Zero, false, true)
        {

        }

        public void SpawnProp(bool deletePreviousProp = true)
        {
            if (deletePreviousProp)
                Delete();

            if (!IsSpawned)
            {
                Utils.LoadAndRequestModel(Model);
                Prop = World.CreateProp(Model, Entity.Position, false, false);
                Prop.IsPersistent = true;
            }

            Attach();
        }

        public void MoveProp(Vector3 pOffset, Vector3 pRotation, bool deletePreviousProp = true)
        {
            if (deletePreviousProp)
                Delete();

            Attach(this.pOffset + pOffset, this.pRotation + pRotation);
        }

        private void Attach()
        {
            if (IsDetached)
                return;

            if (!IsSpawned)
                SpawnProp();

            if (ToBone)
            {
                Function.Call(Hash.ATTACH_ENTITY_TO_ENTITY, Prop.Handle, Entity.Handle, Bone.Index, pOffset.X, pOffset.Y, pOffset.Z, pRotation.X, pRotation.Y, pRotation.Z, false, false, true, false, 2, true);
            }
            else
            {
                Function.Call(Hash.ATTACH_ENTITY_TO_ENTITY, Prop.Handle, Entity.Handle, 0, pOffset.X, pOffset.Y, pOffset.Z, pRotation.X, pRotation.Y, pRotation.Z, false, false, true, false, 2, true);
            }
        }

        private void Attach(Vector3 pOffset, Vector3 pRotation)
        {
            if (IsDetached)
                return;

            if (!IsSpawned)
                SpawnProp();

            if (ToBone)
            {
                Function.Call(Hash.ATTACH_ENTITY_TO_ENTITY, Prop.Handle, Entity.Handle, Bone.Index, pOffset.X, pOffset.Y, pOffset.Z, pRotation.X, pRotation.Y, pRotation.Z, false, false, true, false, 2, true);
            }
            else
            {
                Function.Call(Hash.ATTACH_ENTITY_TO_ENTITY, Prop.Handle, Entity.Handle, 0, pOffset.X, pOffset.Y, pOffset.Z, pRotation.X, pRotation.Y, pRotation.Z, false, false, true, false, 2, true);
            }
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

        public void TransferTo(Entity newEntity, string boneName)
        {
            Entity = newEntity;
            Bone = Entity.Bones[boneName];
            Attach();
        }

        public void TransferTo(Entity newEntity)
        {
            Entity = newEntity;
            Attach();
        }

        public void SwapModel(Model model)
        {
            Model = model;
            
            if (IsSpawned)
            {
                Delete();
                SpawnProp();
            }
        }

        public void SetState(bool state)
        {
            if (state)
                SpawnProp();
            else
                Delete();
        }

        /// <summary>
        /// Deletes prop.
        /// </summary>
        public void Delete()
        {
            if (IsSpawned)
                Prop?.Delete();

            _currentTime = 0;
        }

        public void Dispose()
        {
            Delete();
            AnimatePropsHandler.GlobalPropsList.Remove(this);
        }

        /// <summary>
        /// Gets or sets the visbile status of the prop.
        /// </summary>
        /// <returns></returns>
        public bool Visible
        {
            get
            {
                return Prop.IsVisible;
            }

            set
            {
                Prop.IsVisible = value;
            }
        }

        /// <summary>
        /// Gets or sets offset of the prop.
        /// </summary>
        /// <returns></returns>
        public Vector3 Offset
        {
            get
            {
                return pOffset;
            }

            set
            {
                pOffset = value;
                Attach();
            }
        }

        /// <summary>
        /// Gets or sets rotation of the prop.
        /// </summary>
        /// <returns></returns>
        public Vector3 Rotation
        {
            get
            {
                return pRotation;
            }

            set
            {
                pRotation = value;
                Attach();
            }
        }

        /// <summary>
        /// Sets given position's coordinate settings.
        /// </summary>
        /// <param name="pCord">Coordinate (X, Y, Z)</param>
        /// <param name="cUpdate">If true, the coordinate value will update.</param>
        /// <param name="cIncreasing">If the coordinate value should increase or not.</param>
        /// <param name="cMinimum">Minimum value should reach.</param>
        /// <param name="cMaximum">Maximum value should reach.</param>
        /// <param name="cStep">Delta value that is added or substract.</param>
        /// <param name="cStepRatio">From 0.0 to 1.0. Ratio of maximum and minimum values.</param>
        public void setPositionSettings(Coordinate pCord, bool cUpdate, bool cIncreasing, float cMinimum, float cMaximum, float cStep, float cStepRatio = 1f, bool Stop = false, float cMaxMinRatio = 1f)
        {
            cPos[(int)pCord].Update = cUpdate;
            cPos[(int)pCord].isIncreasing = cIncreasing;
            cPos[(int)pCord].Minimum = cMinimum;
            cPos[(int)pCord].Maximum = cMaximum;
            cPos[(int)pCord].Step = cStep;
            cPos[(int)pCord].StepRatio = cStepRatio;
            cPos[(int)pCord].isFullCircle = false;
            cPos[(int)pCord].Stop = Stop;
            cPos[(int)pCord].MaxMinRatio = cMaxMinRatio;
        }

        /// <summary>
        /// Sets given rotation's coordinate settings.
        /// </summary>
        /// <param name="pCord">Coordinate (X, Y, Z)</param>
        /// <param name="cUpdate">If true, the coordinate value will update.</param>
        /// <param name="cIncreasing">If the coordinate value should increase or not.</param>
        /// <param name="cMinimum">Minimum value should reach.</param>
        /// <param name="cMaximum">Maximum value should reach.</param>
        /// <param name="cStep">Delta value that is added or substract.</param>
        /// <param name="cFullCircle">If true disables maximum\minimum and the prop will continue to rotate indefinitely.</param>
        /// <param name="cStepRatio">From 0.0 to 1.0. Ratio of maximum and minimum values.</param>
        public void setRotationSettings(Coordinate pCord, bool cUpdate, bool cIncreasing, float cMinimum, float cMaximum, float cStep, bool cFullCircle, float cStepRatio = 1f, bool Stop = false, float cMaxMinRatio = 1f)
        {
            cRot[(int)pCord].Update = cUpdate;
            cRot[(int)pCord].isIncreasing = cIncreasing;
            cRot[(int)pCord].Minimum = cMinimum;
            cRot[(int)pCord].Maximum = cMaximum;
            cRot[(int)pCord].Step = cStep;
            cRot[(int)pCord].StepRatio = cStepRatio;
            cRot[(int)pCord].isFullCircle = cFullCircle;
            cRot[(int)pCord].Stop = Stop;
            cRot[(int)pCord].MaxMinRatio = cMaxMinRatio;
        }

        /* TODO ERROR: Skipped RegionDirectiveTrivia */
        public Vector3 RelativePosition
        {
            get
            {
                return Bone.GetRelativeOffsetPosition(pOffset);
            }
        }

        public Vector3 WorldPosition
        {
            get
            {
                return Bone.GetOffsetPosition(pOffset);
            }
        }

        public float get_Position(Coordinate pCord)
        {
            switch (pCord)
            {
                case Coordinate.X:
                    {
                        return pOffset.X;
                    }

                case Coordinate.Y:
                    {
                        return pOffset.Y;
                    }

                default:
                    {
                        return pOffset.Z;
                    }
            }
        }

        public void set_Position(Coordinate pCord, float value)
        {
            switch (pCord)
            {
                case Coordinate.X:
                    {
                        pOffset.X = value;
                        break;
                    }

                case Coordinate.Y:
                    {
                        pOffset.Y = value;
                        break;
                    }

                default:
                    {
                        pOffset.Z = value;
                        break;
                    }
            }

            Attach();
        }

        public bool get_PositionUpdate(Coordinate pCord)
        {
            return cPos[(int)pCord].Update;
        }

        public void set_PositionUpdate(Coordinate pCord, bool value)
        {
            cPos[(int)pCord].Update = value;
        }

        public float get_PositionMaximum(Coordinate pCord)
        {
            return cPos[(int)pCord].Maximum;
        }

        public void set_PositionMaximum(Coordinate pCord, float value)
        {
            cPos[(int)pCord].Maximum = value;
        }

        public float get_PositionMinimum(Coordinate pCord)
        {
            return cPos[(int)pCord].Minimum;
        }

        public void set_PositionMinimum(Coordinate pCord, float value)
        {
            cPos[(int)pCord].Minimum = value;
        }

        public float get_PositionStep(Coordinate pCord)
        {
            return cPos[(int)pCord].Step;
        }

        public void set_PositionStep(Coordinate pCord, float value)
        {
            cPos[(int)pCord].Step = value;
        }

        public float get_PositionStepRatio(Coordinate pCord)
        {
            return cPos[(int)pCord].StepRatio;
        }

        public void set_PositionStepRatio(Coordinate pCord, float value)
        {
            cPos[(int)pCord].StepRatio = value;
        }

        public bool get_PositionIncreasing(Coordinate pCord)
        {
            return cPos[(int)pCord].isIncreasing;
        }

        public void set_PositionIncreasing(Coordinate pCord, bool value)
        {
            cPos[(int)pCord].isIncreasing = value;
        }

        public bool get_PositionStop(Coordinate pCord)
        {
            return cPos[(int)pCord].Stop;
        }

        public void set_PositionStop(Coordinate pCord, bool value)
        {
            cPos[(int)pCord].Stop = value;
        }

        public float get_PositionMaxMinRatio(Coordinate pCord)
        {
            return cPos[(int)pCord].MaxMinRatio;
        }

        public void set_PositionMaxMinRatio(Coordinate pCord, float value)
        {
            cPos[(int)pCord].MaxMinRatio = value;
        }
        /* TODO ERROR: Skipped EndRegionDirectiveTrivia */
        /* TODO ERROR: Skipped RegionDirectiveTrivia */
        public float get_Rotation(Coordinate pCord)
        {
            switch (pCord)
            {
                case Coordinate.X:
                    {
                        return pRotation.X;
                    }

                case Coordinate.Y:
                    {
                        return pRotation.Y;
                    }

                default:
                    {
                        return pRotation.Z;
                    }
            }
        }

        public void set_Rotation(Coordinate pCord, float value)
        {
            switch (pCord)
            {
                case Coordinate.X:
                    {
                        pRotation.X = value;
                        break;
                    }

                case Coordinate.Y:
                    {
                        pRotation.Y = value;
                        break;
                    }

                default:
                    {
                        pRotation.Z = value;
                        break;
                    }
            }

            Attach();
        }

        public bool get_RotationUpdate(Coordinate pCord)
        {
            return cRot[(int)pCord].Update;
        }

        public void set_RotationUpdate(Coordinate pCord, bool value)
        {
            cRot[(int)pCord].Update = value;
        }

        public float get_RotationMaximum(Coordinate pCord)
        {
            return cRot[(int)pCord].Maximum;
        }

        public void set_RotationMaximum(Coordinate pCord, float value)
        {
            cRot[(int)pCord].Maximum = value;
        }

        public float get_RotationMinimum(Coordinate pCord)
        {
            return cRot[(int)pCord].Minimum;
        }

        public void set_RotationMinimum(Coordinate pCord, float value)
        {
            cRot[(int)pCord].Minimum = value;
        }

        public float get_RotationStep(Coordinate pCord)
        {
            return cRot[(int)pCord].Step;
        }

        public void set_RotationStep(Coordinate pCord, float value)
        {
            cRot[(int)pCord].Step = value;
        }

        public float get_RotationStepRatio(Coordinate pCord)
        {
            return cRot[(int)pCord].StepRatio;
        }

        public void set_RotationStepRatio(Coordinate pCord, float value)
        {
            cRot[(int)pCord].StepRatio = value;
        }

        public bool get_RotationIncreasing(Coordinate pCord)
        {
            return cRot[(int)pCord].isIncreasing;
        }

        public void set_RotationIncreasing(Coordinate pCord, bool value)
        {
            cRot[(int)pCord].isIncreasing = value;
        }

        public bool get_RotationFullCircle(Coordinate pCord)
        {
            return cRot[(int)pCord].isFullCircle;
        }

        public void set_RotationFullCircle(Coordinate pCord, bool value)
        {
            cRot[(int)pCord].isFullCircle = value;
        }

        public bool get_RotationStop(Coordinate pCord)
        {
            return cRot[(int)pCord].Stop;
        }

        public void set_RotationStop(Coordinate pCord, bool value)
        {
            cRot[(int)pCord].Stop = value;
        }

        public float get_RotationMaxMinRatio(Coordinate pCord)
        {
            return cRot[(int)pCord].MaxMinRatio;
        }

        public void set_RotationMaxMinRatio(Coordinate pCord, float value)
        {
            cRot[(int)pCord].MaxMinRatio = value;
        }
        
        public void CheckExists()
        {
            if (Prop.Exists() == false)
            {
                Prop = World.CreateProp(Model, Entity.Position, false, false);
                Attach();
            }
        }

        public void Play()
        {
            if (IsSpawned)
                CheckExists();
            else
                return;

            if (Duration > 0)
            {
                _currentTime += Game.LastFrameTime;

                if (_currentTime > Duration)
                {
                    Delete();
                    return;
                }
            }

            if (IsDetached || !IsAnimationOn)
                return;

            {                
                if (cPos[0].Update)
                {
                    if (cPos[0].isIncreasing)
                    {
                        pOffset.X += cPos[0].Step * cPos[0].StepRatio;

                        if (pOffset.X > cPos[0].Maximum * cPos[0].MaxMinRatio)
                        {
                            if (cPos[0].Stop) 
                            {
                                cPos[0].Update = false;
                                AnimationStopped?.Invoke(this, Coordinate.X, cPos[0], false);
                            }
                                
                            cPos[0].isIncreasing = false;
                            pOffset.X = cPos[0].Maximum * cPos[0].MaxMinRatio;
                        }
                    }
                    else
                    {
                        pOffset.X -= cPos[0].Step * cPos[0].StepRatio;

                        if (pOffset.X < cPos[0].Minimum * cPos[0].MaxMinRatio)
                        {
                            if (cPos[0].Stop)
                            {
                                cPos[0].Update = false;
                                AnimationStopped?.Invoke(this, Coordinate.X, cPos[0], false);
                            }

                            cPos[0].isIncreasing = true;
                            pOffset.X = cPos[0].Minimum * cPos[0].MaxMinRatio;
                        }
                    }
                }
            }

            {
                if (cPos[1].Update)
                {
                    if (cPos[1].isIncreasing)
                    {
                        pOffset.Y += cPos[1].Step * cPos[1].StepRatio;

                        if (pOffset.Y > cPos[1].Maximum * cPos[1].MaxMinRatio)
                        {
                            if (cPos[1].Stop)
                            {
                                cPos[1].Update = false;
                                AnimationStopped?.Invoke(this, Coordinate.Y, cPos[1], false);
                            }

                            cPos[1].isIncreasing = false;
                            pOffset.Y = cPos[1].Maximum * cPos[1].MaxMinRatio;
                        }
                    }
                    else
                    {
                        pOffset.Y -= cPos[1].Step * cPos[1].StepRatio;

                        if (pOffset.Y < cPos[1].Minimum * cPos[1].MaxMinRatio)
                        {
                            if (cPos[1].Stop)
                            {
                                cPos[1].Update = false;
                                AnimationStopped?.Invoke(this, Coordinate.Y, cPos[1], false);
                            }

                            cPos[1].isIncreasing = true;
                            pOffset.Y = cPos[1].Minimum * cPos[1].MaxMinRatio;
                        }
                    }
                }
            }

            {
                if (cPos[2].Update)
                {
                    if (cPos[2].isIncreasing)
                    {
                        pOffset.Z += cPos[2].Step * cPos[2].StepRatio;

                        if (pOffset.Z > cPos[2].Maximum * cPos[2].MaxMinRatio)
                        {
                            if (cPos[2].Stop)
                            {
                                cPos[2].Update = false;
                                AnimationStopped?.Invoke(this, Coordinate.Z, cPos[2], false);
                            }

                            cPos[2].isIncreasing = false;
                            pOffset.Z = cPos[2].Maximum * cPos[2].MaxMinRatio;
                        }
                    }
                    else
                    {
                        pOffset.Z -= cPos[2].Step * cPos[2].StepRatio;

                        if (pOffset.Z < cPos[2].Minimum * cPos[2].MaxMinRatio)
                        {
                            if (cPos[2].Stop)
                            {
                                cPos[2].Update = false;
                                AnimationStopped?.Invoke(this, Coordinate.Z, cPos[2], false);
                            }

                            cPos[2].isIncreasing = true;
                            pOffset.Z = cPos[2].Minimum * cPos[2].MaxMinRatio;
                        }
                    }
                }
            }

            {
                if (cRot[0].Update)
                {
                    if (cRot[0].isIncreasing)
                    {
                        pRotation.X += cRot[0].Step * cRot[0].StepRatio;

                        if (pRotation.X > cRot[0].Maximum * cRot[0].MaxMinRatio)
                        {
                            if (cRot[0].isFullCircle)
                            {
                                pRotation.X -= 360;
                            }
                            else
                            {
                                if (cRot[0].Stop)
                                {
                                    cRot[0].Update = false;
                                    AnimationStopped?.Invoke(this, Coordinate.X, cRot[0], true);
                                }

                                cRot[0].isIncreasing = false;
                                pRotation.X = cRot[0].Maximum * cRot[0].MaxMinRatio;
                            }
                        }
                    }
                    else
                    {
                        pRotation.X -= cRot[0].Step * cRot[0].StepRatio;

                        if (pRotation.X < cRot[0].Minimum * cRot[0].MaxMinRatio)
                        {
                            if (cRot[0].isFullCircle)
                            {
                                pRotation.X += 360;
                            }
                            else
                            {
                                if (cRot[0].Stop)
                                {
                                    cRot[0].Update = false;
                                    AnimationStopped?.Invoke(this, Coordinate.X, cRot[0], true);
                                }

                                cRot[0].isIncreasing = true;
                                pRotation.X = cRot[0].Minimum * cRot[0].MaxMinRatio;
                            }
                        }
                    }
                }
            }

            {
                if (cRot[1].Update)
                {
                    if (cRot[1].isIncreasing)
                    {
                        pRotation.Y += cRot[1].Step * cRot[1].StepRatio;

                        if (pRotation.Y > cRot[1].Maximum * cRot[1].MaxMinRatio)
                        {
                            if (cRot[1].isFullCircle)
                            {
                                pRotation.Y -= 360;
                            }
                            else
                            {
                                if (cRot[1].Stop)
                                {
                                    cRot[1].Update = false;
                                    AnimationStopped?.Invoke(this, Coordinate.Y, cRot[1], true);
                                }

                                cRot[1].isIncreasing = false;
                                pRotation.Y = cRot[1].Maximum * cRot[1].MaxMinRatio;
                            }
                        }
                    }
                    else
                    {
                        pRotation.Y -= cRot[1].Step * cRot[1].StepRatio;

                        if (pRotation.Y < cRot[1].Minimum * cRot[1].MaxMinRatio)
                        {
                            if (cRot[1].isFullCircle)
                            {
                                pRotation.Y += 360;
                            }
                            else
                            {
                                if (cRot[1].Stop)
                                {
                                    cRot[1].Update = false;
                                    AnimationStopped?.Invoke(this, Coordinate.Y, cRot[1], true);
                                }

                                cRot[1].isIncreasing = true;
                                pRotation.Y = cRot[1].Minimum * cRot[1].MaxMinRatio;
                            }
                        }
                    }
                }
            }

            {
                if (cRot[2].Update)
                {
                    if (cRot[2].isIncreasing)
                    {
                        pRotation.Z += cRot[2].Step * cRot[2].StepRatio;
                        if (pRotation.Z > cRot[2].Maximum * cRot[2].MaxMinRatio)
                        {
                            if (cRot[2].isFullCircle)
                            {
                                pRotation.Z -= 360;
                            }
                            else
                            {
                                if (cRot[2].Stop)
                                {
                                    cRot[2].Update = false;
                                    AnimationStopped?.Invoke(this, Coordinate.Z, cRot[2], true);
                                }

                                cRot[2].isIncreasing = false;
                                pRotation.Z = cRot[2].Maximum * cRot[2].MaxMinRatio;
                            }
                        }
                    }
                    else
                    {
                        pRotation.Z -= cRot[2].Step * cRot[2].StepRatio;
                        if (pRotation.Z < cRot[2].Minimum * cRot[2].MaxMinRatio)
                        {
                            if (cRot[2].isFullCircle)
                            {
                                pRotation.Z += 360;
                            }
                            else
                            {
                                if (cRot[2].Stop)
                                {
                                    cRot[2].Update = false;
                                    AnimationStopped?.Invoke(this, Coordinate.Z, cRot[2], true);
                                }

                                cRot[2].isIncreasing = true;
                                pRotation.Z = cRot[2].Minimum * cRot[2].MaxMinRatio;
                            }
                        }
                    }
                }
            }

            Attach();
        }
    }
}