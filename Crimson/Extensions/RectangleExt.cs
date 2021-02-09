using Microsoft.Xna.Framework;

namespace Crimson
{
    public static class RectangleExt
    {
        private static Vector2 RotateAround(Vector2 v, float radians, Vector2 around)
        {
            float newX = around.X + (v.X - around.X) * Mathf.Cos(radians) + (v.Y - around.Y) * Mathf.Sin(radians);
            float newY = around.Y - (v.X - around.X) * Mathf.Sin(radians) + (v.Y - around.Y) * Mathf.Cos(radians);
            return new Vector2(newX, newY);
        }
        public static Rectangle GetRotatedBoundingRect(this Rectangle rect, float radians, Vector2 rotateAround)
        {
            Vector2 c1 = new Vector2(rect.Left, rect.Top);
            Vector2 c2 = new Vector2(rect.Left, rect.Bottom);
            Vector2 c3 = new Vector2(rect.Right, rect.Top);
            Vector2 c4 = new Vector2(rect.Right, rect.Bottom);

            var c1R = RotateAround(c1, radians, rotateAround);
            var c2R = RotateAround(c2, radians, rotateAround);
            var c3R = RotateAround(c3, radians, rotateAround);
            var c4R = RotateAround(c4, radians, rotateAround);

            float minX = Mathf.Min(c1R.X, c2R.X, c3R.X, c4R.X);
            float maxX = Mathf.Max(c1R.X, c2R.X, c3R.X, c4R.X);
            float minY = Mathf.Min(c1R.Y, c2R.Y, c3R.Y, c4R.Y);
            float maxY = Mathf.Max(c1R.Y, c2R.Y, c3R.Y, c4R.Y);

            return new Rectangle(Mathf.FloorToInt(minX), Mathf.FloorToInt(minY), Mathf.CeilToInt(maxX - minX), Mathf.CeilToInt(maxY - minY));
        }
    }
}
