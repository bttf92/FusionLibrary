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
        public Entity Entity { get; set; }

        public bool IsAnimationOn { get; set; }
        public bool UsePhysicalAttach { get; set; }

        public float Duration { get; set; } = 0;
        public Vector3 AdjustOffset = Vector3.Zero;
        public Vector3 AdjustRotation = Vector3.Zero;

        public bool IsSpawned => Prop.NotNullAndExists();
        
        private bool ToBone;
        private EntityBone Bone;
        private Vector3 pOffset;
        private Vector3 pRotation;
        
        public CoordinateSetting[] OffsetSettings = new CoordinateSetting[4];
        public CoordinateSetting[] RotationSettings = new CoordinateSetting[4];
        private bool IsDetached = false;
        private float _currentTime = 0;        

        /// <summary>
        /// Spawns a new prop with <paramref name="pModel"/> attached to <paramref name="boneName"/> of <paramref name="pEntity"/> with <paramref name="pOffset"/> and <paramref name="pRotation"/>
        /// </summary>
        /// <param name="pModel"><seealso cref="GTA.Model"/> of the prop.</param>
        /// <param name="pEntity"><seealso cref="GTA.Entity"/> owner of the <paramref name="boneName"/>.</param>
        /// <param name="boneName">Bone's name of the <paramref name="pEntity"/>.</param>
        /// <param name="pOffset">A <seealso cref="GTA.Vector3"/> indicating offset of the prop relative to the <paramref name="boneName"/>'s position.</param>
        /// <param name="pRotation">A <seealso cref="GTA.Vector3"/> indicating rotation of the prop.</param>
        public AnimateProp(Model pModel, Entity pEntity, string boneName, Vector3 pOffset, Vector3 pRotation, bool cAnimationOn = false, bool doNotSpawn = false, bool usePhysicalAttach = false)
        {
            this.Model = pModel;
            this.Entity = pEntity;
            Bone = pEntity.Bones[boneName];
            this.pOffset = pOffset;
            this.pRotation = pRotation;
            ToBone = true;
            IsAnimationOn = cAnimationOn;
            UsePhysicalAttach = usePhysicalAttach;

            if (!doNotSpawn)
                SpawnProp();

            AnimatePropsHandler.GlobalPropsList.Add(this);
        }

        public AnimateProp(Model pModel, Entity pEntity, Vector3 pOffset, Vector3 pRotation, bool cAnimationOn = false, bool doNotSpawn = false, bool usePhysicalAttach = false)
        {
            this.Model = pModel;
            this.Entity = pEntity;            
            this.pOffset = pOffset;
            this.pRotation = pRotation;
            ToBone = false;
            IsAnimationOn = cAnimationOn;
            UsePhysicalAttach = usePhysicalAttach;

            if (!doNotSpawn)
                SpawnProp();

            AnimatePropsHandler.GlobalPropsList.Add(this);
        }

        public AnimateProp(Entity pEntity, Model pModel, Vector3 pOffset, Vector3 pRotation, bool usePhysicalAttach = false) : this(pModel, pEntity, pOffset, pRotation, true, true, usePhysicalAttach)
        {

        }

        public AnimateProp(Entity pEntity, Model pModel, string boneName, bool usePhysicalAttach = false) : this(pModel, pEntity, boneName, Vector3.Zero, Vector3.Zero, true, true, usePhysicalAttach)
        {

        }

        public void SpawnProp()
        {
            if (!IsSpawned)
            {
                Utils.LoadAndRequestModel(Model);
                Prop = World.CreateProp(Model, Entity.Position, false, false);
                Prop.IsPersistent = true;
            }

            Attach();
        }

        private void Attach()
        {
            if (IsDetached)
                return;

            if (!IsSpawned)
                SpawnProp();

            Vector3 pOffset = this.pOffset + AdjustOffset;
            Vector3 pRotation = this.pRotation + AdjustRotation;

            if (ToBone)
            {
                if (UsePhysicalAttach)
                    Prop.AttachToPhysically(Entity, Bone.Index, pOffset, pRotation);
                else
                    Function.Call(Hash.ATTACH_ENTITY_TO_ENTITY, Prop.Handle, Entity.Handle, Bone.Index, pOffset.X, pOffset.Y, pOffset.Z, pRotation.X, pRotation.Y, pRotation.Z, false, false, true, false, 2, true);
            }                
            else
            {
                if (UsePhysicalAttach)
                    Prop.AttachToPhysically(Entity, pOffset, pRotation);
                else
                    Function.Call(Hash.ATTACH_ENTITY_TO_ENTITY, Prop.Handle, Entity.Handle, 0, pOffset.X, pOffset.Y, pOffset.Z, pRotation.X, pRotation.Y, pRotation.Z, false, false, true, false, 2, true);
            }
        }

        public void MoveOffset(Coordinate coordinate, float value)
        {
            switch (coordinate)
            {
                case Coordinate.X:
                    AdjustOffset.X = value;
                    break;
                case Coordinate.Y:
                    AdjustOffset.Y = value;
                    break;
                default:
                    AdjustOffset.Z = value;
                    break;
            }

            Attach();
        }

        public void MoveRotation(Coordinate coordinate, float value)
        {
            switch (coordinate)
            {
                case Coordinate.X:
                    AdjustRotation.X = value;
                    break;
                case Coordinate.Y:
                    AdjustRotation.Y = value;
                    break;
                default:
                    AdjustRotation.Z = value;
                    break;
            }

            Attach();
        }

        public void MoveProp(Vector3 pOffset, Vector3 pRotation)
        {
            AdjustOffset = pOffset;
            AdjustRotation = pRotation;

            Attach();
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
        public void setOffsetSettings(Coordinate pCord, bool cUpdate, bool cIncreasing, float cMinimum, float cMaximum, float cMaxMinRatio, float cStep, float cStepRatio, bool Stop = false, bool simulateAcceleration = false)
        {
            OffsetSettings[(int)pCord].isSetted = true;
            OffsetSettings[(int)pCord].Update = cUpdate;
            OffsetSettings[(int)pCord].isIncreasing = cIncreasing;
            OffsetSettings[(int)pCord].Minimum = cMinimum;
            OffsetSettings[(int)pCord].Maximum = cMaximum;
            OffsetSettings[(int)pCord].Step = cStep;
            OffsetSettings[(int)pCord].StepRatio = cStepRatio;
            OffsetSettings[(int)pCord].Stop = Stop;
            OffsetSettings[(int)pCord].MaxMinRatio = cMaxMinRatio;
            OffsetSettings[(int)pCord].SimulateAcceleration = simulateAcceleration;
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
        public void setRotationSettings(Coordinate pCord, bool cUpdate, bool cIncreasing, float cMinimum, float cMaximum, float cMaxMinRatio, float cStep, float cStepRatio, bool Stop = false, bool simulateAcceleration = false)
        {
            RotationSettings[(int)pCord].isSetted = true;
            RotationSettings[(int)pCord].Update = cUpdate;
            RotationSettings[(int)pCord].isIncreasing = cIncreasing;
            RotationSettings[(int)pCord].Minimum = cMinimum;
            RotationSettings[(int)pCord].Maximum = cMaximum;
            RotationSettings[(int)pCord].Step = cStep;
            RotationSettings[(int)pCord].StepRatio = cStepRatio;
            RotationSettings[(int)pCord].Stop = Stop;
            RotationSettings[(int)pCord].MaxMinRatio = cMaxMinRatio;
            RotationSettings[(int)pCord].SimulateAcceleration = simulateAcceleration;
        }
        
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

        public float getOffset(Coordinate pCord)
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

        public void setOffset(Coordinate pCord, float value)
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

        public bool getOffsetUpdate(Coordinate pCord)
        {
            return OffsetSettings[(int)pCord].Update;
        }

        public void setOffsetUpdate(Coordinate pCord, bool value)
        {
            OffsetSettings[(int)pCord].Update = value;
        }

        public float getOffsetMaximum(Coordinate pCord)
        {
            return OffsetSettings[(int)pCord].Maximum;
        }

        public void setOffsetMaximum(Coordinate pCord, float value)
        {
            OffsetSettings[(int)pCord].Maximum = value;
        }

        public float getOffsetMinimum(Coordinate pCord)
        {
            return OffsetSettings[(int)pCord].Minimum;
        }

        public void setOffsetMinimum(Coordinate pCord, float value)
        {
            OffsetSettings[(int)pCord].Minimum = value;
        }

        public float getOffsetStep(Coordinate pCord)
        {
            return OffsetSettings[(int)pCord].Step;
        }

        public void setOffsetStep(Coordinate pCord, float value)
        {
            OffsetSettings[(int)pCord].Step = value;
        }

        public float getOffsetStepRatio(Coordinate pCord)
        {
            return OffsetSettings[(int)pCord].StepRatio;
        }

        public void setOffsetStepRatio(Coordinate pCord, float value)
        {
            OffsetSettings[(int)pCord].StepRatio = value;
        }

        public bool getOffsetIncreasing(Coordinate pCord)
        {
            return OffsetSettings[(int)pCord].isIncreasing;
        }

        public void setOffsetIncreasing(Coordinate pCord, bool value)
        {
            OffsetSettings[(int)pCord].isIncreasing = value;
        }

        public bool getOffsetStop(Coordinate pCord)
        {
            return OffsetSettings[(int)pCord].Stop;
        }

        public void setOffsetStop(Coordinate pCord, bool value)
        {
            OffsetSettings[(int)pCord].Stop = value;
        }

        public float getOffsetMaxMinRatio(Coordinate pCord)
        {
            return OffsetSettings[(int)pCord].MaxMinRatio;
        }

        public void setOffsetMaxMinRatio(Coordinate pCord, float value)
        {
            OffsetSettings[(int)pCord].MaxMinRatio = value;
        }       
        
        public float getRotation(Coordinate pCord)
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

        public void setRotation(Coordinate pCord, float value)
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

        public bool getRotationUpdate(Coordinate pCord)
        {
            return RotationSettings[(int)pCord].Update;
        }

        public void setRotationUpdate(Coordinate pCord, bool value)
        {
            RotationSettings[(int)pCord].Update = value;
        }

        public float getRotationMaximum(Coordinate pCord)
        {
            return RotationSettings[(int)pCord].Maximum;
        }

        public void setRotationMaximum(Coordinate pCord, float value)
        {
            RotationSettings[(int)pCord].Maximum = value;
        }

        public float getRotationMinimum(Coordinate pCord)
        {
            return RotationSettings[(int)pCord].Minimum;
        }

        public void setRotationMinimum(Coordinate pCord, float value)
        {
            RotationSettings[(int)pCord].Minimum = value;
        }

        public float getRotationStep(Coordinate pCord)
        {
            return RotationSettings[(int)pCord].Step;
        }

        public void setRotationStep(Coordinate pCord, float value)
        {
            RotationSettings[(int)pCord].Step = value;
        }

        public float getRotationStepRatio(Coordinate pCord)
        {
            return RotationSettings[(int)pCord].StepRatio;
        }

        public void setRotationStepRatio(Coordinate pCord, float value)
        {
            RotationSettings[(int)pCord].StepRatio = value;
        }

        public bool getRotationIncreasing(Coordinate pCord)
        {
            return RotationSettings[(int)pCord].isIncreasing;
        }

        public void setRotationIncreasing(Coordinate pCord, bool value)
        {
            RotationSettings[(int)pCord].isIncreasing = value;
        }

        public bool getRotationStop(Coordinate pCord)
        {
            return RotationSettings[(int)pCord].Stop;
        }

        public void setRotationStop(Coordinate pCord, bool value)
        {
            RotationSettings[(int)pCord].Stop = value;
        }

        public float getRotationMaxMinRatio(Coordinate pCord)
        {
            return RotationSettings[(int)pCord].MaxMinRatio;
        }

        public void setRotationMaxMinRatio(Coordinate pCord, float value)
        {
            RotationSettings[(int)pCord].MaxMinRatio = value;
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

            for(int i = 0;i < 4;i++)
            {
                UpdateOffset(i);
                UpdateRotation(i);
            }

            Attach();
        }

        public void setOffsetAtMaximum(Coordinate coordinate)
        {
            AdjustOffset[(int)coordinate] = (OffsetSettings[(int)coordinate].Maximum * OffsetSettings[(int)coordinate].MaxMinRatio) - pOffset[(int)coordinate];
            OffsetSettings[(int)coordinate].isIncreasing = false;
            Attach();
        }

        public void setOffsetAtMinimum(Coordinate coordinate)
        {
            AdjustOffset[(int)coordinate] = (OffsetSettings[(int)coordinate].Minimum * OffsetSettings[(int)coordinate].MaxMinRatio) - pOffset[(int)coordinate];
            OffsetSettings[(int)coordinate].isIncreasing = true;
            Attach();
        }

        public void setRotationAtMaximum(Coordinate coordinate)
        {
            AdjustRotation[(int)coordinate] = (RotationSettings[(int)coordinate].Maximum * RotationSettings[(int)coordinate].MaxMinRatio) - pRotation[(int)coordinate];
            RotationSettings[(int)coordinate].isIncreasing = false;
            Attach();
        }

        public void setRotationAtMinimum(Coordinate coordinate)
        {
            AdjustRotation[(int)coordinate] = (RotationSettings[(int)coordinate].Minimum * RotationSettings[(int)coordinate].MaxMinRatio) - pRotation[(int)coordinate];
            RotationSettings[(int)coordinate].isIncreasing = true;
            Attach();
        }

        private void UpdateOffset(int i)
        {
            if (!OffsetSettings[i].Update)
                return;

            float current = AdjustOffset[i] + pOffset[i];
            float step = OffsetSettings[i].Step * OffsetSettings[i].StepRatio * Game.LastFrameTime;
            float end;

            if (OffsetSettings[i].isIncreasing)
            {
                end = OffsetSettings[i].Maximum;
                current += step;
            }                
            else
            {
                end = OffsetSettings[i].Minimum;
                current -= step;
            }
                
            end *= OffsetSettings[i].MaxMinRatio;

            AdjustOffset[i] = current - pOffset[i];

            if (current.Near(end, step))
            {
                OffsetSettings[i].isIncreasing = false;
                AdjustOffset[i] = end - pOffset[i];

                if (OffsetSettings[i].Stop)
                {
                    OffsetSettings[i].Update = false;
                    AnimationStopped?.Invoke(this, (Coordinate)i, OffsetSettings[i], false);
                }                
            }
        }

        private void UpdateRotation(int i)
        {
            if (!RotationSettings[i].Update)
                return;

            float current = AdjustRotation[i] + pRotation[i];
            float step = RotationSettings[i].Step * RotationSettings[i].StepRatio * Game.LastFrameTime;
            float end;
            
            if (RotationSettings[i].isIncreasing)
            {
                end = RotationSettings[i].Maximum;
                current += step;
            }                
            else
            {
                end = RotationSettings[i].Minimum;
                current -= step;
            }

            end *= RotationSettings[i].MaxMinRatio;

            if (Math.Abs(current) > 360)
                current += 360 * (current > 0 ? -1 : 1);

            AdjustRotation[i] = current - pRotation[i];

            if (current.Near(end, step))
            {
                RotationSettings[i].isIncreasing = !RotationSettings[i].isIncreasing;
                AdjustRotation[i] = end - pRotation[i];

                if (RotationSettings[i].Stop)
                {                    
                    RotationSettings[i].Update = false;
                    AnimationStopped?.Invoke(this, (Coordinate)i, RotationSettings[i], true);
                }
            }
        }

        public static implicit operator Prop(AnimateProp animateProp) => animateProp.Prop;
        public static implicit operator Entity(AnimateProp animateProp) => animateProp.Prop;
    }
}