using Microsoft.Xna.Framework;

namespace Crimson
{
    public struct Bounds2D
    {
        /// <summary>
        /// The center of the bounding box.
        /// </summary>
        public Vector2 Center;
        /// <summary>
        /// The total size of the box. This is always twice as large
        /// as the extents.
        /// </summary>
        public Vector2 Size;

        /// <summary>
        /// The extents of the bounding box. This is always half the
        /// size of the bounds.
        /// </summary>
        public Vector2 Extents
        {
            readonly get => new Vector2(Size.X / 2, Size.Y / 2);
            set => Size = new Vector2(value.X * 2, value.Y * 2);
        }

        /// <summary>
        /// The maximal point of the box. This is always equal to center+extents.
        /// Setting this value keeps the old bounds' min in place. If the new max
        /// is less than the min, the values are swapped.
        /// </summary>
        public Vector2 Max
        {
            readonly get => Center + Extents;
            set
            {
                var (x, y) = Min;
                Center = new Vector2((value.X + x) / 2, (value.Y + y) / 2);
                Size = new Vector2(Mathf.Abs(value.X - x), Mathf.Abs(value.Y - y));
            }
        }

        /// <summary>
        /// The minimal point of the box. This is always equal to center-extents.
        /// Setting this value keeps the old bounds' max in place. If the new min
        /// is greater than the max, the values are swapped.
        /// </summary>
        public Vector2 Min
        {
            readonly get => Center - Extents;
            set
            {
                var (x, y) = Max;
                Center = new Vector2((value.X + x) / 2, (value.Y + y) / 2);
                Size = new Vector2(Mathf.Abs(x - value.X), Mathf.Abs(y - value.Y));
            }
        }

        /// <summary>
        /// Creates a new bounds.
        /// </summary>
        public Bounds2D(Vector2 center, Vector2 size)
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
        public readonly Vector2 ClosestPoint(in Vector2 point)
        {
            return point.Clamp(Min, Max);
        }

        /// <summary>
        /// Is <c>point</c> contained in the bounding box?
        /// </summary>
        public readonly bool Contains(in Vector2 point)
        {
            return point.X >= Min.X && point.Y >= Min.Y
                && point.X <= Max.X && point.Y <= Max.Y;
        }

        /// <summary>
        /// Grows the bounds to include the point.
        /// </summary>
        public void Encapsulate(in Vector2 point)
        {
            var newMin = new Vector2(Mathf.Min(Min.X, point.X),
                Mathf.Min(Min.Y, point.Y));
            var newMax = new Vector2(Mathf.Max(Max.X, point.X),
                Mathf.Max(Max.Y, point.Y));
            SetMinMax(in newMin, in newMax);
        }
        
        /// <summary>
        /// Grows the bounds to include the bounds.
        /// </summary>
        public void Encapsulate(in Bounds2D bounds)
        {
            var newMin = new Vector2(Mathf.Min(Min.X, bounds.Min.X),
                Mathf.Min(Min.Y, bounds.Min.Y));
            var newMax = new Vector2(Mathf.Max(Max.X, bounds.Max.X),
                Mathf.Max(Max.Y, bounds.Max.Y));
            SetMinMax(in newMin, in newMax);
        }

        /// <summary>
        /// Expand the bounds by increasing its size by <c>amount</c> along each side.
        /// </summary>
        /// <param name="amount"></param>
        public void Expand(in Vector2 amount)
        {
            Size += amount;
        }

        /// <summary>
        /// Does another bounding box intersect with this bounding box?
        /// </summary>
        public readonly bool Intersects(in Bounds2D bounds)
        {
            if (bounds.Min.X > Max.X || Min.X > bounds.Max.X)
                return false;
            if (bounds.Min.Y > Max.Y || Min.Y > bounds.Max.Y)
                return false;

            return true;
        }

        public readonly bool Intersects(in Rectangle rect)
        {
            if (rect.Left > Max.X || Min.X > rect.Right)
                return false;
            if (rect.Top > Max.Y || Min.Y > rect.Bottom)
                return false;
            return true;
        }

        /// <summary>
        /// Sets the bounds to the <c>min</c> and <c>max</c> value of the box.
        /// </summary>
        /// <remarks>
        /// Using this function is faster than assigning <c>min</c> and <c>max</c> separately.
        /// </remarks>
        public void SetMinMax(in Vector2 min, in Vector2 max)
        {
            Center = new Vector2((min.X + max.X) / 2, (min.Y + max.Y) / 2);
            Size = new Vector2(Mathf.Abs(max.X - min.X), Mathf.Abs(max.Y - min.Y));
        }

        /// <summary>
        /// The smallest squared distance between the point and this bounding box.
        /// </summary>
        public readonly float SqrDistance(in Vector2 point)
        {
            var dx = Mathf.Max(Min.X - point.X, 0, point.X - Max.X);
            var dy = Mathf.Max(Min.Y - point.Y, 0, point.Y - Max.Y);
            return dx * dx + dy * dy;
        }
    }
}