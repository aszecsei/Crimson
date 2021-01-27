using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

namespace Crimson
{
    [StructLayout(LayoutKind.Sequential)]
    [DataContract]
    public struct Vector3Int : IEquatable<Vector3Int>
    {
        private static readonly Vector3Int down = new Vector3Int(0, -1, 0);
        private static readonly Vector3Int left = new Vector3Int(-1, 0, 0);
        private static readonly Vector3Int one = new Vector3Int(1, 1, 1);
        private static readonly Vector3Int right = new Vector3Int(1, 0, 0);
        private static readonly Vector3Int up = new Vector3Int(0, 1, 0);
        private static readonly Vector3Int zero = new Vector3Int(0, 0, 0);
        private static readonly Vector3Int forward = new Vector3Int(0, 0, -1);
        private static readonly Vector3Int backward = new Vector3Int(0, 0, 1);
        private static readonly Vector3Int unitX = new Vector3Int(1, 0, 0);
        private static readonly Vector3Int unitY = new Vector3Int(0, 1, 0);
        private static readonly Vector3Int unitZ = new Vector3Int(0, 0, 1);

        public static Vector3Int Down => down;
        public static Vector3Int Left => left;
        public static Vector3Int One => one;
        public static Vector3Int Right => right;
        public static Vector3Int Up => up;
        public static Vector3Int Zero => zero;
        public static Vector3Int Forward => forward;
        public static Vector3Int Backward => backward;
        public static Vector3Int UnitX => unitX;
        public static Vector3Int UnitY => unitY;
        public static Vector3Int UnitZ => unitZ;

        [DataMember]
        public int X;
        [DataMember]
        public int Y;
        [DataMember]
        public int Z;

        public Vector3Int(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public int this[int index]
        {
            get
            {
                return index switch
                {
                    0 => X,
                    1 => Y,
                    2 => Z,
                    _ => throw new ArgumentOutOfRangeException(nameof(index), "index out of bounds")
                };
            }
            set
            {
                switch (index)
                {
                    case 0:
                        X = value;
                        break;
                    case 1:
                        Y = value;
                        break;
                    case 2:
                        Z = value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(index), "index out of bounds");
                }
            }
        }

        public float Magnitude => Mathf.Sqrt(X * X + Y * Y + Z * Z);
        public float SqrMagnitude => X * X + Y * Y + Z * Z;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clamp(Vector3Int min, Vector3Int max)
        {
            X = Mathf.Clamp(X, min.X, max.X);
            Y = Mathf.Clamp(Y, min.Y, max.Y);
            Z = Mathf.Clamp(Z, min.Z, max.Z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString()
        {
            return $"<{X}, {Y}, {Z}>";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Scale(Vector3Int scale)
        {
            X *= scale.X;
            Y *= scale.Y;
            Z *= scale.Z;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int CeilToInt(Vector3 v)
        {
            return new Vector3Int
            {
                X = Mathf.CeilToInt(v.X),
                Y = Mathf.CeilToInt(v.Y),
                Z = Mathf.CeilToInt(v.Z)
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Distance(Vector3Int a, Vector3Int b)
        {
            return (a - b).Magnitude;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ManhattanDistance(Vector3Int a, Vector3Int b)
        {
            return Mathf.Abs(a.X - b.X) + Mathf.Abs(a.Y - b.Y) + Mathf.Abs(a.Z - b.Z);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ManhattanDistance(Vector3Int b)
        {
            return Mathf.Abs(X - b.X) + Mathf.Abs(Y - b.Y) + Mathf.Abs(Z - b.Z);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ChebyshevDistance(Vector3Int a, Vector3Int b)
        {
            return Mathf.Max(Mathf.Abs(a.X - b.X), Mathf.Abs(a.Y - b.Y), Mathf.Abs(a.Z - b.Z));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ChebyshevDistance(Vector3Int b)
        {
            return Mathf.Max(Mathf.Abs(X - b.X), Mathf.Abs(Y - b.Y), Mathf.Abs(Z - b.Z));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int FloorToInt(Vector3 v)
        {
            return new Vector3Int
            {
                X = Mathf.FloorToInt(v.X),
                Y = Mathf.FloorToInt(v.Y),
                Z = Mathf.FloorToInt(v.Z)
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int Max(Vector3Int a, Vector3Int b)
        {
            return new Vector3Int
            {
                X = Mathf.Max(a.X, b.X),
                Y = Mathf.Max(a.Y, b.Y),
                Z = Mathf.Max(a.Z, b.Z)
            };
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int Min(Vector3Int a, Vector3Int b)
        {
            return new Vector3Int
            {
                X = Mathf.Min(a.X, b.X),
                Y = Mathf.Min(a.Y, b.Y),
                Z = Mathf.Min(a.Z, b.Z)
            };
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int RoundToInt(Vector3 v)
        {
            return new Vector3Int
            {
                X = Mathf.RoundToInt(v.X),
                Y = Mathf.RoundToInt(v.Y),
                Z = Mathf.RoundToInt(v.Z)
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int Scale(Vector3Int a, Vector3Int b)
        {
            return new Vector3Int
            {
                X = a.X * b.X,
                Y = a.Y * b.Y,
                Z = a.Z * b.Z
            };
        }
        
        /// <summary>
        /// Deconstruction method for <see cref="Vector3Int" />.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public void Deconstruct(out int x, out int y, out int z)
        {
            x = this.X;
            y = this.Y;
            z = this.Z;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int operator -(Vector3Int lhs, Vector3Int rhs)
        {
            return new Vector3Int
            {
                X = lhs.X - rhs.X,
                Y = lhs.Y - rhs.Y,
                Z = lhs.Z - rhs.Z
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Vector3Int lhs, Vector3Int rhs)
        {
            return lhs.X != rhs.X ||
                   lhs.Y != rhs.Y ||
                   lhs.Z != rhs.Z;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Vector3Int lhs, Vector3Int rhs)
        {
            return lhs.X == rhs.X &&
                   lhs.Y == rhs.Y &&
                   lhs.Z == rhs.Z;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int operator *(Vector3Int lhs, Vector3Int rhs)
        {
            return new Vector3Int
            {
                X = lhs.X * rhs.X,
                Y = lhs.Y * rhs.Y,
                Z = lhs.Z * rhs.Z
            };
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int operator *(Vector3Int lhs, int rhs)
        {
            return new Vector3Int
            {
                X = lhs.X * rhs,
                Y = lhs.Y * rhs,
                Z = lhs.Z * rhs
            };
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int operator /(Vector3Int lhs, int rhs)
        {
            return new Vector3Int
            {
                X = lhs.X / rhs,
                Y = lhs.Y / rhs,
                Z = lhs.Z / rhs
            };
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int operator +(Vector3Int lhs, Vector3Int rhs)
        {
            return new Vector3Int
            {
                X = lhs.X + rhs.X,
                Y = lhs.Y + rhs.Y,
                Z = lhs.Z + rhs.Z
            };
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int operator -(Vector3Int v)
        {
            return new Vector3Int
            {
                X = -v.X,
                Y = -v.Y,
                Z = -v.Z
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator Vector3(Vector3Int v)
        {
            return new Vector3
            {
                X = v.X,
                Y = v.Y,
                Z = v.Z
            };
        }

        public bool Equals(Vector3Int other)
        {
            return X == other.X && Y == other.Y && Z == other.Z;
        }

        public override bool Equals(object? obj)
        {
            return obj is Vector3Int other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = X.GetHashCode();
                hashCode = (hashCode * 397) ^ Y.GetHashCode();
                hashCode = (hashCode * 397) ^ Z.GetHashCode();
                return hashCode;
            }
        }
    }
}
