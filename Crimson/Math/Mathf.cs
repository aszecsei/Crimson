using System;
using Microsoft.Xna.Framework;
using System.Runtime.CompilerServices;

namespace Crimson
{
    public static class Mathf
    {
        #region Simplex Noise

        private class SimplexNoise
        {
            private static readonly int[,] Grad3 =
            {
                {1, 1, 0}, {-1, 1, 0}, {1, -1, 0}, {-1, -1, 0},
                {1, 0, 1}, {-1, 0, 1}, {1, 0, -1}, {-1, 0, -1},
                {0, 1, 1}, {0, -1, 1}, {0, 1, -1}, {0, -1, -1}
            };

            private readonly int[] _perm;

            public SimplexNoise(float seed)
            {
                var p = new int[256];
                for (var i = 0; i < 256; i++) p[i] = FloorToInt(seed * 256);

                // To remove the need for index wrapping, double the permutation table length
                _perm = new int[512];
                for (var i = 0; i < 512; i++) _perm[i] = p[i & 255];
            }

            private static int FastFloor(float x)
            {
                return x > 0 ? (int) x : (int) x - 1;
            }

            private static float Dot(int[,] g, int idx, float x, float y)
            {
                return g[idx, 0] * x + g[idx, 1] * y;
            }

            /// <summary>
            ///     2D simplex noise
            /// </summary>
            /// <param name="xin"></param>
            /// <param name="yin"></param>
            /// <returns></returns>
            public float Noise(float xin, float yin)
            {
                // ReSharper disable InconsistentNaming

                float n0, n1, n2; // Noise contributions from the three corners
                // Skew the input space to determine which simplex cell we're in

                var F2 = 0.5F * (Sqrt(3F) - 1F);

                var s = (xin + yin) * F2;
                var i = FastFloor(xin + s);
                var j = FastFloor(yin + s);

                var G2 = (3F - Sqrt(3F)) / 6F;
                var t = (i + j) * G2;
                var X0 = i - t; // unskew the cell origin back to (x, y) space
                var Y0 = j - t;
                var x0 = xin - X0; // the x, y distances from the cell origin
                var y0 = yin - Y0;

                // For the 2D case, the simplex shape is an equilateral triangle.
                // Determine which simplex we are in.
                int i1, j1; // offsets for second (middle) corner of simplex in (i, j) coords
                if (x0 > y0)
                {
                    // Lower triangle, XY order: (0,0)->(1,0)->(1,1)
                    i1 = 1;
                    j1 = 0;
                }
                else
                {
                    // Upper triangle, YX order: (0,0)->(0,1)->(1,1)
                    i1 = 0;
                    j1 = 1;
                }

                // A step of (1,0) in (i,j) means a step of (1-c,-c) in (x,y), and
                // a step of (0,1) in (i,j) means a step of (-c,1-c) in (x,y), where
                // c = (3-sqrt(3))/6

                var x1 = x0 - i1 + G2; // Offsets for middle corner in (x, y) unskewed coords
                var y1 = y0 - j1 + G2;
                var x2 = x0 - 1F + 2F * G2; // Offsets for last corner in (x, y) unskewed coords
                var y2 = y0 - 1F + 2F * G2;

                // Work out the hashed gradient indices of the three simplex corners
                var ii = i & 255;
                var jj = j & 255;
                var gi0 = _perm[ii + _perm[jj]] % 12;
                var gi1 = _perm[ii + i1 + _perm[jj + j1]] % 12;
                var gi2 = _perm[ii + 1 + _perm[jj + 1]] % 12;

                // Calculate the contribution from the three corners
                var t0 = 0.5F - x0 * x0 - y0 * y0;
                if (t0 < 0)
                {
                    n0 = 0;
                }
                else
                {
                    t0 *= t0;
                    n0 = t0 * t0 * Dot(Grad3, gi0, x0, x0);
                }

                var t1 = 0.5F - x1 * x1 - y1 * y1;
                if (t1 < 0)
                {
                    n1 = 0F;
                }
                else
                {
                    t1 *= t1;
                    n1 = t1 * t1 * Dot(Grad3, gi1, x1, y1);
                }

                var t2 = 0.5F - x2 * x2 - y2 * y2;
                if (t2 < 0)
                {
                    n2 = 0F;
                }
                else
                {
                    t2 *= t2;
                    n2 = t2 * t2 * Dot(Grad3, gi2, x2, y2);
                }

                // Add contributions from each corner to get the final noise value.
                // The result is scaled to return values in the interval [-1, 1].
                return 70F * (n0 + n1 + n2);

                // ReSharper restore InconsistentNaming
            }
        }

        #endregion

        #region Private Constants

        private const float GAMMA_TO_LINEAR = 2.2F;
        private const float LINEAR_TO_GAMMA = 0.45454545F;
        private const float RANDOM_SEED = 0.8694896071683615F;

        #endregion

        #region Constants

        public const float PI = MathHelper.Pi;
        public const float PI_OVER_TWO = MathHelper.PiOver2;
        public const float HALF_PI = PI_OVER_TWO;
        public const float PI_OVER_FOUR = MathHelper.PiOver4;
        public const float TAU = MathHelper.TwoPi;

        public const float RIGHT = 0;
        public const float UP = -MathHelper.PiOver2;
        public const float LEFT = MathHelper.Pi;
        public const float DOWN = MathHelper.PiOver2;
        public const float UP_RIGHT = -MathHelper.PiOver4;
        public const float UP_LEFT = -MathHelper.PiOver4 - MathHelper.PiOver2;
        public const float DOWN_RIGHT = MathHelper.PiOver4;
        public const float DOWN_LEFT = MathHelper.PiOver4 + MathHelper.PiOver2;
        public const float DEG_TO_RAD = MathHelper.Pi / 180f;
        public const float RAD_TO_DEG = 180f / MathHelper.Pi;
        public const float CIRCLE = MathHelper.TwoPi;
        public const float HALF_CIRCLE = MathHelper.Pi;
        public const float QUARTER_CIRCLE = MathHelper.PiOver2;
        public const float EIGTH_CIRCLE = MathHelper.PiOver4;
        private const string HEX = "0123456789ABCDEF";

        public const float INFINITY = float.MaxValue;
        public const float NEG_INFINITY = float.MinValue;
        public const float EPSILON = float.Epsilon;

        #endregion

        #region Static Methods

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Abs(float f)
        {
            return Math.Abs(f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Abs(int i)
        {
            return Math.Abs(i);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float AbsAngleDiff(float radiansA, float radiansB)
        {
            return Math.Abs(AngleDiff(radiansA, radiansB));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Acos(float f)
        {
            return (float) Math.Acos(f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Angle(Vector2 vec)
        {
            return Atan2(vec.Y, vec.X);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Angle(Vector2 from, Vector2 to)
        {
            return (float) Math.Atan2(to.Y - from.Y, to.X - from.X);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float AngleApproach(float val, float target, float maxMove)
        {
            var diff = AngleDiff(val, target);
            if (Math.Abs(diff) < maxMove) return target;

            return val + Mathf.Clamp(diff, -maxMove, maxMove);
        }
        
        public static float AngleDiff(float radiansA, float radiansB)
        {
            var diff = radiansB - radiansA;
            while (diff > Mathf.PI) diff -= Mathf.TAU;
            while (diff <= -Mathf.PI) diff += Mathf.TAU;
            return diff;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float AngleLerp(float startAngle, float endAngle, float percent)
        {
            return startAngle + AngleDiff(startAngle, endAngle) * percent;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 AngleToVector(float angleRadians, float length = 1)
        {
            return new Vector2(Cos(angleRadians) * length, Sin(angleRadians) * length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Approach(float val, float target, float maxMove)
        {
            return val > target ? Math.Max(val - maxMove, target) : Math.Min(val + maxMove, target);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Approximately(float a, float b, float tolerance = EPSILON) => Mathf.Abs(a - b) < tolerance;

        public static int Digits(this int num)
        {
            var digits = 1;
            var target = 10;

            while (Mathf.Abs(num) >= target)
            {
                digits++;
                target *= 10;
            }

            return digits;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte HexToByte(char c)
        {
            return (byte) HEX.IndexOf(char.ToUpper(c));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Percent(float num, float zeroAt, float oneAt)
        {
            return Mathf.Clamp((num - zeroAt) / oneAt, 0, 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Cos(float a)
        {
            return (float) Math.Cos(a);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Sin(float a)
        {
            return (float) Math.Sin(a);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Atan2(float y, float x)
        {
            return (float) Math.Atan2(y, x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Sqrt(float f)
        {
            return (float) Math.Sqrt(f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SignThreshold(float value, float threshold)
        {
            if (Abs(value) >= threshold) return Sign(value);

            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Min(int a, int b)
        {
            return a < b ? a : b;
        }

        public static int Min(params int[] values)
        {
            var min = values[0];
            for (var i = 1; i < values.Length; i++) min = Mathf.Min(values[i], min);

            return min;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Min(float a, float b)
        {
            return a < b ? a : b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Min(float a, float b, float c)
        {
            return Mathf.Min(Mathf.Min(a, b), c);
        }

        public static float Min(params float[] values)
        {
            var min = values[0];
            for (var i = 1; i < values.Length; i++) min = Mathf.Min(values[i], min);

            return min;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Max(int a, int b)
        {
            return a > b ? a : b;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Max(int a, int b, int c)
        {
            return Mathf.Max(Mathf.Max(a, b), c);
        }

        public static int Max(params int[] values)
        {
            var max = values[0];
            for (var i = 1; i < values.Length; i++) max = Mathf.Max(values[i], max);

            return max;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Max(float a, float b)
        {
            return a > b ? a : b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Max(float a, float b, float c)
        {
            return Mathf.Max(Mathf.Max(a, b), c);
        }

        public static float Max(params float[] values)
        {
            var max = values[0];
            for (var i = 1; i < values.Length; i++) max = Mathf.Max(values[i], max);

            return max;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ToRad(this float f)
        {
            return f * DEG_TO_RAD;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ToDeg(this float f)
        {
            return f * RAD_TO_DEG;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Axis(bool negative, bool positive, int both = 0)
        {
            if (negative)
            {
                if (positive) return both;
                return -1;
            }

            if (positive) return 1;

            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Sign(int i)
        {
            return Math.Sign(i);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Sign(float f)
        {
            return Math.Sign(f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Sign(double d)
        {
            return Math.Sign(d);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Pow(int b, int e)
        {
            return (int) Math.Pow(b, e);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Pow(float b, float e)
        {
            return (float) Math.Pow(b, e);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Pow(double b, double e)
        {
            return Math.Pow(b, e);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Exp(float f)
        {
            return (float) Math.Exp(f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Clamp(int value, int min, int max)
        {
            return Mathf.Min(Mathf.Max(value, min), max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Clamp(float value, float min, float max)
        {
            return Mathf.Min(Mathf.Max(value, min), max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Clamp01(float value)
        {
            return Mathf.Min(Mathf.Max(value, 0), 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float YoYo(float value)
        {
            if (value <= .5f) return value * 2;

            return 1 - (value - .5f) * 2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Map(float val, float min, float max, float newMin = 0, float newMax = 1)
        {
            return (val - min) / (max - min) * (newMax - newMin) + newMin;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SineMap(float counter, float newMin, float newMax)
        {
            return Map((float) Math.Sin(counter), -1, 1, newMin, newMax);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ClampedMap(float val, float min, float max, float newMin = 0, float newMax = 1)
        {
            return Mathf.Clamp((val - min) / (max - min), 0, 1) * (newMax - newMin) + newMin;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Lerp(float a, float b, float t)
        {
            return (1 - t) * a + t * b;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Lerp(double a, double b, float t)
        {
            return (1 - t) * a + t * b;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static decimal Lerp(decimal a, decimal b, decimal t)
        {
            return (1 - t) * a + t * b;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ClosestPowerOfTwo(int value)
        {
            var nextPowerOfTwo = Mathf.NextPowerOfTwo(value);
            
            // if value is between nextPowerOfTwo and pre-pre nextPowerOfTwo
            if (nextPowerOfTwo - value > nextPowerOfTwo >> 2)
            {
                // prev power of two
                return nextPowerOfTwo >> 1;
            }

            return nextPowerOfTwo;
        }
        
        public static int NextPowerOfTwo(int value)
        {
            if (value < 0)
                return 0;

            --value;
            value |= value >> 1;
            value |= value >> 2;
            value |= value >> 4;
            value |= value >> 8;
            value |= value >> 16;
            value += 1;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPowerOfTwo(int value)
        {
            return value > 0 && (value & value - 1) == 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float LerpSnap(float value1, float value2, float amount, float snapThreshold = .1f)
        {
            var ret = Mathf.Lerp(value1, value2, amount);
            if (Math.Abs(ret - value2) < snapThreshold) return value2;

            return ret;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float LerpClamp(float value1, float value2, float lerp)
        {
            return Mathf.Lerp(value1, value2, Mathf.Clamp(lerp, 0, 1));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ReflectAngle(float angle, float axis = 0)
        {
            return -(angle + axis) - axis;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ReflectAngle(float angleRadians, Vector2 axis)
        {
            return ReflectAngle(angleRadians, axis.Angle());
        }

        public static Vector2 ClosestPointOnLine(Vector2 lineA, Vector2 lineB, Vector2 closestTo)
        {
            Vector2 v = lineB - lineA;
            Vector2 w = closestTo - lineA;
            float t = Vector2.Dot(w, v) / Vector2.Dot(v, v);
            t = MathHelper.Clamp(t, 0, 1);

            return lineA + v * t;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Snap(float value, float increment)
        {
            return (float) Math.Round(value / increment) * increment;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Snap(float value, float increment, float offset)
        {
            return (float) Math.Round((value - offset) / increment) * increment + offset;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float WrapAngleDeg(float angleDegrees)
        {
            return ((angleDegrees * Mathf.Sign(angleDegrees) + 180) % 360 - 180) * Mathf.Sign(angleDegrees);
        }

        /// <summary>
        /// Wraps a value between min (inclusive) and max (exclusive)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Wrap(int value, int minValue, int maxValue)
        {
            if (value >= maxValue) return minValue;
            if (value < minValue) return maxValue;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float WrapAngle(float angleRadians)
        {
            return ((angleRadians * Math.Sign(angleRadians) + Mathf.PI) % (Mathf.PI * 2) - Mathf.PI) *
                   Mathf.Sign(angleRadians);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SignAngleDiff(float radiansA, float radiansB)
        {
            return Math.Sign(AngleDiff(radiansA, radiansB));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color ToggleColors(Color current, Color a, Color b)
        {
            if (current == a) return b;

            return a;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ShorterAngleDifference(float currentAngle, float angleA, float angleB)
        {
            if (Math.Abs(AngleDiff(currentAngle, angleA)) < Math.Abs(AngleDiff(currentAngle, angleB))) return angleA;

            return angleB;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ShorterAngleDifference(float currentAngle, float angleA, float angleB, float angleC)
        {
            if (Math.Abs(AngleDiff(currentAngle, angleA)) < Math.Abs(AngleDiff(currentAngle, angleB)))
                return ShorterAngleDifference(currentAngle, angleA, angleC);

            return ShorterAngleDifference(currentAngle, angleB, angleC);
        }

        /// <summary>
        /// Converts the given value from gamma (sRGB) to linear color space.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GammaToLinearSpace(float value)
        {
            return Pow(value, GAMMA_TO_LINEAR);
        }

        /// <summary>
        /// Converts the given value from linear to gamma (sRGB) color space.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float LinearToGammaSpace(float value)
        {
            return Pow(value, LINEAR_TO_GAMMA);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool BetweenInterval(float val, float interval)
        {
            return val % (interval * 2) > interval;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool OnInterval(float val, float prevVal, float interval)
        {
            return (int) (prevVal / interval) != (int) (val / interval);
        }

        /// <summary>
        /// Generate 2D Perlin noise.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float PerlinNoise(float x, float y)
        {
            return new SimplexNoise(RANDOM_SEED).Noise(x, y);
        }

        /// <summary>
        /// A spring approximation
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Wiggle(float t, float freq)
        {
            return (float) Math.Sin(t * freq) * Mathf.Lerp(1, 0, t);
        }

        /// <summary>
        /// A numeric spring calculation
        /// </summary>
        /// <param name="x">value</param>
        /// <param name="v">velocity</param>
        /// <param name="xt">target value</param>
        /// <param name="zeta">damping ratio</param>
        /// <param name="omega">angular frequency</param>
        /// <param name="h">time step</param>
        /// <param name="nX">new value</param>
        /// <param name="nV">new velocity</param>
        public static void Spring(
            float x,
            float v,
            float xt,
            float zeta,
            float omega,
            float h,
            out float nX,
            out float nV
        )
        {
            var f = 1f + 2f * h * zeta * omega;
            var oo = omega * omega;
            var hoo = h * oo;
            var hhoo = h * hoo;
            var detInv = 1.0f / (f + hhoo);
            var detX = f * x + h * v + hhoo * xt;
            var detV = v + hoo * (xt - x);
            nX = detX * detInv;
            nV = detV * detInv;
        }

        #region Rounding
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Floor(float f)
        {
            return (float) Math.Floor(f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Floor(Vector2 v)
        {
            return new Vector2(Mathf.Floor(v.X), Mathf.Floor(v.Y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FloorToInt(float f)
        {
            return (int) Math.Floor(f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Ceil(float f)
        {
            return (float) Math.Ceiling(f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Ceil(Vector2 v)
        {
            return new Vector2(Mathf.Ceil(v.X), Mathf.Ceil(v.Y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CeilToInt(float f)
        {
            return (int) Math.Ceiling(f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Round(float f)
        {
            return (float) Math.Round(f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int RoundToInt(float f)
        {
            return (int) Math.Round(f);
        }

        #endregion

        #endregion
    }
}
