using System;

namespace Crimson.Tweening
{
    public abstract class TweenBase
    {
        /// <summary>
        /// The type of tween
        /// </summary>
        internal TweenType TweenType;
        /// <summary>
        /// Position in sequence
        /// </summary>
        internal float SequencedPosition;
        /// <summary>
        /// End position in sequence
        /// </summary>
        internal float SequencedEndPosition;
        /// <summary>
        /// Called the first time the tween is set in a playing state,
        /// after any eventual delay
        /// </summary>
        internal TweenCallback OnStart;
        
        public float TimeScale;
        public bool IsBackwards;
        public int? Id;
        public UpdateType UpdateType;
        internal bool IsIndependentUpdate;

        public TweenCallback OnPlay;
        public TweenCallback OnPause;
        public TweenCallback OnRewind;
        public TweenCallback OnUpdate;
        public TweenCallback OnStepComplete;
        public TweenCallback OnComplete;
        public TweenCallback OnKill;

        internal float Duration;
        internal int Loops;
        internal LoopType LoopType;
        internal float Delay;
        internal Easer Easer;

        public bool Active;
        internal bool IsSequenced;
        internal Sequence? SequenceParent;
        internal int ActiveId = -1;

        public float FullPosition
        {
            get => this.Elapsed(true);
            set => this.Goto(value, IsPlaying);
        }

        public bool HasLoops => Loops == -1 || Loops > 1;

        internal bool CreationLocked;
        internal bool StartupDone;
        public bool PlayedOnce { get; private set; }
        public float Position { get; internal set; }
        internal float FullDuration;
        internal int CompletedLoops;
        internal bool IsPlaying;
        internal bool IsComplete;
        internal float ElapsedDelay;
        internal bool DelayComplete = true;

        internal virtual void Reset()
        {
            TimeScale = 1;
            IsBackwards = false;
            Id = null;
            IsIndependentUpdate = false;
            OnStart = OnPlay = OnRewind = OnUpdate = OnComplete = OnStepComplete = OnKill = null;

            Duration = 0;
            Loops = 1;
            Delay = 0;
            IsSequenced = false;
            SequenceParent = null;
            CreationLocked = StartupDone = PlayedOnce = false;
            Position = FullDuration = CompletedLoops = 0;
            IsPlaying = IsComplete = false;
            ElapsedDelay = 0;
            DelayComplete = true;
        }

        internal virtual float UpdateDelay(float elapsed)
        {
            return 0;
        }

        internal abstract bool Startup();

        internal abstract bool ApplyTween(float prevPosition, int prevCompletedLoops, int newCompletedSteps,
            bool useInversePosition);
        
    }
}