using FusionLibrary.Extensions;
using GTA;
using System.Collections.Generic;
using System.Linq;
using static FusionLibrary.FusionEnums;

namespace FusionLibrary
{
    public delegate void OnSequenceCompleted(bool isLooped);

    public class AnimatePropsHandler
    {
        internal static List<AnimatePropsHandler> GlobalAnimatePropsHandlerList = new List<AnimatePropsHandler>();

        internal static void TickAll()
        {
            for (int i = 0; i < GlobalAnimatePropsHandlerList.Count; i++)
            {
                GlobalAnimatePropsHandlerList[i].Tick();
            }
        }

        /// <summary>
        /// This event is fired up when sequence is completed.
        /// </summary>
        public event OnSequenceCompleted OnSequenceCompleted;

        /// <summary>
        /// This event is fired up when an <see cref="Animation"/> of any <see cref="AnimateProp"/> is completed.
        /// </summary>
        public event OnAnimCompleted OnAnimCompleted;

        /// <summary>
        /// List of <see cref="AnimateProp"/>s handled by this <see cref="AnimatePropsHandler"/>.
        /// </summary>
        public List<AnimateProp> Props { get; } = new List<AnimateProp>();

        /// <summary>
        /// Whether spawn <see cref="AnimateProp"/> sequentially.
        /// </summary>
        public bool SequenceSpawn { get; set; }

        /// <summary>
        /// Interval of the spawn sequence.
        /// </summary>
        public int SequenceInterval { get; set; }

        /// <summary>
        /// Whether the spawn sequence is looped.
        /// </summary>
        public bool IsSequenceLooped { get; set; }

        /// <summary>
        /// Whether the spawn sequence is playing or not.
        /// </summary>
        public bool IsSequencePlaying { get; private set; }

        /// <summary>
        /// Whether the spawn sequence is random.
        /// </summary>
        public bool IsSequenceRandom { get; set; }

        private int nextSequenceTime;
        private int currentSequenceProp = -1;
        private readonly List<int> playedProps = new List<int>();

        public AnimatePropsHandler()
        {
            GlobalAnimatePropsHandlerList.Add(this);
        }

        /// <summary>
        /// Counts of handled <see cref="AnimateProp"/>.
        /// </summary>
        public int Count
        {
            get
            {
                return Props.Count();
            }
        }

        /// <summary>
        /// Gets or sets <see cref="AnimateProp.Visible"/> of <see cref="Props"/>.
        /// </summary>
        public bool Visible
        {
            get
            {
                return Props[0].Visible;
            }

            set
            {
                Props.ForEach(x => x.Visible = value);
            }
        }

        /// <summary>
        /// Gets or sets <see cref="AnimateProp.UseFixedRot"/> of <see cref="Props"/>.
        /// </summary>
        public bool UseFixedRot
        {
            get
            {
                return Props.TrueForAll(x => x.UseFixedRot);
            }

            set
            {
                Props.ForEach(x => x.UseFixedRot = value);
            }
        }

        /// <summary>
        /// Gets or sets <see cref="AnimateProp.UsePhysicalAttach"/> of <see cref="Props"/>.
        /// </summary>
        public bool UsePhysicalAttach
        {
            get
            {
                return Props.TrueForAll(x => x.UsePhysicalAttach);
            }

            set
            {
                Props.ForEach(x => x.UsePhysicalAttach = value);
            }
        }

        /// <summary>
        /// <see langword="true"/> if all the <see cref="Props"/> are spawned.
        /// </summary>
        public bool IsSpawned
        {
            get
            {
                return Props.TrueForAll(x => x.IsSpawned);
            }
        }

        /// <summary>
        /// <see langword="true"/> if at least one of the <see cref="Props"/> is playing.
        /// </summary>
        public bool IsPlaying
        {
            get
            {
                return Props.Any(x => x.IsPlaying);
            }
        }

        /// <summary>
        /// Adds <see cref="AnimateProp"/> to <see cref="Props"/>.
        /// </summary>
        /// <param name="animateProp"></param>
        public void Add(AnimateProp animateProp)
        {
            Props.Add(animateProp);
            animateProp.OnAnimCompleted += AnimateProp_OnAnimCompleted;
        }

        internal void Tick()
        {
            if (!IsSequencePlaying || Game.GameTime < nextSequenceTime)
            {
                return;
            }

            if (IsSequenceRandom)
            {
                if (playedProps.Count == Count)
                {
                    if (IsSequenceLooped)
                    {
                        playedProps.Clear();

                        OnSequenceCompleted?.Invoke(true);
                    }
                    else
                    {
                        Delete();

                        OnSequenceCompleted?.Invoke(false);
                        return;
                    }
                }

                if (currentSequenceProp > -1)
                {
                    Props[currentSequenceProp]?.Delete();
                }

                if (playedProps.Count == 0)
                {
                    currentSequenceProp = FusionUtils.Random.NextExcept(0, Count, currentSequenceProp);
                }
                else
                {
                    currentSequenceProp = FusionUtils.Random.NextExcept(0, Count, currentSequenceProp);
                }

                Props[currentSequenceProp]?.SpawnProp();

                playedProps.Add(currentSequenceProp);

                nextSequenceTime = Game.GameTime + SequenceInterval;

                return;
            }

            if (playedProps.Count == Count)
            {
                if (IsSequenceLooped)
                {
                    Props[currentSequenceProp]?.Delete();

                    currentSequenceProp = -1;
                    playedProps.Clear();
                }
                else
                {
                    Delete();

                    OnSequenceCompleted?.Invoke(false);
                    return;
                }
            }

            if (currentSequenceProp > -1)
            {
                Props[currentSequenceProp]?.Delete();
            }

            currentSequenceProp++;

            Props[currentSequenceProp]?.SpawnProp();

            playedProps.Add(currentSequenceProp);

            nextSequenceTime = Game.GameTime + SequenceInterval;
        }

        private void AnimateProp_OnAnimCompleted(AnimationStep animationStep)
        {
            foreach (AnimateProp prop in Props)
            {
                if (prop[AnimationType.Offset][animationStep].CoordinateSettings.Any(x => x.Update) || prop[AnimationType.Rotation][animationStep].CoordinateSettings.Any(x => x.Update))
                {
                    return;
                }
            }

            OnAnimCompleted?.Invoke(animationStep);
        }

        /// <summary>
        /// <see cref="AnimateProp.SaveAnimation"/> for each <see cref="Props"/>.
        /// </summary>
        public void SaveAnimation()
        {
            Props.ForEach(x => x.SaveAnimation());
        }

        /// <summary>
        /// <see cref="AnimateProp.RestoreAnimation"/> for each <see cref="Props"/>.
        /// </summary>
        public void RestoreAnimation()
        {
            Props.ForEach(x => x.RestoreAnimation());
        }

        /// <summary>
        /// <see cref="AnimateProp.SpawnProp"/> for each <see cref="Props"/>.
        /// </summary>
        public void SpawnProp()
        {
            Props.ForEach(x => x.SpawnProp());
        }

        /// <summary>
        /// Starts <see cref="Animation"/> of all <see cref="Props"/>.
        /// </summary>
        /// <param name="instant">Whether play the animation instant.</param>
        /// <param name="spawnAndRestore">Whether restore the <see cref="AnimateProp.SavedAnimation"/> before playing.</param>
        public void Play(bool instant = false, bool spawnAndRestore = false)
        {
            Play(AnimationStep.First, instant, spawnAndRestore);
        }

        /// <summary>
        /// Starts <see cref="Animation"/> of all <see cref="Props"/>.
        /// </summary>
        /// <param name="animationStep"><see cref="FusionEnums.AnimationStep"/> to be played.</param>
        /// <param name="instant">Whether play the animation instant.</param>
        /// <param name="playInstantPreviousSteps">Whether play previous steps.</param>
        /// <param name="spawnAndRestore">Whether restore the <see cref="AnimateProp.SavedAnimation"/> before playing.</param>
        public void Play(AnimationStep animationStep, bool instant = false, bool playInstantPreviousSteps = false, bool spawnAndRestore = false)
        {
            if (SequenceSpawn)
            {
                IsSequencePlaying = true;
                return;
            }

            Props.ForEach(x => x.Play(animationStep, instant, playInstantPreviousSteps, spawnAndRestore));
        }

        /// <summary>
        /// Stops playing all the <see cref="Props"/>.
        /// </summary>
        public void Stop()
        {
            IsSequencePlaying = false;
            //currentSequenceProp = -1;
            //playedProps.Clear();

            Props.ForEach(x => x.Stop());
        }

        /// <summary>
        /// Transfers all the <see cref="Props"/> to <paramref name="entity"/>.
        /// </summary>
        /// <param name="entity">New <see cref="GTA.Entity"/> instance.</param>
        public void TransferTo(Entity entity)
        {
            Props.ForEach(x => x.TransferTo(entity));
        }

        /// <summary>
        /// Transfers all the <see cref="Props"/> to <paramref name="boneName"/> of <paramref name="entity"/>.
        /// </summary>
        /// <param name="entity">New <see cref="GTA.Entity"/> instance.</param>
        /// <param name="boneName">Bone's name.</param>
        public void TransferTo(Entity entity, string boneName)
        {
            Props.ForEach(x => x.TransferTo(entity, boneName));
        }

        /// <summary>
        /// Transfers all the <see cref="Props"/> to <paramref name="entityBone"/> of <paramref name="entity"/>.
        /// </summary>
        /// <param name="entity">New <see cref="GTA.Entity"/> instance.</param>
        /// <param name="entityBone"><see cref="EntityBone"/> of <paramref name="entity"/>.</param>
        public void TransferTo(Entity entity, EntityBone entityBone)
        {
            Props.ForEach(x => x.TransferTo(entity, entityBone));
        }

        /// <summary>
        /// Sets the specified <see cref="Coordinate"/> of the offset to <paramref name="value"/>.
        /// </summary>
        /// <param name="coordinate">Wanted <see cref="Coordinate"/></param>
        /// <param name="value">Value of the <paramref name="coordinate"/>.</param>
        /// <param name="currentOffset">If <see langword="true"/> is applied to <see cref="AnimateProp.CurrentOffset"/> otherwise to <see cref="AnimateProp.SecondOffset"/>.</param>
        public void setOffset(Coordinate coordinate, float value, bool currentOffset = false)
        {
            Props.ForEach(x => x.setOffset(coordinate, value, currentOffset));
        }

        /// <summary>
        /// Sets the specified <see cref="Coordinate"/> of the rotation to <paramref name="value"/>.
        /// </summary>
        /// <param name="coordinate">Wanted <see cref="Coordinate"/></param>
        /// <param name="value">Value of the <paramref name="coordinate"/>.</param>
        /// <param name="currentRotation">If <see langword="true"/> is applied to <see cref="AnimateProp.CurrentRotation"/> otherwise to <see cref="AnimateProp.SecondRotation"/>.</param>
        public void setRotation(Coordinate coordinate, float value, bool currentRotation = false)
        {
            Props.ForEach(x => x.setRotation(coordinate, value, currentRotation));
        }

        /// <summary>
        /// Sets the specified <paramref name="coordinate"/> to the maximum or minimum value.
        /// </summary>
        /// <param name="animationType">Wanted <see cref="FusionEnums.AnimationType"/>.</param>
        /// <param name="animationStep"><see cref="FusionEnums.AnimationStep"/> of <paramref name="animationType"/>.</param>
        /// <param name="coordinate"><see cref="FusionEnums.Coordinate"/> of <paramref name="animationStep"/>.</param>
        /// <param name="maximum"><see langword="true"/> sets <paramref name="coordinate"/> to maximum value; otherwise minimum.</param>
        public void setCoordinateAt(AnimationType animationType, AnimationStep animationStep, Coordinate coordinate, bool maximum)
        {
            Props.ForEach(x => x.setCoordinateAt(animationType, animationStep, coordinate, maximum));
        }

        /// <summary>
        /// Plays specified <paramref name="animationStep"/> instantly.
        /// </summary>
        /// <param name="animationStep">Wanted <see cref="FusionEnums.AnimationStep"/>.</param>
        public void setInstantAnimationStep(AnimationStep animationStep)
        {
            Props.ForEach(x => x.setInstantAnimationStep(animationStep));
        }

        /// <summary>
        /// Deletes all the <see cref="Props"/>.
        /// </summary>
        /// <param name="keepProp">Whether keep the <see cref="Prop"/> in the world.</param>
        public void Delete(bool keepProp = false)
        {
            IsSequencePlaying = false;
            currentSequenceProp = -1;
            playedProps.Clear();

            Props.ForEach(x => x.Delete(keepProp));
        }

        /// <summary>
        /// Detaches the <see cref="Props"/>.
        /// </summary>
        public void Detach()
        {
            Props.ForEach(x => x.Detach());
        }

        /// <summary>
        /// Detaches and scatters all the <see cref="Props"/>.
        /// </summary>
        /// <param name="ForceMultiplier">Force value to be applied. Default <c>1</c>.</param>
        public void ScatterProp(float ForceMultiplier = 1f)
        {
            Props.ForEach(x => x.ScatterProp(ForceMultiplier));
        }

        /// <summary>
        /// Disposes all the <see cref="Props"/>.
        /// </summary>
        /// <param name="keepProp">>Whether keep the <see cref="Prop"/> in the world.</param>
        public void Dispose(bool keepProp = false)
        {
            Props.ForEach(x => x.Dispose(keepProp));
            Props.Clear();

            GlobalAnimatePropsHandlerList.Remove(this);
        }

        public AnimateProp this[int propIndex]
        {
            get
            {
                return Props[propIndex];
            }

            set
            {
                Props[propIndex] = value;
            }
        }
    }
}