using System;
using Microsoft.Xna.Framework;

namespace Crimson
{
    public class SineWave : Component
    {
        private float counter;
        /*
         *    SINE WAVE:
         * 
         *  1       x      
         *  |    x     x   
         *  |  x         x 
         *  | x           x
         *  -x-------------x-------------x
         *  |               x           x
         *  |                x         x
         *  |                  x     x
         * -1                     x
         * 
         *     COS WAVE:
         * 
         *  1x                           x
         *  |   x                     x
         *  |     x                 x
         *  |      x               x
         *  --------x-------------x-------
         *  |        x           x
         *  |         x         x
         *  |           x     x
         * -1              x
         * 
         */

        public float Frequency = 1f;
        public Action<float> OnUpdate;
        public float Rate = 1f;
        public bool UseRawDeltaTime;

        public SineWave()
            : base(true, false)
        {
        }

        public SineWave(float frequency)
            : this()
        {
            Frequency = frequency;
        }

        public float Value { get; private set; }
        public float ValueOverTwo { get; private set; }
        public float TwoValue { get; private set; }

        public float Counter
        {
            get => counter;

            set
            {
                counter = (value + MathHelper.TwoPi * 4) % (MathHelper.TwoPi * 4);

                Value = (float) Math.Sin(counter);
                ValueOverTwo = (float) Math.Sin(counter / 2);
                TwoValue = (float) Math.Sin(counter * 2);
            }
        }

        public override void Update()
        {
            Counter += Mathf.TAU * Frequency * Rate * (UseRawDeltaTime ? Time.RawDeltaTime : Time.DeltaTime);
            if (OnUpdate != null)
                OnUpdate(Value);
        }

        public float ValueOffset(float offset)
        {
            return Mathf.Sin(counter + offset);
        }

        public SineWave Randomize()
        {
            Counter = Utils.Random.NextFloat() * Mathf.TAU * 2;
            return this;
        }

        public void Reset()
        {
            Counter = 0;
        }

        public void StartUp()
        {
            Counter = MathHelper.PiOver2;
        }

        public void StartDown()
        {
            Counter = MathHelper.PiOver2 * 3f;
        }
    }
}