using System;
using System.Collections.Generic;
using Crimson.Tweening.Plugins.Options;

namespace Crimson.Tweening
{
    internal static class AnimManager
    {
        const int DEFAULT_MAX_TWEENS = 200;
        const int DEFAULT_MAX_SEQUENCES = 50;

        internal static int MaxActive = DEFAULT_MAX_TWEENS + DEFAULT_MAX_SEQUENCES;
        internal static int MaxTweens = DEFAULT_MAX_TWEENS;
        internal static int MaxSequences = DEFAULT_MAX_SEQUENCES;
        internal static int TotalActiveAnimations;
        internal static int TotalActiveTweens, TotalActiveSequences;
        internal static int TotalPooledTweens, TotalPooledSequences;
        internal static int TotalTweens, TotalSequences;
        /// <summary>
        /// TRUE while an update cycle is running (used to treat direct tween Kills differently)
        /// </summary>
        internal static bool IsUpdateLoop = false;

        internal static Animation?[] ActiveAnimations = new Animation[DEFAULT_MAX_TWEENS + DEFAULT_MAX_SEQUENCES];
        private static Animation?[] s_pooledAnimations = new Animation[DEFAULT_MAX_TWEENS];
        private static readonly Stack<Animation> PooledSequences = new Stack<Animation>();
        
        private static readonly List<Animation> KillList = new List<Animation>(DEFAULT_MAX_TWEENS + DEFAULT_MAX_SEQUENCES);
        private static int s_maxActiveLookupId;
        private static bool s_needsReorganization;
        private static int s_reorganizeFromId = -1;
        private static int s_minPooledTweenId = -1;
        private static int s_maxPooledTweenId = -1;

        internal static TweenCore<T, TPlugOptions> GetTween<T, TPlugOptions>()
        where TPlugOptions : struct, IPlugOptions
        {
            TweenCore<T, TPlugOptions> t;
            if (TotalPooledTweens > 0)
            {
                Type typeOf = typeof(T);
                Type typeOfTPlugOptions = typeof(TPlugOptions);
                for (int i = s_maxPooledTweenId; i > s_minPooledTweenId - 1; --i)
                {
                    Animation? tween = s_pooledAnimations[i];
                    if (tween != null && tween.Type == typeOf && tween.OptionsType == typeOfTPlugOptions)
                    {
                        // Pooled tween exists: spawn it
                        t = (TweenCore<T, TPlugOptions>) tween;
                        AddActiveAnimation(t);
                        s_pooledAnimations[i] = null;
                        if (s_maxPooledTweenId != s_minPooledTweenId)
                        {
                            if (i == s_maxPooledTweenId) s_maxPooledTweenId--;
                            else if (i == s_minPooledTweenId) s_minPooledTweenId++;
                        }

                        TotalPooledTweens--;
                        return t;
                    }
                }
                // Not found: remove a tween from the pool in case it's full
                if (TotalTweens >= MaxTweens)
                {
                    s_pooledAnimations[s_maxPooledTweenId] = null;
                    s_maxPooledTweenId--;
                    TotalPooledTweens--;
                    TotalTweens--;
                }
            }
            else
            {
                // Increase capacity in case max number of Tweens has already been reached, then continue
                if (TotalTweens >= MaxTweens - 1)
                {
                    IncreaseCapacities(CapacityIncreaseMode.TweensOnly);
                }
            }
            
            // Not found: create new Tween
            t = new TweenCore<T, TPlugOptions>();
            TotalTweens++;
            AddActiveAnimation(t);
            return t;
        }

        internal static Sequence GetSequence()
        {
            Sequence s;
            if (TotalPooledSequences > 0)
            {
                s = (Sequence) PooledSequences.Pop();
                AddActiveAnimation(s);
                TotalPooledSequences--;
                return s;
            }
            // Increase capacity in case max number of Sequences has already been reached, then continue
            if (TotalSequences >= MaxSequences - 1)
            {
                IncreaseCapacities(CapacityIncreaseMode.SequencesOnly);
            }
            // Not found: create new Sequence
            s = new Sequence();
            TotalSequences++;
            AddActiveAnimation(s);
            return s;
        }

        internal static void SetUpdateType(Animation anim, UpdateType updateType, bool isIndependentUpdate)
        {
            anim.UpdateType = updateType;
            anim.IsTimeScaleIndependent = isIndependentUpdate;
        }

        internal static void AddActiveAnimationToSequence(Animation anim)
        {
            RemoveActiveAnimation(anim);
        }

        internal static int DespawnAll()
        {
            int totalDespawned = TotalActiveAnimations;
            for (int i = 0; i < s_maxActiveLookupId + 1; ++i)
            {
                Animation? t = ActiveAnimations[i];
                if (t != null) Despawn(t, false);
            }

            ClearAnimationArray(ActiveAnimations);
            TotalActiveAnimations = 0;
            TotalActiveTweens = TotalActiveSequences = 0;
            s_maxActiveLookupId = s_reorganizeFromId = -1;
            s_needsReorganization = false;
            
            return totalDespawned;
        }

        internal static void Despawn(Animation anim, bool modifyActiveLists = true)
        {
            anim.OnKill?.Invoke();

            if (modifyActiveLists)
            {
                RemoveActiveAnimation(anim);
            }

            if (anim.Recyclable)
            {
                // Put the tween inside a pool
                switch (anim.AnimationType)
                {
                    case AnimationType.Sequence:
                        PooledSequences.Push(anim);
                        TotalPooledSequences++;
                        // Despawn sequenced tweens
                        Sequence s = (Sequence) anim;
                        int len = s.SequencedAnimations.Count;
                        for (int i = 0; i < len; ++i)
                            Despawn(s.SequencedAnimations[i], false);
                        break;
                    case AnimationType.Tween:
                        if (s_maxPooledTweenId == -1)
                        {
                            s_maxPooledTweenId = MaxTweens - 1;
                            s_minPooledTweenId = MaxTweens - 1;
                        }

                        if (s_maxPooledTweenId < MaxTweens - 1)
                        {
                            s_pooledAnimations[s_maxPooledTweenId + 1] = anim;
                            s_maxPooledTweenId++;
                            if (s_minPooledTweenId > s_maxPooledTweenId) s_minPooledTweenId = s_maxPooledTweenId;
                        }
                        else
                        {
                            for (int i = s_maxPooledTweenId; i > -1; --i)
                            {
                                if (s_pooledAnimations[i] != null) continue;
                                s_pooledAnimations[i] = anim;
                                if (i < s_minPooledTweenId) s_minPooledTweenId = i;
                                if (s_maxPooledTweenId < s_minPooledTweenId) s_maxPooledTweenId = s_minPooledTweenId;
                                break;
                            }
                        }

                        TotalPooledTweens++;
                        break;
                }
            }
            else
            {
                // Remove
                switch (anim.AnimationType)
                {
                    case AnimationType.Sequence:
                        TotalSequences--;
                        Sequence s = (Sequence) anim;
                        int len = s.SequencedAnimations.Count;
                        for (int i = 0; i < len; ++i) Despawn(s.SequencedAnimations[i], false);
                        break;
                    case AnimationType.Tween:
                        TotalTweens--;
                        break;
                }
            }

            anim.Active = false;
            anim.Reset();
        }

        /// <summary>
        /// Destroys any active tweens without putting them back in a pool,
        /// then purges all pools and resets capacities
        /// </summary>
        internal static void PurgeAll(bool isApplicationQuitting)
        {
            if (!isApplicationQuitting)
            {
                // Fire eventual onKill callbacks
                for (int i = 0; i < MaxActive; ++i)
                {
                    Animation? t = ActiveAnimations[i];
                    if (t != null && t.Active)
                    {
                        t.Active = false;
                        t.OnKill?.Invoke();
                    }
                }
            }

            ClearAnimationArray(ActiveAnimations);
            TotalActiveAnimations = 0;
            TotalActiveTweens = TotalActiveSequences = 0;
            s_maxActiveLookupId = s_reorganizeFromId = -1;
            s_needsReorganization = false;
            PurgePools();
            ResetCapacities();
            TotalTweens = TotalSequences = 0;
        }

        /// <summary>
        /// Removes any cached tween from the pools
        /// </summary>
        internal static void PurgePools()
        {
            TotalTweens -= TotalPooledTweens;
            TotalSequences -= TotalPooledSequences;
            ClearAnimationArray(s_pooledAnimations);
            PooledSequences.Clear();
            TotalPooledTweens = TotalPooledSequences = 0;
            s_minPooledTweenId = s_maxPooledTweenId = -1;
        }

        internal static void ResetCapacities()
        {
            SetCapacities(DEFAULT_MAX_TWEENS, DEFAULT_MAX_SEQUENCES);
        }

        internal static void SetCapacities(int tweensCapacity, int sequencesCapacity)
        {
            if (tweensCapacity < sequencesCapacity) tweensCapacity = sequencesCapacity;

            MaxActive = tweensCapacity + sequencesCapacity;
            MaxTweens = tweensCapacity;
            MaxSequences = sequencesCapacity;
            Array.Resize(ref ActiveAnimations, MaxActive);
            Array.Resize(ref s_pooledAnimations, tweensCapacity);
            KillList.Capacity = MaxActive;
        }

        internal static int Validate()
        {
            if (s_needsReorganization) ReorganizeActiveAnimations();

            int totalInvalid = 0;
            for (int i = 0; i < s_maxActiveLookupId + 1; ++i)
            {
                Animation anim = ActiveAnimations[i]!;
                if (!anim.Validate())
                {
                    totalInvalid++;
                    MarkForKilling(anim);
                }
            }
            
            // Kill all eventually marked tweens
            if (totalInvalid > 0)
            {
                DespawnActiveAnimations(KillList);
                KillList.Clear();
            }

            return totalInvalid;
        }

        internal static void Update(UpdateType updateType, float deltaTime, float independentTime)
        {
            if (s_needsReorganization) ReorganizeActiveAnimations();
            IsUpdateLoop = true;

            bool willKill = false;
            int len = s_maxActiveLookupId + 1;

            for (int i = 0; i < len; ++i)
            {
                Animation? t = ActiveAnimations[i];
                if (t == null || t.UpdateType != updateType) continue;

                if (!t.Active)
                {
                    willKill = true;
                    MarkForKilling(t);
                    continue;
                }

                if (!t.IsPlaying) continue;

                t.Locked = true;
                float tDeltaTime = (t.IsTimeScaleIndependent ? independentTime : deltaTime);
                if (Mathf.Approximately(tDeltaTime, 0)) continue;

                if (!t.DelayComplete)
                {
                    tDeltaTime = t.UpdateDelay(t.ElapsedDelay + tDeltaTime);
                    if (tDeltaTime <= -1)
                    {
                        // Error during startup; mark tween for killing
                        willKill = true;
                        MarkForKilling(t);
                        continue;
                    }

                    if (tDeltaTime <= 0) continue;
                    if (t.PlayedOnce)
                    {
                        // If it hasn't played yet, startup routine will call it
                        t.OnPlay?.Invoke();
                    }
                }

                if (!t.StartupDone)
                {
                    if (!t.Startup())
                    {
                        // Startup failure; mark for killing
                        willKill = true;
                        MarkForKilling(t);
                        continue;
                    }
                }

                float toPosition = t.Position;
                bool wasEndPosition = toPosition >= t.BaseDuration;
                int toCompletedLoops = t.CompletedLoops;

                if (t.BaseDuration <= 0)
                {
                    toPosition = 0;
                    toCompletedLoops = t.Loops == -1 ? t.CompletedLoops + 1 : t.Loops;
                }
                else
                {
                    if (t.IsBackwards)
                    {
                        toPosition -= tDeltaTime;
                        while (toPosition < 0 && toCompletedLoops > -1)
                        {
                            toPosition += t.BaseDuration;
                            toCompletedLoops--;
                        }

                        if (toCompletedLoops < 0 || wasEndPosition && toCompletedLoops < 1)
                        {
                            // Result is equivalent to a rewind
                            toPosition = 0;
                            toCompletedLoops = wasEndPosition ? 1 : 0;
                        }
                    }
                    else
                    {
                        toPosition += tDeltaTime;
                        while (toPosition >= t.BaseDuration && (t.Loops == -1 || toCompletedLoops < t.Loops))
                        {
                            toPosition -= t.BaseDuration;
                            toCompletedLoops++;
                        }
                    }

                    if (wasEndPosition) toCompletedLoops--;
                    if (t.Loops != -1 && toCompletedLoops >= t.Loops) toPosition = t.BaseDuration;
                }

                bool needsKilling = Animation.DoGoto(t, toPosition, toCompletedLoops);
                if (needsKilling)
                {
                    willKill = true;
                    MarkForKilling(t);
                }
            }
            
            // Kill all eventually marked tweens
            if (willKill)
            {
                DespawnActiveAnimations(KillList);
                KillList.Clear();
            }

            IsUpdateLoop = false;
        }

        #region Play Operations

        internal static bool Complete(Animation anim, bool modifyActiveLists = true)
        {
            if (anim.Loops == -1) return false;
            if (!anim.IsComplete)
            {
                Animation.DoGoto(anim, anim.BaseDuration, anim.Loops);
                anim.IsPlaying = false;
                // Despawn if needed
                if (anim.AutoKill)
                {
                    if (IsUpdateLoop) anim.Active = false;
                    else Despawn(anim, modifyActiveLists);
                }

                return true;
            }

            return false;
        }

        internal static bool Flip(Animation anim)
        {
            anim.IsBackwards = !anim.IsBackwards;
            return true;
        }

        internal static void ForceInit(Animation anim, bool isSequenced = false)
        {
            if (anim.StartupDone) return;

            if (!anim.Startup() && !isSequenced)
            {
                // Startup failed: kill tween
                if (IsUpdateLoop) anim.Active = false;
                else RemoveActiveAnimation(anim);
            }
        }

        internal static bool Goto(Animation anim, float to, bool andPlay = false)
        {
            bool wasPlaying = anim.IsPlaying;
            anim.IsPlaying = andPlay;
            anim.ElapsedDelay = anim.Delay;
            int toCompletedLoops = anim.BaseDuration <= 0 ? 1 : Mathf.FloorToInt(to / anim.BaseDuration);
            float toPosition = to % anim.BaseDuration;
            if (anim.Loops != -1 && toCompletedLoops >= anim.Loops)
            {
                toCompletedLoops = anim.Loops;
                toPosition = anim.BaseDuration;
            }
            else if (toPosition >= anim.BaseDuration)
            {
                toPosition = 0;
            }

            bool needsKilling = Animation.DoGoto(anim, toPosition, toCompletedLoops);
            if (!andPlay && wasPlaying && !needsKilling) anim.OnPause?.Invoke();
            return needsKilling;
        }

        internal static bool Pause(Animation anim)
        {
            if (anim.IsPlaying)
            {
                anim.IsPlaying = false;
                anim.OnPause?.Invoke();
                return true;
            }

            return false;
        }

        internal static bool Play(Animation anim)
        {
            if (!anim.IsPlaying && (!anim.IsBackwards && !anim.IsComplete ||
                                 anim.IsBackwards && (anim.CompletedLoops > 0 || anim.Position > 0)))
            {
                anim.IsPlaying = true;
                if (anim.PlayedOnce && anim.DelayComplete) anim.OnPlay?.Invoke();
                return true;
            }

            return false;
        }

        internal static bool PlayBackwards(Animation anim)
        {
            if (anim.CompletedLoops == 0 && anim.Position <= 0)
            {
                anim.IsBackwards = true;
                anim.IsPlaying = false;
                return false;
            }

            if (!anim.IsBackwards)
            {
                anim.IsBackwards = true;
                Play(anim);
                return true;
            }

            return Play(anim);
        }

        internal static bool PlayForward(Animation anim)
        {
            if (anim.IsComplete)
            {
                anim.IsBackwards = false;
                anim.IsPlaying = false;
                return false;
            }

            if (anim.IsBackwards)
            {
                anim.IsBackwards = false;
                Play(anim);
                return true;
            }

            return Play(anim);
        }

        internal static bool Restart(Animation anim, bool includeDelay = true, float changeDelayTo = -1)
        {
            bool wasPaused = !anim.IsPlaying;
            anim.IsBackwards = false;
            if (changeDelayTo >= 0 && anim.AnimationType == AnimationType.Tween) anim.Delay = changeDelayTo;
            Rewind(anim, includeDelay);
            anim.IsPlaying = true;
            if (wasPaused && anim.PlayedOnce && anim.DelayComplete) anim.OnPlay?.Invoke();
            return true;
        }

        internal static bool Rewind(Animation anim, bool includeDelay = true)
        {
            bool wasPlaying = anim.IsPlaying;
            anim.IsPlaying = false;
            bool rewinded = false;
            if (anim.Delay > 0)
            {
                if (includeDelay)
                {
                    rewinded = anim.ElapsedDelay > 0;
                    anim.ElapsedDelay = 0;
                }
                else
                {
                    rewinded = anim.ElapsedDelay < anim.Delay;
                    anim.ElapsedDelay = anim.Delay;
                }
            }

            if (anim.Position > 0 || anim.CompletedLoops > 0 || !anim.StartupDone)
            {
                rewinded = true;
                bool needsKilling = Animation.DoGoto(anim, 0, 0);
                if (!needsKilling && wasPlaying) anim.OnPause?.Invoke();
            }

            return rewinded;
        }

        internal static bool SmoothRewind(Animation anim)
        {
            bool rewinded = false;
            if (anim.Delay > 0)
            {
                rewinded = anim.ElapsedDelay < anim.Delay;
                anim.ElapsedDelay = anim.Delay;
            }

            if (anim.Position > 0 || anim.CompletedLoops > 0 || !anim.StartupDone)
            {
                rewinded = true;
                anim.Goto(anim.ElapsedDirectionalPercentage() * anim.BaseDuration);
                anim.PlayBackwards();
            }
            else
            {
                anim.IsPlaying = false;
            }

            return rewinded;
        }

        internal static bool TogglePause(Animation anim)
        {
            if (anim.IsPlaying) return Pause(anim);
            return Play(anim);
        }

        #endregion

        #region Private Methods

        private static void MarkForKilling(Animation anim)
        {
            anim.Active = false;
            KillList.Add(anim);
        }

        private static void AddActiveAnimation(Animation anim)
        {
            if (s_needsReorganization) ReorganizeActiveAnimations();

            if (TotalActiveAnimations < 0)
            {
                TotalActiveAnimations = 0;
            }

            anim.Active = true;
            anim.UpdateType = TweenSubsystem.DefaultUpdateType;
            anim.IsTimeScaleIndependent = TweenSubsystem.DefaultTimeScaleIndependent;
            anim.ActiveId = s_maxActiveLookupId = TotalActiveAnimations;
            ActiveAnimations[TotalActiveAnimations] = anim;

            TotalActiveAnimations++;
            if (anim.AnimationType == AnimationType.Tween) TotalActiveTweens++;
            else TotalActiveSequences++;
        }

        private static void ReorganizeActiveAnimations()
        {
            if (TotalActiveAnimations <= 0)
            {
                s_maxActiveLookupId = 0;
                s_needsReorganization = false;
                s_reorganizeFromId = -1;
                return;
            }
            else if (s_reorganizeFromId == s_maxActiveLookupId)
            {
                s_maxActiveLookupId--;
                s_needsReorganization = false;
                s_reorganizeFromId = -1;
                return;
            }

            int shift = 1;
            int len = s_maxActiveLookupId + 1;
            s_maxActiveLookupId = s_reorganizeFromId - 1;
            for (int i = s_reorganizeFromId + 1; i < len; ++i)
            {
                Animation a = ActiveAnimations[i];
                if (a == null)
                {
                    shift++;
                    continue;
                }

                a.ActiveId = s_maxActiveLookupId = i - shift;
                ActiveAnimations[i - shift] = a;
                ActiveAnimations[i] = null;
            }

            s_needsReorganization = false;
            s_reorganizeFromId = -1;
        }

        private static void DespawnActiveAnimations(List<Animation> animations)
        {
            int count = animations.Count - 1;
            for (int i = count; i > -1; --i) Despawn(animations[i]);
        }

        private static void RemoveActiveAnimation(Animation t)
        {
            int index = t.ActiveId;

            t.ActiveId = -1;
            s_needsReorganization = true;
            if (s_reorganizeFromId == -1 || s_reorganizeFromId > index) s_reorganizeFromId = index;
            ActiveAnimations[index] = null;

            TotalActiveAnimations--;
            if (t.AnimationType == AnimationType.Tween) TotalActiveTweens--;
            else TotalActiveSequences--;
            
            // Bounds safety checks
            if (TotalActiveAnimations < 0)
            {
                TotalActiveAnimations = 0;
            }

            if (TotalActiveTweens < 0)
            {
                TotalActiveTweens = 0;
            }

            if (TotalActiveSequences < 0)
            {
                TotalActiveSequences = 0;
            }
        }

        private static void ClearAnimationArray(Animation?[] animations)
        {
            int len = animations.Length;
            for (int i = 0; i < len; ++i) animations[i] = null;
        }

        private static void IncreaseCapacities(CapacityIncreaseMode increaseMode)
        {
            int killAdd = 0;
            int increaseTweensBy = Mathf.Max((int) (MaxTweens * 1.5f), DEFAULT_MAX_TWEENS);
            int increaseSequencesBy = Mathf.Max((int) (MaxSequences * 1.5f), DEFAULT_MAX_SEQUENCES);
            switch (increaseMode)
            {
                case CapacityIncreaseMode.TweensOnly:
                    killAdd += increaseTweensBy;
                    MaxTweens += increaseTweensBy;
                    Array.Resize(ref s_pooledAnimations, MaxTweens);
                    break;
                case CapacityIncreaseMode.SequencesOnly:
                    killAdd += increaseSequencesBy;
                    MaxSequences += increaseSequencesBy;
                    break;
                case CapacityIncreaseMode.TweensAndSequences:
                    killAdd += increaseTweensBy + increaseSequencesBy;
                    MaxTweens += increaseTweensBy;
                    MaxSequences += increaseSequencesBy;
                    Array.Resize(ref s_pooledAnimations, MaxTweens);
                    break;
            }

            MaxActive = MaxTweens + MaxSequences;
            Array.Resize(ref ActiveAnimations, MaxActive);
            if (killAdd > 0) KillList.Capacity += killAdd;
        }

        #endregion

        internal enum CapacityIncreaseMode
        {
            TweensAndSequences,
            TweensOnly,
            SequencesOnly,
        }
    }
}