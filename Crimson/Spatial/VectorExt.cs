using Microsoft.Xna.Framework;

namespace Crimson.Spatial
{
    public static class VectorExt
    {
        public static Vector2 Floor(in this Vector2 vector)
        {
            return new Vector2(Mathf.Floor(vector.X), Mathf.Floor(vector.Y));
        }

        public static Vector3 Floor(in this Vector3 vector)
        {
            return new Vector3(Mathf.Floor(vector.X), Mathf.Floor(vector.Y), Mathf.Floor(vector.Z));
        }

        public static Vector4 Floor(in this Vector4 vector)
        {
            return new Vector4(Mathf.Floor(vector.X), Mathf.Floor(vector.Y),
                Mathf.Floor(vector.Z), Mathf.Floor(vector.W));
        }

        public static Vector2 Round(in this Vector2 vector)
        {
            return new Vector2(Mathf.Round(vector.X), Mathf.Round(vector.Y));
        }

        public static Vector3 Round(in this Vector3 vector)
        {
            return new Vector3(Mathf.Round(vector.X), Mathf.Round(vector.Y), Mathf.Round(vector.Z));
        }

        public static Vector4 Round(in this Vector4 vector)
        {
            return new Vector4(Mathf.Round(vector.X), Mathf.Round(vector.Y),
                Mathf.Round(vector.Z), Mathf.Round(vector.W));
        }

        public static Vector2 Ceil(in this Vector2 vector)
        {
            return new Vector2(Mathf.Ceil(vector.X), Mathf.Ceil(vector.Y));
        }

        public static Vector3 Ceil(in this Vector3 vector)
        {
            return new Vector3(Mathf.Ceil(vector.X), Mathf.Ceil(vector.Y), Mathf.Ceil(vector.Z));
        }

        public static Vector4 Ceil(in this Vector4 vector)
        {
            return new Vector4(Mathf.Ceil(vector.X), Mathf.Ceil(vector.Y),
                Mathf.Ceil(vector.Z), Mathf.Ceil(vector.W));
        }

        /// <summary>
        /// Turns a vector to its right perpendicular
        /// </summary>
        public static Vector2 TurnRight(in this Vector2 vector)
        {
            return new Vector2(-vector.Y, vector.X);
        }

        /// <summary>
        /// Turns a vector to its left perpendicular
        /// </summary>
        public static Vector2 TurnLeft(in this Vector2 vector)
        {
            return new Vector2(vector.Y, -vector.X);
        }

        public static float Angle(in this Vector2 vector)
        {
            return Mathf.Atan2(vector.Y, vector.X);
        }
        
        public static Vector2 Clamp(in this Vector2 value, Vector2 min, Vector2 max)
        {
            var newX = Mathf.Clamp(value.X, min.X, max.X);
            var newY = Mathf.Clamp(value.Y, min.Y, max.Y);
            return new Vector2(newX, newY);
        }

        public static Vector3 Clamp(in this Vector3 value, Vector3 min, Vector3 max)
        {
            var newX = Mathf.Clamp(value.X, min.X, max.X);
            var newY = Mathf.Clamp(value.Y, min.Y, max.Y);
            var newZ = Mathf.Clamp(value.Z, min.Z, max.Z);
            return new Vector3(newX, newY, newZ);
        }

        public static Vector2 SafeNormalize(in this Vector2 vector, float length = 1)
        {
            if (vector == Vector2.Zero)
                return Vector2.Zero;
            return Vector2.Normalize(vector) * length;
        }

        public static Vector3 SafeNormalize(in this Vector3 vector, float length = 1)
        {
            if (vector == Vector3.Zero)
                return Vector3.Zero;
            return Vector3.Normalize(vector) * length;
        }

        public static Vector4 SafeNormalize(in this Vector4 vector, float length = 1)
        {
            if (vector == Vector4.Zero)
                return Vector4.Zero;
            return Vector4.Normalize(vector) * length;
        }
        
        public static Vector2 Toward(Vector2 from, Vector2 to, float length)
        {
            if (from == to) return Vector2.Zero;

            return (to - from).SafeNormalize(length);
        }
        
        public static Vector2 LerpSnap(Vector2 value1, Vector2 value2, float amount, float snapThresholdSq = .1f)
        {
            var ret = Vector2.Lerp(value1, value2, amount);
            if ((ret - value2).LengthSquared() < snapThresholdSq) return value2;

            return ret;
        }

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
        
        public static Vector3 ClosestPointOnLine(this Vector3 closestTo, Vector3 lineA, Vector3 lineB)
        {
            Vector3 v = lineB - lineA;
            Vector3 w = closestTo - lineA;
            float t = Vector3.Dot(w, v) / Vector3.Dot(v, v);
            t = MathHelper.Clamp(t, 0, 1);

            return lineA + v * t;
        }
        
        public static Vector4 ClosestPointOnLine(this Vector4 closestTo, Vector4 lineA, Vector4 lineB)
        {
            Vector4 v = lineB - lineA;
            Vector4 w = closestTo - lineA;
            float t = Vector4.Dot(w, v) / Vector4.Dot(v, v);
            t = MathHelper.Clamp(t, 0, 1);

            return lineA + v * t;
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
    }
}