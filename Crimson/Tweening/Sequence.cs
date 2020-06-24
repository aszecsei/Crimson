using System;
using System.Collections.Generic;

namespace Crimson.Tweening
{
    /// <summary>
    /// Controls other tweens as a group.
    /// </summary>
    public sealed class Sequence : Animation
    {
        internal readonly List<Animation> SequencedAnimations = new List<Animation>();
        internal float LastAnimationInsertTime;

        public Sequence()
        {
            AnimationType = AnimationType.Sequence;
            Reset();
        }

        #region Creation Methods

        internal static Sequence DoPrepend(Sequence inSequence, Animation anim)
        {
            if (anim.Loops == -1) anim.Loops = 1;
            float tFullTime = anim.Delay + (anim.BaseDuration * anim.Loops);
            inSequence.BaseDuration += tFullTime;
            int len = inSequence.SequencedAnimations.Count;
            for (int i = 0; i < len; ++i)
            {
                Animation sq = inSequence.SequencedAnimations[i];
                sq.SequencedPosition += tFullTime;
                sq.SequencedEndPosition += tFullTime;
            }

            return DoInsert(inSequence, anim, 0);
        }

        internal static Sequence DoInsert(Sequence inSequence, Animation anim, float atPosition)
        {
            AnimManager.AddActiveAnimationToSequence(anim);

            atPosition += anim.Delay;
            inSequence.LastAnimationInsertTime = atPosition;

            anim.Locked = true;
            anim.Parent = inSequence;
            if (anim.Loops == -1) anim.Loops = 1;
            float tFullTime = anim.BaseDuration * anim.Loops;
            anim.AutoKill = false;
            anim.Delay = anim.ElapsedDelay = 0;
            anim.SequencedPosition = atPosition;
            anim.SequencedEndPosition = atPosition + tFullTime;

            if (anim.SequencedEndPosition > inSequence.BaseDuration) inSequence.BaseDuration = anim.SequencedEndPosition;
            inSequence.SequencedAnimations.Add(anim);

            return inSequence;
        }

        internal static Sequence DoAppendInterval(Sequence inSequence, float interval)
        {
            inSequence.LastAnimationInsertTime = inSequence.BaseDuration;
            inSequence.BaseDuration += interval;
            return inSequence;
        }

        internal static Sequence DoPrependInterval(Sequence inSequence, float interval)
        {
            inSequence.LastAnimationInsertTime = 0;
            inSequence.BaseDuration += interval;
            int len = inSequence.SequencedAnimations.Count;
            for (int i = 0; i < len; ++i)
            {
                Animation sq = inSequence.SequencedAnimations[i];
                sq.SequencedPosition += interval;
                sq.SequencedEndPosition += interval;
            }

            return inSequence;
        }

        #endregion

        internal override float UpdateDelay(float elapsed)
        {
            float animDelay = Delay;
            if (elapsed > animDelay)
            {
                // Delay complete
                ElapsedDelay = animDelay;
                return elapsed - animDelay;
            }

            ElapsedDelay = elapsed;
            return 0;
        }

        internal override void Reset()
        {
            base.Reset();
            
            SequencedAnimations.Clear();
            LastAnimationInsertTime = 0;
        }

        internal override bool Validate()
        {
            int len = SequencedAnimations.Count;
            for (int i = 0; i < len; ++i)
            {
                if (!SequencedAnimations[i].Validate()) return false;
            }

            return true;
        }

        internal override bool Startup()
        {
            return DoStartup(this);
        }

        internal override bool ApplyAnimation(float prevPosition, int prevCompletedLoops, int newCompletedSteps, bool useInversePosition)
        {
            return DoApplyAnimation(this, prevPosition, prevCompletedLoops, newCompletedSteps, useInversePosition);
        }

        internal static void Setup(Sequence s)
        {
            s.AutoKill = TweenSubsystem.DefaultAutoKill;
            s.Recyclable = TweenSubsystem.DefaultRecyclable;
            s.IsPlaying = TweenSubsystem.DefaultAutoPlay.HasFlag(AutoPlay.Sequences);
            s.LoopType = TweenSubsystem.DefaultLoopType;
            s.Easer = TweenSubsystem.DefaultEaser;
        }

        internal static bool DoStartup(Sequence s)
        {
            if (s.SequencedAnimations.Count == 0 && !IsAnyCallbackSet(s))
            {
                return false; // empty sequence without callback set
            }

            s.StartupDone = true;
            s.FullDuration = s.Loops > -1 ? s.BaseDuration * s.Loops : Mathf.INFINITY;
            StableSortSequencedAnimations(s.SequencedAnimations);
            return true;
        }

        internal static bool DoApplyAnimation(Sequence s, float prevPosition, int prevCompletedLoops, int newCompletedSteps,
            bool useInversePosition)
        {
            float prevPos = prevPosition;
            float newPos = s.Position;
            if (s.Easer != Ease.Linear.GetEaser())
            {
                prevPos = s.BaseDuration * s.Easer.Invoke(prevPos / s.BaseDuration);
                newPos = s.BaseDuration * s.Easer.Invoke(newPos / s.BaseDuration);
            }

            float from, to = 0;
            // Determine if prevPos was inverse. Used to calculate correct "from" value when applying
            // internal cycle and also in case of multiple loops within a single update
            bool prevPosIsInverse = (s.Loops == -1 || s.Loops > 1) && s.LoopType == LoopType.Yoyo
                                                                   && (prevPos < s.BaseDuration
                                                                       ? prevCompletedLoops % 2 != 0
                                                                       : prevCompletedLoops % 2 == 0);
            if (s.IsBackwards) prevPosIsInverse = !prevPosIsInverse;
            // Update multiple loop cycles within the same update
            if (newCompletedSteps > 0)
            {
                int expectedCompletedLoops = s.CompletedLoops;
                float expectedPosition = s.Position;

                int cycles = newCompletedSteps;
                int cyclesDone = 0;
                from = prevPos;
                while (cyclesDone < cycles)
                {
                    if (cyclesDone > 0) from = to;
                    else if (prevPosIsInverse && !s.IsBackwards) from = s.BaseDuration - from;
                    to = prevPosIsInverse ? 0 : s.BaseDuration;
                    if (ApplyInternalCycle(s, from, to, useInversePosition, prevPosIsInverse, true)) return true;
                    cyclesDone++;
                    if (s.HasLoops && s.LoopType == LoopType.Yoyo) prevPosIsInverse = !prevPosIsInverse;
                }
                if (expectedCompletedLoops != s.CompletedLoops || Mathf.Approximately(expectedPosition, s.Position))
                {
                    return !s.Active;
                }
            }
            
            // Run current cycle
            if (newCompletedSteps == 1 && s.IsComplete) return false;
            if (newCompletedSteps > 0 && !s.IsComplete)
            {
                from = useInversePosition ? s.BaseDuration : 0;
                if (s.LoopType == LoopType.Restart && to > 0)
                    ApplyInternalCycle(s, s.BaseDuration, 0, false, false, false);
            }
            else
            {
                from = useInversePosition ? s.BaseDuration - prevPos : prevPos;
            }

            return ApplyInternalCycle(s, from, useInversePosition ? s.BaseDuration - newPos : newPos,
                useInversePosition, prevPosIsInverse);
        }

        private static bool ApplyInternalCycle(Sequence s, float fromPos, float toPos, bool useInverse,
            bool prevPosIsInverse, bool multiCycleStep = false)
        {
            bool wasPlaying = s.IsPlaying;
            bool isBackwardsUpdate = toPos < fromPos;

            if (isBackwardsUpdate)
            {
                int len = s.SequencedAnimations.Count - 1;
                for (int i = len; i > -1; --i)
                {
                    if (!s.Active) return true; // Killed by some internal callback
                    if (!s.IsPlaying && wasPlaying) return false; // Paused by internal callback
                    Animation anim = s.SequencedAnimations[i];
                    if (anim.SequencedEndPosition < toPos || anim.SequencedPosition > fromPos) continue;

                    float gotoPos = toPos - anim.SequencedPosition;
                    if (gotoPos < 0) gotoPos = 0;
                    if (!anim.StartupDone) continue; // we're going backwards and this animation never started, just ignore it
                    anim.IsBackwards = true;
                    if (AnimManager.Goto(anim, gotoPos, false))
                    {
                        // Nested tween failed. If it's the only animation and there's no callbacks, just kill the whole sequence
                        if (s.SequencedAnimations.Count == 1 && !IsAnyCallbackSet(s)) return true;
                        // Otherwise just remove the failed animation from the sequence and continue
                        AnimManager.Despawn(anim, false);
                        s.SequencedAnimations.Remove(anim);
                        --i;
                        --len;
                        continue;
                    }

                    if (multiCycleStep && anim.AnimationType == AnimationType.Sequence)
                    {
                        if (s.Position <= 0 && s.CompletedLoops == 0) anim.Position = 0;
                        else
                        {
                            bool toZero = s.CompletedLoops == 0 ||
                                          s.IsBackwards && (s.CompletedLoops < s.Loops || s.Loops == -1);
                            if (anim.IsBackwards) toZero = !toZero;
                            if (useInverse) toZero = !toZero;
                            if (s.IsBackwards && !useInverse && !prevPosIsInverse) toZero = !toZero;
                            anim.Position = toZero ? 0 : anim.BaseDuration;
                        }
                    }
                }
            }
            else
            {
                int len = s.SequencedAnimations.Count;
                for (int i = 0; i < len; ++i)
                {
                    if (!s.Active) return true; // Killed by some internal callback
                    if (!s.IsPlaying && wasPlaying) return false; // Paused by internal callback
                    Animation anim = s.SequencedAnimations[i];
                    if (anim.SequencedPosition > toPos
                        || anim.SequencedPosition > 0 && anim.SequencedEndPosition <= fromPos
                        || anim.SequencedPosition <= 0 && anim.SequencedEndPosition < fromPos)
                    {
                        continue;
                    }

                    float gotoPos = toPos - anim.SequencedPosition;
                    if (gotoPos < 0) gotoPos = 0;
                    if (toPos >= anim.SequencedEndPosition)
                    {
                        if (!anim.StartupDone) AnimManager.ForceInit(anim, true);
                        if (gotoPos < anim.FullDuration) gotoPos = anim.FullDuration;
                    }

                    anim.IsBackwards = false;
                    if (AnimManager.Goto(anim, gotoPos, false))
                    {
                        // Nested tween failed. If it's the only animation and there's no callbacks, just kill the whole sequence
                        if (s.SequencedAnimations.Count == 1 && !IsAnyCallbackSet(s)) return true;
                        // Otherwise just remove the failed animation from the sequence and continue
                        AnimManager.Despawn(anim, false);
                        s.SequencedAnimations.Remove(anim);
                        --i;
                        --len;
                        continue;
                    }

                    if (multiCycleStep && anim.AnimationType == AnimationType.Sequence)
                    {
                        if (s.Position <= 0 && s.CompletedLoops == 0) anim.Position = 0;
                        else
                        {
                            bool toZero = s.CompletedLoops == 0 ||
                                          !s.IsBackwards && (s.CompletedLoops < s.Loops || s.Loops == -1);
                            if (anim.IsBackwards) toZero = !toZero;
                            if (useInverse) toZero = !toZero;
                            if (s.IsBackwards && !useInverse && !prevPosIsInverse) toZero = !toZero;
                            anim.Position = toZero ? 0 : anim.BaseDuration;
                        }
                    }
                }
            }

            return false;
        }

        // TODO: Better stable sorting algorithm?
        private static void StableSortSequencedAnimations(List<Animation> list)
        {
            int len = list.Count;
            for (int i = 1; i < len; i++)
            {
                int j = i;
                Animation temp = list[i];
                while (j > 0 && list[j - 1].SequencedPosition > temp.SequencedPosition)
                {
                    list[j] = list[j - 1];
                    j = j - 1;
                }

                list[j] = temp;
            }
        }

        private static bool IsAnyCallbackSet(Sequence s)
        {
            return s.OnComplete != null || s.OnKill != null || s.OnPause != null || s.OnPlay != null
                   || s.OnStart != null || s.OnUpdate != null;
        }
    }
}