namespace Crimson.UI
{
    public class Size
    {
        public float Height;
        public float Width;

        public Size(float width, float height)
        {
            Width = width;
            Height = height;
        }
        
        public Size() : this(0, 0) { }
        
        public Size ExpandedTo(Size other) => new Size(
            Mathf.Max(Width, other.Width),
            Mathf.Max(Height, other.Height));

        public Size BoundedTo(Size other) => new Size(
            Mathf.Min(Width, other.Width),
            Mathf.Min(Height, other.Height));

        public bool Valid => Width >= 0 && Height >= 0;

        public static Size operator +(Size lhs, Size rhs)
        {
            return new Size(lhs.Width + rhs.Width, lhs.Height + rhs.Height);
        }
    }
}