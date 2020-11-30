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

        public static void ProcessAll()
        {
            GlobalPropsList.ForEach(x => x.Play());
        }

        public static void Abort()
        {
            GlobalPropsList.ForEach(x => x.Dispose());
        }

        public List<AnimateProp> Props = new List<AnimateProp>();

        public AnimationStopped AnimationStopped;

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

        public void Process()
        {
            List<AnimateProp> animateProps = Props.Where(x =>
            {
                for(int i = 0;i < 4;i++)
                {
                    if (x.OffsetSettings[i].isSetted)
                        return true;

                    if (x.RotationSettings[i].isSetted)
                        return true;
                }

                return false;
            }).ToList();


        }

        public void SpawnProp()
        {
            Props.ForEach(x => x.SpawnProp());
        }

        public void MoveProp(Vector3 pOffset, Vector3 pRotation)
        {
            Props.ForEach(x => x.MoveProp(pOffset, pRotation));
        }

        public void MoveOffset(Coordinate coordinate, float value) 
        {
            Props.ForEach(x => x.MoveOffset(coordinate, value));
        }

        public void MoveRotation(Coordinate coordinate, float value)
        {
            Props.ForEach(x => x.MoveRotation(coordinate, value));
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

        public void setOffsetSettings(Coordinate pCord, bool cUpdate, bool cIncreasing, float cMinimum, float cMaximum, float cMaxMinRatio, float cStep, float cStepRatio, bool cStop = false, bool cSimulateAcceleration = false)
        {
            Props.ForEach(x => x.setOffsetSettings(pCord, cUpdate, cIncreasing, cMinimum, cMaximum, cMaxMinRatio, cStep, cStepRatio, cStop, cSimulateAcceleration));
        }

        public void setRotationSettings(Coordinate pCord, bool cUpdate, bool cIncreasing, float cMinimum, float cMaximum, float cMaxMinRatio, float cStep, float cStepRatio, bool cStop = false, bool cSimulateAcceleration = false)
        {
            Props.ForEach(x => x.setRotationSettings(pCord, cUpdate, cIncreasing, cMinimum, cMaximum, cMaxMinRatio, cStep, cStepRatio, cStop, cSimulateAcceleration));
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
        
        public Vector3 getOffset(int index)
        {
            return Props[index].Offset;
        }

        public void setOffset(int index, Vector3 value)
        {
            Props[index].Offset = value;
        }

        public bool getOffsetUpdate(Coordinate pCord)
        {
            return Props.TrueForAll(x => x.getOffsetUpdate(pCord));
        }

        public void setOffsetUpdate(Coordinate pCord, bool value)
        {
            Props.ForEach(x => x.setOffsetUpdate(pCord, value));
        }

        public float getOffsetMaximum(Coordinate pCord)
        {
            return Props[0].getOffsetMaximum(pCord);
        }

        public void setOffsetMaximum(Coordinate pCord, float value)
        {
            Props.ForEach(x => x.setOffsetMaximum(pCord, value));
        }

        public float getOffsetMinimum(Coordinate pCord)
        {
            return Props[0].getOffsetMinimum(pCord);
        }

        public void setOffsetMinimum(Coordinate pCord, float value)
        {
            Props.ForEach(x => x.setOffsetMinimum(pCord, value));
        }

        public float getOffsetStep(Coordinate pCord)
        {
            return Props[0].getOffsetStep(pCord);
        }

        public void setOffsetStep(Coordinate pCord, float value)
        {
            Props.ForEach(x => x.setOffsetStep(pCord, value));
        }

        public float getOffsetStepRatio(Coordinate pCord)
        {
            return Props[0].getOffsetStepRatio(pCord);
        }

        public void setOffsetStepRatio(Coordinate pCord, float value)
        {
            Props.ForEach(x => x.setOffsetStepRatio(pCord, value));
        }

        public bool getOffsetIncreasing(Coordinate pCord)
        {
            return Props.TrueForAll(x => x.getOffsetIncreasing(pCord));
        }

        public void setOffsetIncreasing(Coordinate pCord, bool value)
        {
            Props.ForEach(x => x.setOffsetIncreasing(pCord, value));
        }

        public float getOffsetMaxMinRatio(Coordinate pCord)
        {
            return Props[0].getOffsetMaxMinRatio(pCord);
        }

        public void setOffsetMaxMinRatio(Coordinate pCord, float value)
        {
            Props.ForEach(x => x.setOffsetMaxMinRatio(pCord, value));
        }
                
        public float getAllRotation(Coordinate pCord)
        {
            return Props[0].getRotation(pCord);
        }

        public void setAllRotation(Coordinate pCord, float value)
        {
            Props.ForEach(x => x.setRotation(pCord, value));
        }

        public Vector3 getRotation(int index)
        {
            return Props[index].Rotation;
        }

        public void setRotation(int index, Vector3 value)
        {
            Props[index].Rotation = value;
        }

        public bool getRotationUpdate(Coordinate pCord)
        {
            return Props.TrueForAll(x => x.getRotationUpdate(pCord));
        }

        public void setRotationUpdate(Coordinate pCord, bool value)
        {
            Props.ForEach(x => x.setRotationUpdate(pCord, value));
        }

        public float getRotationMaximum(Coordinate pCord)
        {
            return Props[0].getRotationMaximum(pCord);
        }

        public void setRotationMaximum(Coordinate pCord, float value)
        {
            Props.ForEach(x => x.setRotationMaximum(pCord, value));
        }

        public float getRotationMinimum(Coordinate pCord)
        {
            return Props[0].getRotationMinimum(pCord);
        }

        public void setRotationMinimum(Coordinate pCord, float value)
        {
            Props.ForEach(x => x.setRotationMinimum(pCord, value));
        }

        public float getRotationStep(Coordinate pCord)
        {
            return Props[0].getRotationStep(pCord);
        }

        public void setRotationStep(Coordinate pCord, float value)
        {
            Props.ForEach(x => x.setRotationStep(pCord, value));
        }

        public float getRotationStepRatio(Coordinate pCord)
        {
            return Props[0].getRotationStepRatio(pCord);
        }

        public void setRotationStepRatio(Coordinate pCord, float value)
        {
            Props.ForEach(x => x.setRotationStepRatio(pCord, value));
        }

        public bool getRotationIncreasing(Coordinate pCord)
        {
            return Props.TrueForAll(x => x.getRotationIncreasing(pCord));
        }

        public void setRotationIncreasing(Coordinate pCord, bool value)
        {
            Props.ForEach(x => x.setRotationIncreasing(pCord, value));
        }

        public float getRotationMaxMinRatio(Coordinate pCord)
        {
            return Props[0].getRotationMaxMinRatio(pCord);
        }

        public void setRotationMaxMinRatio(Coordinate pCord, float value)
        {
            Props.ForEach(x => x.setRotationMaxMinRatio(pCord, value));
        }

        public void setOffsetAtMaximum(Coordinate coordinate)
        {
            Props.ForEach(x => x.setOffsetAtMaximum(coordinate));
        }

        public void setOffsetAtMinimum(Coordinate coordinate)
        {
            Props.ForEach(x => x.setOffsetAtMinimum(coordinate));
        }

        public void setRotationAtMaximum(Coordinate coordinate)
        {
            Props.ForEach(x => x.setRotationAtMaximum(coordinate));
        }

        public void setRotationAtMinimum(Coordinate coordinate)
        {
            Props.ForEach(x => x.setRotationAtMinimum(coordinate));
        }

        public AnimateProp this[int propIndex]
        {
            get { return Props[propIndex]; }
            set { Props[propIndex] = value; }
        }
    }
}