namespace Crimson
{
    public static class Ease
    {
        public delegate float Easer(float t);

        private const float B1 = 1f / 2.75f;
        private const float B2 = 2f / 2.75f;
        private const float B3 = 1.5f / 2.75f;
        private const float B4 = 2.5f / 2.75f;
        private const float B5 = 2.25f / 2.75f;
        private const float B6 = 2.625f / 2.75f;

        public static readonly Easer Linear = t => { return t; };

        public static readonly Easer SineIn = t => { return -Mathf.Cos(Mathf.PI_OVER_TWO * t) + 1; };
        public static readonly Easer SineOut = t => { return Mathf.Sin(Mathf.PI_OVER_TWO * t); };
        public static readonly Easer SineInOut = t => { return -Mathf.Cos(Mathf.PI * t) / 2f + .5f; };

        public static readonly Easer QuadIn = t => { return t * t; };
        public static readonly Easer QuadOut = Invert(QuadIn);
        public static readonly Easer QuadInOut = Follow(QuadIn, QuadOut);

        public static readonly Easer CubeIn = t => { return t * t * t; };
        public static readonly Easer CubeOut = Invert(CubeIn);
        public static readonly Easer CubeInOut = Follow(CubeIn, CubeOut);

        public static readonly Easer QuintIn = t => { return t * t * t * t * t; };
        public static readonly Easer QuintOut = Invert(QuintIn);
        public static readonly Easer QuintInOut = Follow(QuintIn, QuintOut);

        public static readonly Easer ExpoIn = t => { return Mathf.Pow(2, 10 * (t - 1)); };
        public static readonly Easer ExpoOut = Invert(ExpoIn);
        public static readonly Easer ExpoInOut = Follow(ExpoIn, ExpoOut);

        public static readonly Easer BackIn = t => { return t * t * (2.70158f * t - 1.70158f); };
        public static readonly Easer BackOut = Invert(BackIn);
        public static readonly Easer BackInOut = Follow(BackIn, BackOut);

        public static readonly Easer BigBackIn = t => { return t * t * (4f * t - 3f); };
        public static readonly Easer BigBackOut = Invert(BigBackIn);
        public static readonly Easer BigBackInOut = Follow(BigBackIn, BigBackOut);

        public static readonly Easer ElasticIn = t =>
        {
            var ts = t * t;
            var tc = ts * t;
            return 33 * tc * ts + -59 * ts * ts + 32 * tc + -5 * ts;
        };

        public static readonly Easer ElasticOut = t =>
        {
            var ts = t * t;
            var tc = ts * t;
            return 33 * tc * ts + -106 * ts * ts + 126 * tc + -67 * ts + 15 * t;
        };

        public static readonly Easer ElasticInOut = Follow(ElasticIn, ElasticOut);

        public static readonly Easer BounceIn = t =>
        {
            t = 1 - t;
            if (t < B1) return 1 - 7.5625f * t * t;

            if (t < B2) return 1 - (7.5625f * (t - B3) * (t - B3) + .75f);

            if (t < B4) return 1 - (7.5625f * (t - B5) * (t - B5) + .9375f);

            return 1 - (7.5625f * (t - B6) * (t - B6) + .984375f);
        };

        public static readonly Easer BounceOut = t =>
        {
            if (t < B1) return 7.5625f * t * t;

            if (t < B2) return 7.5625f * (t - B3) * (t - B3) + .75f;

            if (t < B4) return 7.5625f * (t - B5) * (t - B5) + .9375f;

            return 7.5625f * (t - B6) * (t - B6) + .984375f;
        };

        public static readonly Easer BounceInOut = t =>
        {
            if (t < .5f)
            {
                t = 1 - t * 2;
                if (t < B1) return (1 - 7.5625f * t * t) / 2;

                if (t < B2) return (1 - (7.5625f * (t - B3) * (t - B3) + .75f)) / 2;

                if (t < B4) return (1 - (7.5625f * (t - B5) * (t - B5) + .9375f)) / 2;

                return (1 - (7.5625f * (t - B6) * (t - B6) + .984375f)) / 2;
            }

            t = t * 2 - 1;
            if (t < B1) return 7.5625f * t * t / 2 + .5f;

            if (t < B2) return (7.5625f * (t - B3) * (t - B3) + .75f) / 2 + .5f;

            if (t < B4) return (7.5625f * (t - B5) * (t - B5) + .9375f) / 2 + .5f;

            return (7.5625f * (t - B6) * (t - B6) + .984375f) / 2 + .5f;
        };

        public static Easer Invert(Easer easer)
        {
            return t => { return 1 - easer(1 - t); };
        }

        public static Easer Follow(Easer first, Easer second)
        {
            return t => { return t <= 0.5f ? first(t * 2) / 2 : second(t * 2 - 1) / 2 + 0.5f; };
        }

        public static float UpDown(float eased)
        {
            if (eased <= .5f) return eased * 2;

            return 1 - (eased - .5f) * 2;
        }
    }
}