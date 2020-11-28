using GTA;
using GTA.Math;
using System.Collections.Generic;
using System.Linq;
using static FusionLibrary.Enums;

namespace FusionLibrary
{
    public class AnimatePropsHandler
    {
        public static List<AnimateProp> GlobalPropsList = new List<AnimateProp>();

        public static void Process()
        {
            GlobalPropsList.ForEach(x => x.Play());
        }

        public List<AnimateProp> Props = new List<AnimateProp>();

        public bool IsSpawned => Props[0].IsSpawned;

        public bool IsAnimationOn
        {
            get
            {
                return Props[0].IsAnimationOn;
            }

            set
            {
                Props.ForEach(x => x.IsAnimationOn = value);
            }
        }

        public void SpawnProp(bool deletePreviousProp = true)
        {
            Props.ForEach(x => x.SpawnProp(deletePreviousProp));
        }

        public void MoveProp(Vector3 pOffset, Vector3 pRotation, bool deletePreviousProp = true)
        {
            Props.ForEach(x => x.MoveProp(pOffset, pRotation, deletePreviousProp));
        }

        public void CheckExists()
        {
            Props.ForEach(x => x.CheckExists());
        }

        public void Detach()
        {
            Props.ForEach(x => x.Detach());
        }

        public void ScatterProp(float ForceMultiplier = 1f)
        {
            Props.ForEach(x => x.ScatterProp(ForceMultiplier));
        }

        public void Play()
        {
            Props.ForEach(x => x.Play());
        }

        public void Delete()
        {
            Props.ForEach(x => x.Delete());            
        }

        public void Dispose()
        {
            Props.ForEach(x => x.Dispose());
            Props.Clear();
        }

        public void TransferTo(Entity newEntity)
        {
            Props.ForEach(x => x.TransferTo(newEntity));
        }

        public void TransferTo(Entity newEntity, string boneName)
        {
            Props.ForEach(x => x.TransferTo(newEntity, boneName));
        }

        public void SetState(bool state)
        {
            Props.ForEach(x => x.SetState(state));
        }

        public void setPositionSettings(Coordinate pCord, bool cUpdate, bool cIncreasing, float cMinimum, float cMaximum, float cStep, float cStepRatio = 1f)
        {
            Props.ForEach(x => x.setPositionSettings(pCord, cUpdate, cIncreasing, cMinimum, cMaximum, cStep, cStepRatio));
        }

        public void setRotationSettings(Coordinate pCord, bool cUpdate, bool cIncreasing, float cMinimum, float cMaximum, float cStep, bool cFullCircle, float cStepRatio = 1f)
        {
            Props.ForEach(x => x.setRotationSettings(pCord, cUpdate, cIncreasing, cMinimum, cMaximum, cStep, cFullCircle, cStepRatio));
        }

        public bool Visible
        {
            get
            {
                return Props.First().Visible;
            }

            set
            {
                Props.ForEach(x => x.Visible = value);
            }
        }
        
        public Vector3 get_Offset(int index)
        {
            return Props[index].Offset;
        }

        public void set_Offset(int index, Vector3 value)
        {
            Props[index].Offset = value;
        }

        public bool get_PositionUpdate(Coordinate pCord)
        {
            return Props[0].get_PositionUpdate(pCord);
        }

        public void set_PositionUpdate(Coordinate pCord, bool value)
        {
            Props.ForEach(x => x.set_PositionUpdate(pCord, value));
        }

        public float get_PositionMaximum(Coordinate pCord)
        {
            return Props[0].get_PositionMaximum(pCord);
        }

        public void set_PositionMaximum(Coordinate pCord, float value)
        {
            Props.ForEach(x => x.set_PositionMaximum(pCord, value));
        }

        public float get_PositionMinimum(Coordinate pCord)
        {
            return Props[0].get_PositionMinimum(pCord);
        }

        public void set_PositionMinimum(Coordinate pCord, float value)
        {
            Props.ForEach(x => x.set_PositionMinimum(pCord, value));
        }

        public float get_PositionStep(Coordinate pCord)
        {
            return Props[0].get_PositionStep(pCord);
        }

        public void set_PositionStep(Coordinate pCord, float value)
        {
            Props.ForEach(x => x.set_PositionStep(pCord, value));
        }

        public float get_PositionStepRatio(Coordinate pCord)
        {
            return Props[0].get_PositionStepRatio(pCord);
        }

        public void set_PositionStepRatio(Coordinate pCord, float value)
        {
            Props.ForEach(x => x.set_PositionStepRatio(pCord, value));
        }

        public bool get_PositionIncreasing(Coordinate pCord)
        {
            return Props[0].get_PositionIncreasing(pCord);
        }

        public void set_PositionIncreasing(Coordinate pCord, bool value)
        {
            Props.ForEach(x => x.set_PositionIncreasing(pCord, value));
        }

        public float get_PositionMaxMinRatio(Coordinate pCord)
        {
            return Props[0].get_PositionMaxMinRatio(pCord);
        }

        public void set_PositionMaxMinRatio(Coordinate pCord, float value)
        {
            Props.ForEach(x => x.set_PositionMaxMinRatio(pCord, value));
        }
                
        public float get_AllRotation(Coordinate pCord)
        {
            return Props[0].get_Rotation(pCord);
        }

        public void set_AllRotation(Coordinate pCord, float value)
        {
            Props.ForEach(x => x.set_Rotation(pCord, value));
        }

        public Vector3 get_Rotation(int index)
        {
            return Props[index].Rotation;
        }

        public void set_Rotation(int index, Vector3 value)
        {
            Props[index].Rotation = value;
        }

        public bool get_RotationUpdate(Coordinate pCord)
        {
            return Props[0].get_RotationUpdate(pCord);
        }

        public void set_RotationUpdate(Coordinate pCord, bool value)
        {
            Props.ForEach(x => x.set_RotationUpdate(pCord, value));
        }

        public float get_RotationMaximum(Coordinate pCord)
        {
            return Props[0].get_RotationMaximum(pCord);
        }

        public void set_RotationMaximum(Coordinate pCord, float value)
        {
            Props.ForEach(x => x.set_RotationMaximum(pCord, value));
        }

        public float get_RotationMinimum(Coordinate pCord)
        {
            return Props[0].get_RotationMinimum(pCord);
        }

        public void set_RotationMinimum(Coordinate pCord, float value)
        {
            Props.ForEach(x => x.set_RotationMinimum(pCord, value));
        }

        public float get_RotationStep(Coordinate pCord)
        {
            return Props[0].get_RotationStep(pCord);
        }

        public void set_RotationStep(Coordinate pCord, float value)
        {
            Props.ForEach(x => x.set_RotationStep(pCord, value));
        }

        public float get_RotationStepRatio(Coordinate pCord)
        {
            return Props[0].get_RotationStepRatio(pCord);
        }

        public void set_RotationStepRatio(Coordinate pCord, float value)
        {
            Props.ForEach(x => x.set_RotationStepRatio(pCord, value));
        }

        public bool get_RotationIncreasing(Coordinate pCord)
        {
            return Props[0].get_RotationIncreasing(pCord);
        }

        public void set_RotationIncreasing(Coordinate pCord, bool value)
        {
            Props.ForEach(x => x.set_RotationIncreasing(pCord, value));
        }

        public bool get_RotationFullCircle(Coordinate pCord)
        {
            return Props[0].get_RotationFullCircle(pCord);
        }

        public void set_RotationFullCircle(Coordinate pCord, bool value)
        {
            Props.ForEach(x => x.set_RotationFullCircle(pCord, value));
        }

        public float get_RotationMaxMinRatio(Coordinate pCord)
        {
            return Props[0].get_RotationMaxMinRatio(pCord);
        }

        public void set_RotationMaxMinRatio(Coordinate pCord, float value)
        {
            Props.ForEach(x => x.set_RotationMaxMinRatio(pCord, value));
        }        
    }
}