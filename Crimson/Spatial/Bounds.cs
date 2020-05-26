using Microsoft.Xna.Framework;

namespace Crimson.Spatial
{
    public struct Bounds
    {
        /// <summary>
        /// The center of the bounding box.
        /// </summary>
        public Vector3 Center;
        /// <summary>
        /// The total size of the box. This is always twice as large
        /// as the extents.
        /// </summary>
        public Vector3 Size;
        /// <summary>
        /// The extents of the bounding box. This is always half the
        /// size of the bounds.
        /// </summary>
        public Vector3 Extents
        {
            get => new Vector3(Size.X / 2, Size.Y / 2, Size.Z / 2);
            set => Size = new Vector3(value.X * 2, value.Y * 2, value.Z * 2);
        }
        
        /// <summary>
        /// The maximal point of the box. This is always equal to center+extents.
        /// Setting this value keeps the old bounds' min in place. If the new max
        /// is less than the min, the values are swapped.
        /// </summary>
        public Vector3 Max
        {
            get => Center + Extents;
            set
            {
                var (x, y, z) = Min;
                Center = new Vector3((value.X + x) / 2, (value.Y + y) / 2, (value.Z + z) / 2);
                Size = new Vector3(Mathf.Abs(value.X - x), Mathf.Abs(value.Y - y), Mathf.Abs(value.Z - z));
            }
        }

        /// <summary>
        /// The minimal point of the box. This is always equal to center-extents.
        /// Setting this value keeps the old bounds' max in place. If the new min
        /// is greater than the max, the values are swapped.
        /// </summary>
        public Vector3 Min
        {
            get => Center - Extents;
            set
            {
                var (x, y, z) = Max;
                Center = new Vector3((value.X + x) / 2, (value.Y + y) / 2, (value.Z + z) / 2);
                Size = new Vector3(Mathf.Abs(x - value.X), Mathf.Abs(y - value.Y), Mathf.Abs(z - value.Z));
            }
        }
        
        /// <summary>
        /// Creates a new bounds.
        /// </summary>
        public Bounds(Vector3 center, Vector3 size)
        {
            Center = center;
            Size = size;
        }
        
        /// <summary>
        /// The closest point on the bounding box.
        /// </summary>
        /// <remarks>
        /// If the point is inside the bounding box, unmodified point position
        /// will be returned.
        /// </remarks>
        /// <param name="point">Arbitrary point.</param>
        /// <returns>The point on or inside the bounding box.</returns>
        public Vector3 ClosestPoint(Vector3 point)
        {
            var dx = Mathf.Max(Min.X - point.X, 0, point.X - Max.X);
            var dy = Mathf.Max(Min.Y - point.Y, 0, point.Y - Max.Y);
            var dz = Mathf.Max(Min.Z - point.Z, 0, point.Z - Max.Z);
            return new Vector3(point.X + dx, point.Y + dy, point.Z + dz);
        }
        
        /// <summary>
        /// Is <c>point</c> contained in the bounding box?
        /// </summary>
        public bool Contains(Vector3 point)
        {
            return point.X >= Min.X && point.Y >= Min.Y && point.Z >= Min.Z 
                   && point.X <= Max.X && point.Y <= Max.Y && point.Z <= Max.Z;
        }
        
        /// <summary>
        /// Grows the bounds to include the point.
        /// </summary>
        public void Encapsulate(Vector3 point)
        {
            var newMin = new Vector3(Mathf.Min(Min.X, point.X),
                Mathf.Min(Min.Y, point.Y),
                Mathf.Min(Min.Z, point.Z));
            var newMax = new Vector3(Mathf.Max(Max.X, point.X),
                Mathf.Max(Max.Y, point.Y),
                Mathf.Max(Max.Z, point.Z));
            SetMinMax(newMin, newMax);
        }
        
        /// <summary>
        /// Grows the bounds to include the bounds.
        /// </summary>
        public void Encapsulate(Bounds bounds)
        {
            var newMin = new Vector3(Mathf.Min(Min.X, bounds.Min.X),
                Mathf.Min(Min.Y, bounds.Min.Y),
                Mathf.Min(Min.Z, bounds.Min.Z));
            var newMax = new Vector3(Mathf.Max(Max.X, bounds.Max.X),
                Mathf.Max(Max.Y, bounds.Max.Y),
                Mathf.Max(Max.Z, bounds.Max.Z));
            SetMinMax(newMin, newMax);
        }
        
        /// <summary>
        /// Expand the bounds by increasing its size by <c>amount</c> along each side.
        /// </summary>
        /// <param name="amount"></param>
        public void Expand(Vector3 amount)
        {
            Size += amount;
        }
        
        /// <summary>
        /// Does another bounding box intersect with this bounding box?
        /// </summary>
        public bool Intersects(Bounds bounds)
        {
            if (bounds.Min.X > Max.X || Min.X > bounds.Max.X)
                return false;
            if (bounds.Min.Y > Max.Y || Min.Y > bounds.Max.Y)
                return false;
            if (bounds.Min.Z > Max.Z || Min.Z > bounds.Max.Z)
                return false;

            return true;
        }

        /// <summary>
        /// Sets the bounds to the <c>min</c> and <c>max</c> value of the box.
        /// </summary>
        /// <remarks>
        /// Using this function is faster than assigning <c>min</c> and <c>max</c> separately.
        /// </remarks>
        public void SetMinMax(Vector3 min, Vector3 max)
        {
            Center = new Vector3((min.X + max.X) / 2, (min.Y + max.Y) / 2, (min.Z + max.Z) / 2);
            Size = new Vector3(Mathf.Abs(max.X - min.X), Mathf.Abs(max.Y - min.Y), Mathf.Abs(max.Z - min.Z));
        }

        /// <summary>
        /// The smallest squared distance between the point and this bounding box.
        /// </summary>
        public float SqrDistance(Vector3 point)
        {
            var dx = Mathf.Max(Min.X - point.X, 0, point.X - Max.X);
            var dy = Mathf.Max(Min.Y - point.Y, 0, point.Y - Max.Y);
            var dz = Mathf.Max(Min.Z - point.Z, 0, point.Z - Max.Z);
            return dx * dx + dy * dy + dz * dz;
        }
    }
}