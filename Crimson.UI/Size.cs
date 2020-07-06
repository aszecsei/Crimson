namespace Crimson.UI
{
    public class Size
    {
        private static readonly Size ZeroSize = new Size(0, 0);
        private static readonly Size InfiniteSize = new Size(Mathf.INFINITY, Mathf.INFINITY);
        
        public float Height;
        public float Width;
        
        public static Size Zero => ZeroSize;
        public static Size Infinite => InfiniteSize;

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