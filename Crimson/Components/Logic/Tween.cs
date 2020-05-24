using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Crimson
{
    public class Tween : Component
    {
        public enum TweenMode
        {
            Persist,
            Oneshot,
            Looping,
            YoyoOneshot,
            YoyoLooping
        }

        public Ease.Easer? Easer;
        public Action<Tween>? OnComplete;
        public Action<Tween>? OnStart;
        public Action<Tween>? OnUpdate;

        private bool _startedReversed;
        public bool UseRawDeltaTime;

        private Tween()
            : base(false, false)
        {
        }

        public TweenMode Mode { get; private set; }
        public float Duration { get; private set; }
        public float TimeLeft { get; private set; }
        public float Percent { get; private set; }
        public float Eased { get; private set; }
        public bool Reverse { get; private set; }

        public float Inverted => 1f - Eased;

        private void Init(TweenMode mode, Ease.Easer easer, float duration, bool start)
        {
#if DEBUG
            if (duration <= 0) throw new Exception("Tween duration cannot be less than zero");
#else
            if (duration <= 0)
                duration = .000001f;
#endif

            UseRawDeltaTime = false;
            Mode = mode;
            Easer = easer;
            Duration = duration;

            TimeLeft = 0;
            Percent = 0;
            Active = false;

            if (start) Start();
        }

        public override void Removed(Entity entity)
        {
            base.Removed(entity);
            Cached.Push(this);
        }

        public override void Update()
        {
            TimeLeft -= UseRawDeltaTime ? Time.RawDeltaTime : Time.DeltaTime;

            //Update the percentage and eased percentage
            Percent = Mathf.Max(0, TimeLeft) / Duration;
            if (!Reverse) Percent = 1 - Percent;

            if (Easer != null)
                Eased = Easer(Percent);
            else
                Eased = Percent;

            //Update the tween
            if (OnUpdate != null) OnUpdate(this);

            //When finished...
            if (TimeLeft <= 0)
            {
                TimeLeft = 0;

                if (OnComplete != null) OnComplete(this);

                switch (Mode)
                {
                    case TweenMode.Persist:
                        Active = false;
                        break;

                    case TweenMode.Oneshot:
                        Active = false;
                        RemoveSelf();
                        break;

                    case TweenMode.Looping:
                        Start(Reverse);
                        break;

                    case TweenMode.YoyoOneshot:
                        if (Reverse == _startedReversed)
                        {
                            Start(!Reverse);
                            _startedReversed = !Reverse;
                        }
                        else
                        {
                            Active = false;
                            RemoveSelf();
                        }

                        break;

                    case TweenMode.YoyoLooping:
                        Start(!Reverse);
                        break;
                }
            }
        }

        public void Start()
        {
            Start(false);
        }

        public void Start(bool reverse)
        {
            _startedReversed = Reverse = reverse;

            TimeLeft = Duration;
            Eased = Percent = Reverse ? 1 : 0;

            Active = true;

            if (OnStart != null) OnStart(this);
        }

        public void Start(float duration, bool reverse = false)
        {
#if DEBUG
            if (duration <= 0) throw new Exception("Tween duration cannot be <= 0");
#endif

            Duration = duration;
            Start(reverse);
        }

        public void Stop()
        {
            Active = false;
        }

        public void Reset()
        {
            TimeLeft = Duration;
            Eased = Percent = Reverse ? 1 : 0;
        }

        public IEnumerator Wait()
        {
            while (Active) yield return 0;
        }

        #region Static

        private static readonly Stack<Tween> Cached = new Stack<Tween>();

        public static Tween Create(TweenMode mode, Ease.Easer easer = null, float duration = 1f, bool start = false)
        {
            Tween tween;
            if (Cached.Count == 0)
                tween = new Tween();
            else
                tween = Cached.Pop();

            tween.OnUpdate = tween.OnComplete = tween.OnStart = null;

            tween.Init(mode, easer, duration, start);
            return tween;
        }

        public static Tween Set(
            Entity entity,
            TweenMode tweenMode,
            float duration,
            Ease.Easer easer,
            Action<Tween> onUpdate,
            Action<Tween> onComplete = null
        )
        {
            var tween = Create(tweenMode, easer, duration, true);
            tween.OnUpdate += onUpdate;
            tween.OnComplete += onComplete;
            entity.Add(tween);
            return tween;
        }

        public static Tween Position(
            Entity entity,
            Vector2 targetPosition,
            float duration,
            Ease.Easer easer,
            TweenMode tweenMode = TweenMode.Oneshot
        )
        {
            var startPosition = entity.Position;
            var tween = Create(tweenMode, easer, duration, true);
            tween.OnUpdate = t => { entity.Position = Vector2.Lerp(startPosition, targetPosition, t.Eased); };
            entity.Add(tween);
            return tween;
        }

        #endregion
    }
}