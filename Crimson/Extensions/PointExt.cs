using Microsoft.Xna.Framework;

namespace Crimson
{
    public static class PointExt
    {
        public static Point Scale(this Point p, int i)
        {
            return new Point(p.X * i, p.Y * i);
        }
    }
}
