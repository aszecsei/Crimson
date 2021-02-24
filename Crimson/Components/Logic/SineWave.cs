using System;
using Microsoft.Xna.Framework;

namespace Crimson
{
    public class SineWave : Component
    {
        public float         Frequency = 1f;
        public float         Rate      = 1f;
        public Action<float> OnUpdate;
        public bool          UseRawDeltaTime;

        private float _counter;

        public float Value        { get; private set; }
        public float ValueOverTwo { get; private set; }
        public float TwoValue     { get; private set; }

        public float Counter
        {
            get
            {
                return _counter;
            }
            set
            {
                _counter     = (value + Mathf.PI * 8f) % (Mathf.PI * 8f);
                Value        = Mathf.Sin(_counter);
                ValueOverTwo = Mathf.Sin(_counter / 2);
                TwoValue     = Mathf.Sin(_counter * 2);
            }
        }

        public SineWave() : base(true, false)
        {

        }

        public SineWave(float frequency, float offset = 0f) : this()
        {
            Frequency = frequency;
            Counter   = offset;
        }

        public override void Update()
        {
            Counter += Mathf.TAU * Frequency * Rate * (UseRawDeltaTime ? Time.RawDeltaTime : Time.DeltaTime);
            OnUpdate?.Invoke(Value);
        }

        public float ValueOffset(float offset)
        {
            return Mathf.Sin(_counter + offset);
        }

        public SineWave Randomize()
        {
            Counter = Utils.Random.NextFloat() * Mathf.TAU * 2f;
            return this;
        }

        public void Reset()
        {
            Counter = 0f;
        }

        public void StartUp()
        {
            Counter = Mathf.PI_OVER_TWO;
        }

        public void StartDown()
        {
            Counter = Mathf.PI - Mathf.PI_OVER_TWO;
        }
    }
}
