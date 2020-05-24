using Microsoft.Xna.Framework;

namespace Crimson.Spatial
{
    public struct Ray2D
    {
        public Vector2 Direction;
        public Vector2 Origin;

        public Ray2D(Vector2 direction, Vector2 origin)
        {
            Direction = direction;
            Origin = origin;
        }

        public Vector2 GetPoint(float distance)
        {
            return Origin + distance * Direction.Normalized();
        }
    }
}