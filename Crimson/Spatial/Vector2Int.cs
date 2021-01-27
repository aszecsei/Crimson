using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

namespace Crimson
{
    [StructLayout(LayoutKind.Sequential)]
    [DataContract]
    public struct Vector2Int : IEquatable<Vector2Int>
    {
        private static readonly Vector2Int down = new Vector2Int(0, -1);
        private static readonly Vector2Int left = new Vector2Int(-1, 0);
        private static readonly Vector2Int one = new Vector2Int(1, 1);
        private static readonly Vector2Int right = new Vector2Int(1, 0);
        private static readonly Vector2Int up = new Vector2Int(0, 1);
        private static readonly Vector2Int zero = new Vector2Int(0, 0);
        private static readonly Vector2Int unitX = new Vector2Int(1, 0);
        private static readonly Vector2Int unitY = new Vector2Int(0, 1);

        public static Vector2Int Down => down;
        public static Vector2Int Left => left;
        public static Vector2Int One => one;
        public static Vector2Int Right => right;
        public static Vector2Int Up => up;
        public static Vector2Int Zero => zero;
        public static Vector2Int UnitX => unitX;
        public static Vector2Int UnitY => unitY;

        [DataMember]
        public int X;
        [DataMember]
        public int Y;

        public Vector2Int(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Vector2Int(Vector2Int other)
            : this(other.X, other.Y)
        {
        }

        public int this[int index]
        {
            get
            {
                return index switch
                {
                    0 => X,
                    1 => Y,
                    _ => throw new ArgumentOutOfRangeException(nameof(index), "index out of bounds")
                };
            }
            set
            {
                switch ( index )
                {
                case 0:
                    X = value;
                    break;
                case 1:
                    Y = value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(index), "index out of bounds");
                }
            }
        }

        public float Magnitude => Mathf.Sqrt(X * X + Y * Y);
        public float SqrMagnitude => X * X + Y * Y;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clamp(Vector2Int min, Vector2Int max)
        {
            X = Mathf.Clamp(X, min.X, max.X);
            Y = Mathf.Clamp(Y, min.Y, max.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set(int x, int y)
        {
            X = x;
            Y = y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString()
        {
            return $"<{X}, {Y}>";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Scale(Vector2Int scale)
        {
            X *= scale.X;
            Y *= scale.Y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int CeilToInt(Vector2 v)
        {
            return new Vector2Int
            {
                X = Mathf.CeilToInt(v.X),
                Y = Mathf.CeilToInt(v.Y)
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Distance(Vector2Int a, Vector2Int b)
        {
            return (a - b).Magnitude;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ManhattanDistance(Vector2Int a, Vector2Int b)
        {
            return Mathf.Abs(a.X - b.X) + Mathf.Abs(a.Y - b.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ManhattanDistance(Vector2Int b)
        {
            return Mathf.Abs(X - b.X) + Mathf.Abs(Y - b.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ChebyshevDistance(Vector2Int a, Vector2Int b)
        {
            return Mathf.Max(Mathf.Abs(a.X - b.X), Mathf.Abs(a.Y - b.Y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ChebyshevDistance(Vector2Int b)
        {
            return Mathf.Max(Mathf.Abs(X - b.X), Mathf.Abs(Y - b.Y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int FloorToInt(Vector2 v)
        {
            return new Vector2Int
            {
                X = Mathf.FloorToInt(v.X),
                Y = Mathf.FloorToInt(v.Y)
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int Max(Vector2Int a, Vector2Int b)
        {
            return new Vector2Int
            {
                X = Mathf.Max(a.X, b.X),
                Y = Mathf.Max(a.Y, b.Y)
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int Min(Vector2Int a, Vector2Int b)
        {
            return new Vector2Int
            {
                X = Mathf.Min(a.X, b.X),
                Y = Mathf.Min(a.Y, b.Y)
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int RoundToInt(Vector2 v)
        {
            return new Vector2Int
            {
                X = Mathf.RoundToInt(v.X),
                Y = Mathf.RoundToInt(v.Y)
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int Scale(Vector2Int a, Vector2Int b)
        {
            return new Vector2Int
            {
                X = a.X * b.X,
                Y = a.Y * b.Y
            };
        }

        /// <summary>
        /// Deconstruction method for <see cref="Vector3Int" />.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Deconstruct(out int x, out int y)
        {
            x = this.X;
            y = this.Y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int operator -(Vector2Int lhs, Vector2Int rhs)
        {
            return new Vector2Int
            {
                X = lhs.X - rhs.X,
                Y = lhs.Y - rhs.Y
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Vector2Int lhs, Vector2Int rhs)
        {
            return lhs.X != rhs.X ||
                   lhs.Y != rhs.Y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Vector2Int lhs, Vector2Int rhs)
        {
            return lhs.X == rhs.X &&
                   lhs.Y == rhs.Y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int operator *(Vector2Int lhs, Vector2Int rhs)
        {
            return new Vector2Int
            {
                X = lhs.X * rhs.X,
                Y = lhs.Y * rhs.Y
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int operator *(Vector2Int lhs, int rhs)
        {
            return new Vector2Int
            {
                X = lhs.X * rhs,
                Y = lhs.Y * rhs
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int operator /(Vector2Int lhs, int rhs)
        {
            return new Vector2Int
            {
                X = lhs.X / rhs,
                Y = lhs.Y / rhs
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int operator +(Vector2Int lhs, Vector2Int rhs)
        {
            return new Vector2Int
            {
                X = lhs.X + rhs.X,
                Y = lhs.Y + rhs.Y
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int operator -(Vector2Int v)
        {
            return new Vector2Int
            {
                X = -v.X,
                Y = -v.Y
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator Vector2(Vector2Int v)
        {
            return new Vector2
            {
                X = v.X,
                Y = v.Y
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator Point(Vector2Int v)
        {
            return new Point
            {
                X = v.X,
                Y = v.Y
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator Vector2Int(Point p)
        {
            return new Vector2Int
            {
                X = p.X,
                Y = p.Y
            };
        }

        public bool Equals(Vector2Int other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object? obj)
        {
            return obj is Vector2Int other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = X.GetHashCode();
                hashCode = (hashCode * 397) ^ Y.GetHashCode();
                return hashCode;
            }
        }
    }
}
