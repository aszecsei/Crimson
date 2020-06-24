using Microsoft.Xna.Framework;

namespace Crimson
{
    public class FRect
    {
        public float X;
        public float Y;
        public float Width;
        public float Height;

        public FRect(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public float Top => Y;
        public float Bottom => Y + Height;
        public float Left => X;
        public float Right => X + Width;
        
        public Vector2 Position => new Vector2(X, Y);
    }
}