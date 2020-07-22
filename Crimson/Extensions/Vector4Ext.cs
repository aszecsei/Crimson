using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

namespace Crimson
{
    public static class Vector4Ext
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 Floor(in this Vector4 vector)
        {
            return new Vector4(Mathf.Floor(vector.X), Mathf.Floor(vector.Y),
                Mathf.Floor(vector.Z), Mathf.Floor(vector.W));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 Round(in this Vector4 vector)
        {
            return new Vector4(Mathf.Round(vector.X), Mathf.Round(vector.Y),
                Mathf.Round(vector.Z), Mathf.Round(vector.W));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 Ceil(in this Vector4 vector)
        {
            return new Vector4(Mathf.Ceil(vector.X), Mathf.Ceil(vector.Y),
                Mathf.Ceil(vector.Z), Mathf.Ceil(vector.W));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 SafeNormalize(in this Vector4 vector, float length = 1)
        {
            if (vector == Vector4.Zero)
                return Vector4.Zero;
            return Vector4.Normalize(vector) * length;
        }
        
        public static Vector4 ClosestPointOnLine(this Vector4 closestTo, Vector4 lineA, Vector4 lineB)
        {
            Vector4 v = lineB - lineA;
            Vector4 w = closestTo - lineA;
            float t = Vector4.Dot(w, v) / Vector4.Dot(v, v);
            t = MathHelper.Clamp(t, 0, 1);

            return lineA + v * t;
        }
    }
}