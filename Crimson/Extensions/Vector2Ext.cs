using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

namespace Crimson
{
    public static class Vector2Ext
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Floor(in this Vector2 vector)
        {
            return new Vector2(Mathf.Floor(vector.X), Mathf.Floor(vector.Y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Round(in this Vector2 vector)
        {
            return new Vector2(Mathf.Round(vector.X), Mathf.Round(vector.Y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Ceil(in this Vector2 vector)
        {
            return new Vector2(Mathf.Ceil(vector.X), Mathf.Ceil(vector.Y));
        }

        /// <summary>
        /// Turns a vector to its right perpendicular
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 TurnRight(in this Vector2 vector)
        {
            return new Vector2(-vector.Y, vector.X);
        }

        /// <summary>
        /// Turns a vector to its left perpendicular
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 TurnLeft(in this Vector2 vector)
        {
            return new Vector2(vector.Y, -vector.X);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Angle(in this Vector2 vector)
        {
            return Mathf.Atan2(vector.Y, vector.X);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Clamp(in this Vector2 value, Vector2 min, Vector2 max)
        {
            var newX = Mathf.Clamp(value.X, min.X, max.X);
            var newY = Mathf.Clamp(value.Y, min.Y, max.Y);
            return new Vector2(newX, newY);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 SafeNormalize(in this Vector2 vector, float length = 1)
        {
            if (vector == Vector2.Zero)
                return Vector2.Zero;
            return Vector2.Normalize(vector) * length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 SafeNormalize(in this Vector2 vector, Vector2 ifZero)
        {
            if (vector == Vector2.Zero)
                return ifZero;
            return Vector2.Normalize(vector);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Toward(Vector2 from, Vector2 to, float length)
        {
            if (from == to) return Vector2.Zero;

            return (to - from).SafeNormalize(length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 LerpSnap(Vector2 value1, Vector2 value2, float amount, float snapThresholdSq = .1f)
        {
            var ret = Vector2.Lerp(value1, value2, amount);
            if ((ret - value2).LengthSquared() < snapThresholdSq) return value2;

            return ret;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Sign(this Vector2 vec)
        {
            return new Vector2(Mathf.Sign(vec.X), Mathf.Sign(vec.Y));
        }

        public static Vector2 ClosestPointOnLine(this Vector2 closestTo, Vector2 lineA, Vector2 lineB)
        {
            Vector2 v = lineB - lineA;
            Vector2 w = closestTo - lineA;
            float t = Vector2.Dot(w, v) / Vector2.Dot(v, v);
            t = MathHelper.Clamp(t, 0, 1);

            return lineA + v * t;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Perpendicular(this Vector2 vector)
        {
            return new Vector2(-vector.Y, vector.X);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Abs(this Vector2 val)
        {
            return new Vector2(Mathf.Abs(val.X), Mathf.Abs(val.Y));
        }

        public static Vector2 Approach(this Vector2 v, Vector2 target, float maxMove)
        {
            if (maxMove == 0 || v == target) return v;

            if (maxMove > (target - v).Length()) return target;

            return v + (target - v).SafeNormalize() * maxMove;
        }

        public static Vector2 SnappedNormal(this Vector2 vec, float slices)
        {
            var divider = MathHelper.TwoPi / slices;

            var angle = vec.Angle();
            angle = Mathf.Floor((angle + divider / 2f) / divider) * divider;
            return Mathf.AngleToVector(angle);
        }

        public static Vector2 Snapped(this Vector2 vec, float slices)
        {
            var divider = MathHelper.TwoPi / slices;

            var angle = vec.Angle();
            angle = Mathf.Floor((angle + divider / 2f) / divider) * divider;
            return Mathf.AngleToVector(angle, vec.Length());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 XComp(this Vector2 vec)
        {
            return Vector2.UnitX * vec.X;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 YComp(this Vector2 vec)
        {
            return Vector2.UnitY * vec.Y;
        }

        /// <summary>
        /// Normalizes a Vector2 and snaps it to the closest of the 4 cardinal directions (a zero-length Vector2 returns 0)
        /// </summary>
        public static Vector2 FourWayNormal(in this Vector2 vector)
        {
            if (vector == Vector2.Zero)
                return vector;

            Vector2 v = Mathf.AngleToVector(Mathf.Snap(vector.Angle(), Mathf.HALF_PI));

            if (Mathf.Abs(v.X) < .1f)
            {
                v.X = 0;
                v.Y = Mathf.Sign(v.Y);
            }
            else if (Mathf.Abs(v.Y) < .1f)
            {
                v.X = Mathf.Sign(v.X);
                v.Y = 0;
            }

            return vector;
        }

        /// <summary>
        /// Normalizes a Vector2 and snaps it to the closest of the 8 cardinal or diagonal directions (a zero-length Vector2
        /// returns 0)
        /// </summary>
        public static Vector2 EightWayNormal(in this Vector2 vector)
        {
            if (vector == Vector2.Zero)
                return vector;

            Vector2 v = Mathf.AngleToVector(Mathf.Snap(vector.Angle(), Mathf.PI / 4));

            if (Mathf.Abs(v.X) < .1f)
            {
                v.X = 0;
                v.Y = Mathf.Sign(v.Y);
            }
            else if (Mathf.Abs(v.Y) < .1f)
            {
                v.X = Mathf.Sign(v.X);
                v.Y = 0;
            }

            return v;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Rotate(this Vector2 vec, float angleRadians)
        {
            return Mathf.AngleToVector(vec.Angle() + angleRadians, vec.Length());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 RotateTowards(this Vector2 vec, float targetAngleRadians, float maxMoveRadians)
        {
            var angle = Mathf.AngleApproach(vec.Angle(), targetAngleRadians, maxMoveRadians);
            return Mathf.AngleToVector(angle, vec.Length());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ManhattanDistance(this Vector2 vec, Vector2 other)
        {
            return Mathf.Abs(vec.X - other.X) + Mathf.Abs(vec.Y - other.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ChebyshevDistance(this Vector2 vec, Vector2 other)
        {
            return Mathf.Max(Mathf.Abs(vec.X - other.X), Mathf.Abs(vec.Y - other.Y));
        }

        public static Vector2 ClosestTo(this IList<Vector2> collection, Vector2 point)
        {
            if ( collection.Count == 0 ) return Vector2.Zero;
            Vector2 closest          = collection[0];
            float   closestSqrLength = (collection[0] - point).LengthSquared();

            for ( int i = 0; i < collection.Count; ++i )
            {
                float sqrDist = (collection[i] - point).LengthSquared();
                if ( sqrDist < closestSqrLength )
                {
                    closest          = collection[i];
                    closestSqrLength = sqrDist;
                }
            }

            return closest;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Numerics.Vector2 ToNumerics(this Vector2 vec)
        {
            return new System.Numerics.Vector2(vec.X, vec.Y);
        }

        public static Vector2 ToMonogame(this System.Numerics.Vector2 vec)
        {
            return new Vector2(vec.X, vec.Y);
        }
    }
}
