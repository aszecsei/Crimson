using System;
using System.Collections.Generic;

namespace Crimson
{
    public class Wiggler : Component
    {
        private static Stack<Wiggler> s_cache = new Stack<Wiggler>();

        public bool StartZero;
        public bool UseRawDeltaTime;

        private float         _sineCounter;
        private float         _increment;
        private float         _sineAdd;
        private Action<float> _onChange;
        private bool          _removeSelfOnFinish;

        public float Counter { get; private set; }
        public float Value   { get; private set; }

        public static Wiggler Create(
            float         duration,
            float         frequency,
            Action<float> onChange           = null,
            bool          start              = false,
            bool          removeSelfOnFinish = false
        )
        {
            Wiggler wiggler = ((s_cache.Count <= 0) ? new Wiggler() : s_cache.Pop());
            wiggler.Init(duration, frequency, onChange, start, removeSelfOnFinish);
            return wiggler;
        }

        private Wiggler() : base(false, false)
        {

        }

        private void Init(
            float         duration,
            float         frequency,
            Action<float> onChange,
            bool          start,
            bool          removeSelfOnFinish
        )
        {
            Counter             = _sineCounter = 0f;
            UseRawDeltaTime     = false;
            _increment          = 1f               / duration;
            _sineAdd            = (float)Mathf.TAU * frequency;
            _onChange           = onChange;
            _removeSelfOnFinish = removeSelfOnFinish;
            if ( start )
            {
                Start();
            }
            else
            {
                Active = false;
            }
        }

        public override void Removed(Entity entity)
        {
            base.Removed(entity);
            s_cache.Push(this);
        }

        public void Start()
        {
            Counter = 1f;
            if ( StartZero )
            {
                _sineCounter = Mathf.QUARTER_CIRCLE;
                Value        = 0f;
                _onChange?.Invoke(0f);
            }
            else
            {
                _sineCounter = 0f;
                Value        = 1f;
                _onChange?.Invoke(1f);
            }

            Active = true;
        }

        public void Start(float duration, float frequency)
        {
            _increment = 1f        / duration;
            _sineAdd   = Mathf.TAU * frequency;
            Start();
        }

        public void Stop()
        {
            Active = false;
        }

        public void StopAndClear()
        {
            Stop();
            Value = 0f;
        }

        public override void Update()
        {
            if ( UseRawDeltaTime )
            {
                _sineCounter += _sineAdd   * Time.RawDeltaTime;
                Counter      -= _increment * Time.RawDeltaTime;
            }
            else
            {
                _sineCounter += _sineAdd   * Time.DeltaTime;
                Counter      -= _increment * Time.DeltaTime;
            }

            if ( Counter <= 0f )
            {
                Counter = 0f;
                Active  = false;
                if ( _removeSelfOnFinish )
                {
                    RemoveSelf();
                }
            }

            Value = Mathf.Cos(_sineCounter) * Counter;
            _onChange?.Invoke(Value);
        }
    }
}
