using Microsoft.Xna.Framework;

namespace Crimson.Spatial
{
    public static class VectorExt
    {
        public static Vector2 Floor(this Vector2 vector)
        {
            return new Vector2(Mathf.Floor(vector.X), Mathf.Floor(vector.Y));
        }

        public static Vector3 Floor(this Vector3 vector)
        {
            return new Vector3(Mathf.Floor(vector.X), Mathf.Floor(vector.Y), Mathf.Floor(vector.Z));
        }

        public static Vector4 Floor(this Vector4 vector)
        {
            return new Vector4(Mathf.Floor(vector.X), Mathf.Floor(vector.Y),
                Mathf.Floor(vector.Z), Mathf.Floor(vector.W));
        }

        public static Vector2 Round(this Vector2 vector)
        {
            return new Vector2(Mathf.Round(vector.X), Mathf.Round(vector.Y));
        }

        public static Vector3 Round(this Vector3 vector)
        {
            return new Vector3(Mathf.Round(vector.X), Mathf.Round(vector.Y), Mathf.Round(vector.Z));
        }

        public static Vector4 Round(this Vector4 vector)
        {
            return new Vector4(Mathf.Round(vector.X), Mathf.Round(vector.Y),
                Mathf.Round(vector.Z), Mathf.Round(vector.W));
        }

        public static Vector2 Ceil(this Vector2 vector)
        {
            return new Vector2(Mathf.Ceil(vector.X), Mathf.Ceil(vector.Y));
        }

        public static Vector3 Ceil(this Vector3 vector)
        {
            return new Vector3(Mathf.Ceil(vector.X), Mathf.Ceil(vector.Y), Mathf.Ceil(vector.Z));
        }

        public static Vector4 Ceil(this Vector4 vector)
        {
            return new Vector4(Mathf.Ceil(vector.X), Mathf.Ceil(vector.Y),
                Mathf.Ceil(vector.Z), Mathf.Ceil(vector.W));
        }

        /// <summary>
        ///     Turns a vector to its right perpendicular
        /// </summary>
        public static Vector2 TurnRight(this Vector2 vector)
        {
            return new Vector2(-vector.Y, vector.X);
        }

        /// <summary>
        ///     Turns a vector to its left perpendicular
        /// </summary>
        public static Vector2 TurnLeft(this Vector2 vector)
        {
            return new Vector2(vector.Y, -vector.X);
        }

        public static float Angle(this Vector2 vector)
        {
            return Mathf.Atan2(vector.Y, vector.X);
        }
        
        public static Vector2 Clamp(this Vector2 value, Vector2 min, Vector2 max)
        {
            var newX = Mathf.Clamp(value.X, min.X, max.X);
            var newY = Mathf.Clamp(value.Y, min.Y, max.Y);
            return new Vector2(newX, newY);
        }

        public static Vector3 Clamp(this Vector3 value, Vector3 min, Vector3 max)
        {
            var newX = Mathf.Clamp(value.X, min.X, max.X);
            var newY = Mathf.Clamp(value.Y, min.Y, max.Y);
            var newZ = Mathf.Clamp(value.Z, min.Z, max.Z);
            return new Vector3(newX, newY, newZ);
        }

        public static Vector2 Normalized(this Vector2 vector)
        {
            if (vector.X == 0 && vector.Y == 0)
                return Vector2.Zero;
            return Vector2.Normalize(vector);
        }

        public static Vector3 Normalized(this Vector3 vector)
        {
            if (vector.X == 0 && vector.Y == 0)
                return Vector3.Zero;
            return Vector3.Normalize(vector);
        }

        public static Vector4 Normalized(this Vector4 vector)
        {
            if (vector.X == 0 && vector.Y == 0)
                return Vector4.Zero;
            return Vector4.Normalize(vector);
        }

        /// <summary>
        ///     Normalizes a Vector2 and snaps it to the closest of the 4 cardinal directions (a zero-length Vector2 returns 0)
        /// </summary>
        public static Vector2 FourWayNormal(this Vector2 vector)
        {
            if (vector == Vector2.Zero)
                return vector;

            vector = Mathf.AngleToVector(Mathf.Snap(vector.Angle(), Mathf.HALF_PI));

            if (Mathf.Abs(vector.X) < .1f)
            {
                vector.X = 0;
                vector.Y = Mathf.Sign(vector.Y);
            }
            else if (Mathf.Abs(vector.Y) < .1f)
            {
                vector.X = Mathf.Sign(vector.X);
                vector.Y = 0;
            }

            return vector;
        }

        /// <summary>
        ///     Normalizes a Vector2 and snaps it to the closest of the 8 cardinal or diagonal directions (a zero-length Vector2
        ///     returns 0)
        /// </summary>
        public static Vector2 EightWayNormal(this Vector2 vector)
        {
            if (vector == Vector2.Zero)
                return vector;

            vector = Mathf.AngleToVector(Mathf.Snap(vector.Angle(), Mathf.PI / 4));

            if (Mathf.Abs(vector.X) < .1f)
            {
                vector.X = 0;
                vector.Y = Mathf.Sign(vector.Y);
            }
            else if (Mathf.Abs(vector.Y) < .1f)
            {
                vector.X = Mathf.Sign(vector.X);
                vector.Y = 0;
            }

            return vector;
        }
    }
}