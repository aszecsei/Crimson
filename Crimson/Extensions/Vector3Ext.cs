using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

namespace Crimson
{
    public static class Vector3Ext
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Floor(in this Vector3 vector)
        {
            return new Vector3(Mathf.Floor(vector.X), Mathf.Floor(vector.Y), Mathf.Floor(vector.Z));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Round(in this Vector3 vector)
        {
            return new Vector3(Mathf.Round(vector.X), Mathf.Round(vector.Y), Mathf.Round(vector.Z));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Ceil(in this Vector3 vector)
        {
            return new Vector3(Mathf.Ceil(vector.X), Mathf.Ceil(vector.Y), Mathf.Ceil(vector.Z));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Clamp(in this Vector3 value, Vector3 min, Vector3 max)
        {
            var newX = Mathf.Clamp(value.X, min.X, max.X);
            var newY = Mathf.Clamp(value.Y, min.Y, max.Y);
            var newZ = Mathf.Clamp(value.Z, min.Z, max.Z);
            return new Vector3(newX, newY, newZ);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 SafeNormalize(in this Vector3 vector, float length = 1)
        {
            if (vector == Vector3.Zero)
                return Vector3.Zero;
            return Vector3.Normalize(vector) * length;
        }
        
        public static Vector3 ClosestPointOnLine(this Vector3 closestTo, Vector3 lineA, Vector3 lineB)
        {
            Vector3 v = lineB - lineA;
            Vector3 w = closestTo - lineA;
            float t = Vector3.Dot(w, v) / Vector3.Dot(v, v);
            t = MathHelper.Clamp(t, 0, 1);

            return lineA + v * t;
        }
        
        public static Vector3 Approach(this Vector3 v, Vector3 target, float maxMove)
        {
            if (maxMove == 0 || v == target) return v;
            
            if (maxMove > (target - v).Length()) return target;

            return v + (target - v).SafeNormalize() * maxMove;
        }
        
        public static Vector3 RotateTowards(this Vector3 from, Vector3 target, float maxRotationRadians)
        {
            var c = Vector3.Cross(from, target);
            var alen = from.Length();
            var blen = target.Length();
            var w = Mathf.Sqrt(alen * alen * (blen * blen)) + Vector3.Dot(from, target);
            var q = new Quaternion(c.X, c.Y, c.Z, w);

            if (q.Length() <= maxRotationRadians) return target;

            q.Normalize();
            q *= maxRotationRadians;

            return Vector3.Transform(from, q);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 XZ(this Vector3 vector)
        {
            return new Vector2(vector.X, vector.Z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 ToVector2(in this Vector3 vec)
        {
            return new Vector2(vec.X, vec.Y);
        }
    }
}