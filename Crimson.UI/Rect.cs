using Microsoft.Xna.Framework;

namespace Crimson.UI
{
    public class Rect
    {
        public Vector2 Position;
        public Size Size;

        public Rect(float x, float y, float width, float height)
        {
            Position = new Vector2(x, y);
            Size = new Size(width, height);
        }

        public Rect() : this(0, 0, 0, 0) { }

        public Rect(Vector2 position, Size size)
        {
            Position = position;
            Size = size;
        }
        
        public float X
        {
            get => Position.X;
            set => Position.X = value;
        }

        public float Y
        {
            get => Position.Y;
            set => Position.Y = value;
        }

        public float Width
        {
            get => Size.Width;
            set => Size.Width = value;
        }

        public float Height
        {
            get => Size.Height;
            set => Size.Height = value;
        }

        public float Left   => X;
        public float Right  => X + Width;
        public float Top    => Y;
        public float Bottom => Y + Height;

        public static Rect RectFromSides(float left, float top, float right, float bottom) =>
            new Rect(left, top, right - left, bottom - top);

        /// <summary>
        /// Returns <c>true</c> if the given point is inside or on the edge of the
        /// rectangle, otherwise returns <c>false</c>. If proper is true, this
        /// function only returns <c>true</c> if the given point is inside the
        /// rectangle (i.e., not on the edge).
        /// </summary>
        public bool Contains(Vector2 point, bool proper = false)
        {
            if ( proper )
            {
                return point.X > Left && point.X < Right &&
                       point.Y > Top  && point.Y < Bottom;
            }

            return point.X >= Left && point.X <= Right &&
                   point.Y >= Top  && point.Y <= Bottom;
        }

        public Rect ExpandedTo(Rect other) => RectFromSides(
            Mathf.Min(Left, other.Left),
            Mathf.Min(Top,  other.Top),
            Mathf.Max(Right,  other.Right),
            Mathf.Max(Bottom, other.Bottom)
        );

        public Rect Adjusted(float xp1, float yp1, float xp2, float yp2)
        {
            return RectFromSides(
                Left + xp1, Top + yp1, Right + xp2, Bottom + yp2
            );
        }
    }
}