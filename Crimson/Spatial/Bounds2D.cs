using Microsoft.Xna.Framework;

namespace Crimson.Spatial
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
            get => new Vector2(Size.X / 2, Size.Y / 2);
            set => Size = new Vector2(value.X * 2, value.Y * 2);
        }

        /// <summary>
        /// The maximal point of the box. This is always equal to center+extents.
        /// Setting this value keeps the old bounds' min in place. If the new max
        /// is less than the min, the values are swapped.
        /// </summary>
        public Vector2 Max
        {
            get => Center + Extents;
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
            get { return Center - Extents; }
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
        public Vector2 ClosestPoint(Vector2 point)
        {
            var dx = Mathf.Max(Min.X - point.X, 0, point.X - Max.X);
            var dy = Mathf.Max(Min.Y - point.Y, 0, point.Y - Max.Y);
            return new Vector2(point.X + dx, point.Y + dy);
        }

        /// <summary>
        /// Is <c>point</c> contained in the bounding box?
        /// </summary>
        public bool Contains(Vector2 point)
        {
            return point.X >= Min.X && point.Y >= Min.Y
                && point.X <= Max.X && point.Y <= Max.Y;
        }

        /// <summary>
        /// Grows the bounds to include the point.
        /// </summary>
        public void Encapsulate(Vector2 point)
        {
            var newMin = new Vector2(Mathf.Min(Min.X, point.X),
                Mathf.Min(Min.Y, point.Y));
            var newMax = new Vector2(Mathf.Max(Max.X, point.X),
                Mathf.Max(Max.Y, point.Y));
            SetMinMax(newMin, newMax);
        }
        
        /// <summary>
        /// Grows the bounds to include the bounds.
        /// </summary>
        public void Encapsulate(Bounds2D bounds)
        {
            var newMin = new Vector2(Mathf.Min(Min.X, bounds.Min.X),
                Mathf.Min(Min.Y, bounds.Min.Y));
            var newMax = new Vector2(Mathf.Max(Max.X, bounds.Max.X),
                Mathf.Max(Max.Y, bounds.Max.Y));
            SetMinMax(newMin, newMax);
        }

        /// <summary>
        /// Expand the bounds by increasing its size by <c>amount</c> along each side.
        /// </summary>
        /// <param name="amount"></param>
        public void Expand(Vector2 amount)
        {
            Size += amount;
        }

        /// <summary>
        /// Does <c>ray</c> intersect this bounding box?
        /// </summary>
        /// <param name="ray"></param>
        /// <returns></returns>
        public bool IntersectRay(Ray2D ray)
        {
            Vector2 r_inv = new Vector2(1 / ray.Direction.X, 1 / ray.Direction.Y);
            float tx1 = (Min.X - ray.Origin.X) * r_inv.X;
            float tx2 = (Max.X - ray.Origin.X) * r_inv.X;

            float tMin = Mathf.Min(tx1, tx2);
            float tMax = Mathf.Max(tx1, tx2);

            float ty1 = (Min.Y - ray.Origin.Y) * r_inv.Y;
            float ty2 = (Max.Y - ray.Origin.Y) * r_inv.Y;

            tMin = Mathf.Max(tMin, Mathf.Min(ty1, ty2));
            tMax = Mathf.Min(tMax, Mathf.Max(ty1, ty2));

            return (tMax >= Mathf.Max(tMin, 0));
        }

        /// <summary>
        /// Does <c>ray</c> intersect this bounding box?
        /// </summary>
        /// <remarks>
        /// When the function returns true, the distance to the ray's origin will be
        /// returned in the <c>distance</c> parameter.
        /// </remarks>
        public bool IntersectRay(Ray2D ray, out float distance)
        {
            Vector2 r_inv = new Vector2(1 / ray.Direction.X, 1 / ray.Direction.Y);
            float tx1 = (Min.X - ray.Origin.X) * r_inv.X;
            float tx2 = (Max.X - ray.Origin.X) * r_inv.X;

            float tMin = Mathf.Min(tx1, tx2);
            float tMax = Mathf.Max(tx1, tx2);

            float ty1 = (Min.Y - ray.Origin.Y) * r_inv.Y;
            float ty2 = (Max.Y - ray.Origin.Y) * r_inv.Y;

            tMin = Mathf.Max(tMin, Mathf.Min(ty1, ty2));
            tMax = Mathf.Min(tMax, Mathf.Max(ty1, ty2));

            distance = Mathf.Max(tMin, 0);
            return tMax >= distance;
        }

        /// <summary>
        /// Does another bounding box intersect with this bounding box?
        /// </summary>
        public bool Intersects(Bounds2D bounds)
        {
            if (bounds.Min.X > Max.X || Min.X > bounds.Max.X)
                return false;
            if (bounds.Min.Y > Max.Y || Min.Y > bounds.Max.Y)
                return false;

            return true;
        }

        /// <summary>
        /// Sets the bounds to the <c>min</c> and <c>max</c> value of the box.
        /// </summary>
        /// <remarks>
        /// Using this function is faster than assigning <c>min</c> and <c>max</c> separately.
        /// </remarks>
        public void SetMinMax(Vector2 min, Vector2 max)
        {
            Center = new Vector2((min.X + max.X) / 2, (min.Y + max.Y) / 2);
            Size = new Vector2(Mathf.Abs(max.X - min.X), Mathf.Abs(max.Y - min.Y));
        }

        /// <summary>
        /// The smallest squared distance between the point and this bounding box.
        /// </summary>
        public float SqrDistance(Vector2 point)
        {
            var dx = Mathf.Max(Min.X - point.X, 0, point.X - Max.X);
            var dy = Mathf.Max(Min.Y - point.Y, 0, point.Y - Max.Y);
            return dx * dx + dy * dy;
        }
    }
}