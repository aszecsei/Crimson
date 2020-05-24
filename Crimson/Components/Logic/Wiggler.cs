using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Crimson
{
    public class Wiggler : Component
    {
        private static readonly Stack<Wiggler> Cache = new Stack<Wiggler>();

        private float _increment;
        private Action<float>? _onChange;
        private bool _removeSelfOnFinish;
        private float _sineAdd;

        private float _sineCounter;
        public bool StartZero = false;
        public bool UseRawDeltaTime;

        private Wiggler()
            : base(false, false)
        {
        }

        public float Counter { get; private set; }
        public float Value { get; private set; }

        public static Wiggler Create(
            float duration,
            float frequency,
            Action<float>? onChange = null,
            bool start = false,
            bool removeSelfOnFinish = false
        )
        {
            Wiggler wiggler = Cache.Count > 0 ? Cache.Pop() : new Wiggler();
            wiggler.Init(duration, frequency, onChange, start, removeSelfOnFinish);

            return wiggler;
        }

        private void Init(float duration, float frequency, Action<float>? onChange, bool start, bool removeSelfOnFinish)
        {
            Counter = _sineCounter = 0;
            UseRawDeltaTime = false;

            _increment = 1f / duration;
            _sineAdd = Mathf.TAU * frequency;
            this._onChange = onChange;
            this._removeSelfOnFinish = removeSelfOnFinish;

            if (start)
                Start();
            else
                Active = false;
        }

        public override void Removed(Entity entity)
        {
            base.Removed(entity);
            Cache.Push(this);
        }

        public void Start()
        {
            Counter = 1f;

            if (StartZero)
            {
                _sineCounter = MathHelper.PiOver2;
                Value = 0;
                _onChange?.Invoke(0);
            }
            else
            {
                _sineCounter = 0;
                Value = 1f;
                _onChange?.Invoke(1f);
            }

            Active = true;
        }

        public void Start(float duration, float frequency)
        {
            _increment = 1f / duration;
            _sineAdd = MathHelper.TwoPi * frequency;
            Start();
        }

        public void Stop()
        {
            Active = false;
        }

        public void StopAndClear()
        {
            Stop();
            Value = 0;
        }

        public override void Update()
        {
            if (UseRawDeltaTime)
            {
                _sineCounter += _sineAdd * Time.RawDeltaTime;
                Counter -= _increment * Time.RawDeltaTime;
            }
            else
            {
                _sineCounter += _sineAdd * Time.DeltaTime;
                Counter -= _increment * Time.DeltaTime;
            }

            if (Counter <= 0)
            {
                Counter = 0;
                Active = false;
                if (_removeSelfOnFinish)
                    RemoveSelf();
            }

            Value = (float) Math.Cos(_sineCounter) * Counter;

            _onChange?.Invoke(Value);
        }
    }
}