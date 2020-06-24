using System;

namespace Crimson.Tweening
{
    public abstract class Animation
    {
        #region Properties

        /// <summary>
        /// Whether the animation is a tween or a sequence.
        /// </summary>
        internal AnimationType AnimationType;
        /// <summary>
        /// Position in sequence
        /// </summary>
        internal float SequencedPosition;
        /// <summary>
        /// End position in sequence
        /// </summary>
        internal float SequencedEndPosition;
        /// <summary>
        /// The index of the animation in the AnimManager list
        /// </summary>
        internal int ActiveId;
        
        /// <summary>
        /// The timescale of the animation.
        /// </summary>
        public float TimeScale = 1f;
        /// <summary>
        /// Whether the animation is independent of Crimson's scene time scale.
        /// </summary>
        public bool IsTimeScaleIndependent;
        /// <summary>
        /// The update type of the animation
        /// </summary>
        public UpdateType UpdateType;
        /// <summary>
        /// Whether the animation is playing backwards.
        /// </summary>
        public bool IsBackwards = false;
        /// <summary>
        /// This value is -1 if the animation is playing backwards, and 1 if it is playing forwards.
        /// </summary>
        public float Direction => IsBackwards ? -1 : 1;
        /// <summary>
        /// The animation's numeric ID
        /// </summary>
        public int? Id = null;
        
        /// <summary>
        /// Called when the animation begins (before any delay)
        /// </summary>
        public TweenCallback? OnStart = null;
        /// <summary>
        /// Called when the animation begins to play (after any delay)
        /// </summary>
        public TweenCallback? OnPlay = null;
        /// <summary>
        /// Called when the animation is paused
        /// </summary>
        public TweenCallback? OnPause = null;
        /// <summary>
        /// Called every frame the animation updates
        /// </summary>
        public TweenCallback? OnUpdate = null;
        /// <summary>
        /// Called when the animation completes
        /// </summary>
        public TweenCallback? OnComplete = null;
        /// <summary>
        /// Called when the animation is destroyed
        /// </summary>
        public TweenCallback? OnKill = null;
        /// <summary>
        /// Whether or not the animation is recyclable
        /// </summary>
        public bool Recyclable { get; internal set; }
        /// <summary>
        /// Whether or not the animation should auto-kill after it has elapsed
        /// </summary>
        public bool AutoKill { get; internal set; }
        /// <summary>
        /// The delay before the animation begins
        /// </summary>
        public float Delay { get; internal set; } = 0f;
        /// <summary>
        /// How long the animation has waited to begin so far
        /// </summary>
        public float ElapsedDelay { get; internal set; } = 0f;
        /// <summary>
        /// Whether or not the animation has completed its delay
        /// </summary>
        public bool DelayComplete => ElapsedDelay >= Delay;
        /// <summary>
        /// Whether or not startup has completed
        /// </summary>
        public bool StartupDone { get; internal set; } = false;

        /// <summary>
        /// The duration of a single loop of the animation
        /// </summary>
        public float BaseDuration { get; internal set; } = 0f;
        /// <summary>
        /// The duration of the entire animation
        /// </summary>
        public float FullDuration { get; internal set; } = 0f;
        /// <summary>
        /// The amount of time the animation has played in the current loop
        /// </summary>
        public float Position { get; internal set; } = 0;
        /// <summary>
        /// How many times to loop the animation (negative values indicate an infinite loop)
        /// </summary>
        public int Loops { get; internal set; } = 1;
        /// <summary>
        /// Whether or not the animation loops
        /// </summary>
        public bool HasLoops => Loops < 0 || Loops > 1;
        /// <summary>
        /// The loop type to use
        /// </summary>
        public LoopType LoopType { get; internal set; }
        /// <summary>
        /// How many times the animation has looped thus far
        /// </summary>
        public int CompletedLoops { get; internal set; } = 0;
        /// <summary>
        /// Whether or not the animation is currently playing
        /// </summary>
        public bool IsPlaying { get; internal set; } = false;
        /// <summary>
        /// Whether or not the animation has played before
        /// </summary>
        public bool PlayedOnce { get; internal set; } = false;
        /// <summary>
        /// Whether or not the animation has finished
        /// </summary>
        public bool IsComplete { get; internal set; } = false;
        /// <summary>
        /// The parent sequence, if any
        /// </summary>
        public Animation? Parent { get; internal set; } = null;
        /// <summary>
        /// Whether or not this animation has a parent sequence
        /// </summary>
        public bool IsSequenced => Parent != null;
        /// <summary>
        /// Whether or not the animation is active (used for pooling)
        /// </summary>
        public bool Active { get; internal set; }
        /// <summary>
        /// Whether or not the animation is currently locked (locked animations have certain restrictions on modifications)
        /// </summary>
        public bool Locked { get; internal set; } = false;
        /// <summary>
        /// The easing function for this animation
        /// </summary>
        public Easer Easer { get; internal set; } = Ease.Linear.GetEaser();
        
        /// <summary>
        /// The type being modified by the animation, if any
        /// </summary>
        public Type? Type { get; internal set; } = null;
        /// <summary>
        /// The options for the type plugin, if any
        /// </summary>
        public Type? OptionsType { get; internal set; } = null;

        #endregion

        /// <summary>
        /// Resets all variables related to this animation
        /// </summary>
        internal virtual void Reset()
        {
            TimeScale = 1;
            IsBackwards = false;
            Id = null;
            IsTimeScaleIndependent = false;
            OnStart = OnPlay = OnUpdate = OnComplete = OnKill = OnPause = null;

            BaseDuration = 0;
            Loops = 1;
            Delay = 0;
            Easer = Ease.Linear.GetEaser();
            Parent = null;
            Locked = StartupDone = PlayedOnce = false;
            Position = CompletedLoops = 0;
            IsPlaying = IsComplete = false;
            ElapsedDelay = 0;
        }

        /// <summary>
        /// Validate that this animation's values are correct
        /// </summary>
        /// <returns></returns>
        internal abstract bool Validate();

        /// <summary>
        /// Called by AnimManager in case an animation has a delay that needs to be
        /// updated. Returns the eventual time in excess compared to the animation's
        /// delay time.
        /// </summary>
        internal virtual float UpdateDelay(float elapsed)
        {
            return 0;
        }
        
        /// <summary>
        /// Called the moment the animation starts. For tweens, that means AFTER any delay has elapsed.
        /// </summary>
        /// <returns>TRUE in case of success, FALSE if there are missing references and the animation
        /// needs to be killed</returns>
        internal abstract bool Startup();

        /// <summary>
        /// Applies the animation set by DoGoto.
        /// </summary>
        /// <returns>TRUE if the animation needs to be killed</returns>
        internal abstract bool ApplyAnimation(float prevPosition, int prevCompletedLoops, int newCompletedSteps,
            bool useInversePosition);

        #region Goto

        internal static bool DoGoto(Animation t, float toPosition, int toCompletedLoops, bool withCallbacks = true)
        {
            if (!t.StartupDone)
            {
                if (!t.Startup()) return true;
            }

            if (!t.PlayedOnce)
            {
                t.PlayedOnce = true;
                if (t.OnStart != null && withCallbacks)
                {
                    t.OnStart.Invoke();
                    if (!t.Active) return true; // animation might have been killed by OnStart callback
                }

                if (t.OnPlay != null && withCallbacks)
                {
                    t.OnPlay.Invoke();
                    if (!t.Active) return true; // animation might have been killed by OnStart callback
                }
            }

            float prevPosition = t.Position;
            int prevCompletedLoops = t.CompletedLoops;
            t.CompletedLoops = toCompletedLoops;
            bool wasRewinded = t.Position <= 0 && prevCompletedLoops <= 0;
            bool wasComplete = t.IsComplete;
            if (t.Loops != -1) t.IsComplete = t.CompletedLoops == t.Loops;

            int newCompletedSteps = 0;
            if (t.IsBackwards) {
                newCompletedSteps = t.CompletedLoops < prevCompletedLoops ? prevCompletedLoops - t.CompletedLoops : (toPosition <= 0 && !wasRewinded ? 1 : 0);
                if (wasComplete) newCompletedSteps--;
            } else newCompletedSteps = t.CompletedLoops > prevCompletedLoops ? t.CompletedLoops - prevCompletedLoops : 0;
            
            t.Position = toPosition;
            if (t.Position > t.BaseDuration) t.Position = t.BaseDuration;
            else if (t.Position <= 0)
            {
                if (t.CompletedLoops > 0 || t.IsComplete) t.Position = t.BaseDuration;
                else t.Position = 0;
            }

            bool wasPlaying = t.IsPlaying;
            if (t.IsPlaying)
            {
                if (!t.IsBackwards) t.IsPlaying = !t.IsComplete; // Reached the end
                else t.IsPlaying = !(t.CompletedLoops == 0 && t.Position <= 0); // Rewinded
            }

            bool useInversePosition = t.HasLoops && t.LoopType == LoopType.Yoyo
                                                 && (t.Position < t.BaseDuration
                                                     ? t.CompletedLoops % 2 != 0
                                                     : t.CompletedLoops % 2 == 0);
            if (t.ApplyAnimation(prevPosition, prevCompletedLoops, newCompletedSteps, useInversePosition)) return true;
            
            if (withCallbacks) t.OnUpdate?.Invoke();
            if (t.IsComplete && !wasComplete && withCallbacks)
            {
                t.OnComplete?.Invoke();
            }

            if (!t.IsPlaying && wasPlaying && (!t.IsComplete || !t.AutoKill) && withCallbacks)
            {
                t.OnPause?.Invoke();
            }

            return t.AutoKill && t.IsComplete;
        }

        #endregion

        #region Runtime Operations

        public void Complete(bool withCallbacks = false)
        {
            if (!Active || IsSequenced)
            {
                // TODO: Log error
                return;
            }

            AnimManager.Complete(this, withCallbacks);
        }

        public void Flip()
        {
            AnimManager.Flip(this);
        }

        public void ForceInit()
        {
            AnimManager.ForceInit(this);
        }

        public void Goto(float to, bool andPlay = false)
        {
            if (to < 0) to = 0;
            if (!StartupDone) AnimManager.ForceInit(this);
            AnimManager.Goto(this, to, andPlay);
        }

        public void Kill(bool complete = false)
        {
            if (complete)
            {
                AnimManager.Complete(this);
                if (AutoKill && Loops >= 0) return;
            }

            if (AnimManager.IsUpdateLoop)
            {
                Active = false;
            }
            else
            {
                AnimManager.Despawn(this);
            }
        }

        public void PlayBackwards()
        {
            AnimManager.PlayBackwards(this);
        }

        public void PlayForward()
        {
            AnimManager.PlayForward(this);
        }

        public void Restart(bool includeDelay = true, float changeDelayTo = -1)
        {
            AnimManager.Restart(this, includeDelay, changeDelayTo);
        }

        public void Rewind(bool includeDelay = true)
        {
            AnimManager.Rewind(this, includeDelay);
        }

        public void SmoothRewind()
        {
            AnimManager.SmoothRewind(this);
        }

        public void TogglePause()
        {
            AnimManager.TogglePause(this);
        }

        #endregion

        #region Yield Coroutines

        public YieldInstruction WaitForCompletion()
        {
            return new WaitWhile(() => Active && !IsComplete);
        }

        public YieldInstruction WaitForKill()
        {
            return new WaitWhile(() => Active);
        }

        public YieldInstruction WaitForElapsedLoops(int elapsedLoops)
        {
            return new WaitWhile(() => Active && CompletedLoops < elapsedLoops);
        }

        public YieldInstruction WaitForPosition(float position)
        {
            return new WaitWhile(() => Active && Position * (CompletedLoops + 1) < position);
        }

        public YieldInstruction WaitForStart()
        {
            return new WaitWhile(() => Active && !PlayedOnce);
        }

        #endregion

        #region Info Getters

        public float Duration(bool includeLoops = true)
        {
            if (includeLoops) return Loops == -1 ? Mathf.INFINITY : BaseDuration * Loops;
            return BaseDuration;
        }

        public float Elapsed(bool includeLoops = true)
        {
            if (includeLoops)
            {
                int loopsToCount = Position >= BaseDuration ? CompletedLoops - 1 : CompletedLoops;
                return (loopsToCount * BaseDuration) + Position;
            }

            return Position;
        }

        public float ElapsedPercentage(bool includeLoops = true)
        {
            if (includeLoops)
            {
                if (FullDuration <= 0) return 0;
                int loopsToCount = Position >= BaseDuration ? CompletedLoops - 1 : CompletedLoops;
                return ((loopsToCount * BaseDuration) + Position) / FullDuration;
            }

            return Position / BaseDuration;
        }

        public float ElapsedDirectionalPercentage()
        {
            float perc = Position / BaseDuration;
            bool isInverse = CompletedLoops > 0 && HasLoops && LoopType == LoopType.Yoyo
                             && (!IsComplete && CompletedLoops % 2 != 0 || IsComplete && CompletedLoops % 2 == 0);
            return isInverse ? 1 - perc : perc;
        }
        
        #endregion
    }

    public static class SequentiableExtensions
    {
        #region Chaining Property Setters

        public static T? SetAutoKill<T>(this T? t, bool autoKillOnCompletion = true) where T : Animation
        {
            if (t == null || !t.Active || t.Locked) return t;

            t.AutoKill = autoKillOnCompletion;
            return t;
        }

        public static T? SetId<T>(this T? t, int intId) where T : Tween
        {
            if (t == null || !t.Active) return t;

            t.Id = intId;
            return t;
        }

        public static T? SetLoops<T>(this T? t, int loops) where T : Tween
        {
            if (t == null || !t.Active || t.Locked) return t;

            if (loops < -1) loops = -1;
            else if (loops == 0) loops = 1;
            t.Loops = loops;
            if (t.AnimationType == AnimationType.Tween)
            {
                if (loops > -1) t.FullDuration = t.BaseDuration * loops;
                else t.FullDuration = Mathf.INFINITY;
            }

            return t;
        }
        
        public static T? SetLoops<T>(this T? t, int loops, LoopType loopType) where T : Tween
        {
            if (t == null || !t.Active || t.Locked) return t;

            if (loops < -1) loops = -1;
            else if (loops == 0) loops = 1;
            t.Loops = loops;
            t.LoopType = loopType;
            if (t.AnimationType == AnimationType.Tween)
            {
                if (loops > -1) t.FullDuration = t.BaseDuration * loops;
                else t.FullDuration = Mathf.INFINITY;
            }

            return t;
        }

        public static T? SetEase<T>(this T? t, Ease ease) where T : Tween
        {
            if (t == null || !t.Active) return t;

            t.Easer = ease.GetEaser();
            return t;
        }
        
        public static T? SetEase<T>(this T? t, Easer easer) where T : Tween
        {
            if (t == null || !t.Active) return t;

            t.Easer = easer;
            return t;
        }

        public static T? SetRecyclable<T>(this T t, bool recyclable = true) where T : Tween
        {
            if (t == null || !t.Active) return t;
            t.Recyclable = recyclable;
            return t;
        }

        public static T? SetUpdate<T>(this T? t, bool isIndependentUpdate) where T : Tween
        {
            if (t == null || !t.Active) return t;
            return SetUpdate(t, TweenSubsystem.DefaultUpdateType, isIndependentUpdate);
        }
        
        public static T? SetUpdate<T>(this T? t, UpdateType updateType) where T : Tween
        {
            if (t == null || !t.Active) return t;
            return SetUpdate(t, updateType, TweenSubsystem.DefaultTimeScaleIndependent);
        }
        
        public static T? SetUpdate<T>(this T? t, UpdateType updateType, bool isIndependentUpdate) where T : Tween
        {
            if (t == null || !t.Active) return t;
            AnimManager.SetUpdateType(t, updateType, isIndependentUpdate);
            return t;
        }

        #endregion

        #region Callback Shortcuts

        public static T? OnStart<T>(this T? t, TweenCallback action) where T : Tween
        {
            if (t == null || !t.Active) return t;

            t.OnStart = action;
            return t;
        }

        public static T? OnPlay<T>(this T? t, TweenCallback action) where T : Tween
        {
            if (t == null || !t.Active) return t;

            t.OnPlay = action;
            return t;
        }
        
        public static T? OnPause<T>(this T? t, TweenCallback action) where T : Tween
        {
            if (t == null || !t.Active) return t;

            t.OnPause = action;
            return t;
        }

        public static T? OnUpdate<T>(this T? t, TweenCallback action) where T : Tween
        {
            if (t == null || !t.Active) return t;

            t.OnUpdate = action;
            return t;
        }

        public static T? OnStop<T>(this T? t, TweenCallback action) where T : Tween
        {
            if (t == null || !t.Active) return t;

            t.OnComplete = action;
            return t;
        }
        
        public static T? OnKill<T>(this T? t, TweenCallback action) where T : Tween
        {
            if (t == null || !t.Active) return t;

            t.OnKill = action;
            return t;
        }

        #endregion

        #region Sequences-Only

        /// <summary>Adds the given tween to the end of the Sequence. 
        /// Has no effect if the Sequence has already started</summary>
        /// <param name="t">The tween to append</param>
        public static Sequence Append(this Sequence s, Tween t)
        {
            if (s == null || !s.Active || s.Locked) return s;
            if (t == null || !t.Active || t.IsSequenced) return s;

            Sequence.DoInsert(s, t, s.BaseDuration);
            return s;
        }
        /// <summary>Adds the given tween to the beginning of the Sequence, pushing forward the other nested content. 
        /// Has no effect if the Sequence has already started</summary>
        /// <param name="t">The tween to prepend</param>
        public static Sequence Prepend(this Sequence s, Tween t)
        {
            if (s == null || !s.Active || s.Locked) return s;
            if (t == null || !t.Active || t.IsSequenced) return s;

            Sequence.DoPrepend(s, t);
            return s;
        }
        /// <summary>Inserts the given tween at the same time position of the last tween, callback or intervale added to the Sequence.
        /// Note that, in case of a Join after an interval, the insertion time will be the time where the interval starts, not where it finishes.
        /// Has no effect if the Sequence has already started</summary>
        public static Sequence Join(this Sequence s, Tween t)
        {
            if (s == null || !s.Active || s.Locked) return s;
            if (t == null || !t.Active || t.IsSequenced) return s;

            Sequence.DoInsert(s, t, s.LastAnimationInsertTime);
            return s;
        }
        /// <summary>Inserts the given tween at the given time position in the Sequence,
        /// automatically adding an interval if needed. 
        /// Has no effect if the Sequence has already started</summary>
        /// <param name="atPosition">The time position where the tween will be placed</param>
        /// <param name="t">The tween to insert</param>
        public static Sequence Insert(this Sequence s, float atPosition, Tween t)
        {
            if (s == null || !s.Active || s.Locked) return s;
            if (t == null || !t.Active || t.IsSequenced) return s;

            Sequence.DoInsert(s, t, atPosition);
            return s;
        }

        /// <summary>Adds the given interval to the end of the Sequence. 
        /// Has no effect if the Sequence has already started</summary>
        /// <param name="interval">The interval duration</param>
        public static Sequence AppendInterval(this Sequence s, float interval)
        {
            if (s == null || !s.Active || s.Locked) return s;

            Sequence.DoAppendInterval(s, interval);
            return s;
        }
        /// <summary>Adds the given interval to the beginning of the Sequence, pushing forward the other nested content. 
        /// Has no effect if the Sequence has already started</summary>
        /// <param name="interval">The interval duration</param>
        public static Sequence PrependInterval(this Sequence s, float interval)
        {
            if (s == null || !s.Active || s.Locked) return s;

            Sequence.DoPrependInterval(s, interval);
            return s;
        }

        #endregion
        
        public static T? Pause<T>(this T? t) where T : Animation
        {
            // TODO: Log warning here?
            if (t == null || !t.Active || t.IsSequenced) return t;
            
            AnimManager.Pause(t);
            return t;
        }

        public static T? Play<T>(this T? t) where T : Animation
        {
            // TODO: Log warning here?
            if (t == null || !t.Active || t.IsSequenced) return t;
            
            AnimManager.Play(t);
            return t;
        }
    }
}