using Microsoft.Xna.Framework;

namespace Crimson.Physics
{
    public class JumpThru : Platform
    {
        public int Rotation;

        public JumpThru(int rotation = 0) : this(Vector2.Zero, rotation)
        {
        }

        public JumpThru(Vector2 position, int rotation = 0)
            : base(position)
        {
            Rotation = rotation;
        }
    }
}
