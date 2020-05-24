using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Crimson
{
    public class GraphicsComponent : Component
    {
        public Color Color = Color.White;
        public SpriteEffects Effects = SpriteEffects.None;
        public Vector2 Origin;
        public Vector2 Position;
        public float Rotation;
        public Vector2 Scale = Vector2.One;

        public GraphicsComponent(bool active)
            : base(active, true)
        {
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

        public bool FlipX
        {
            get => (Effects & SpriteEffects.FlipHorizontally) == SpriteEffects.FlipHorizontally;
            set => Effects = value
                ? Effects | SpriteEffects.FlipHorizontally
                : Effects & ~SpriteEffects.FlipHorizontally;
        }

        public bool FlipY
        {
            get => (Effects & SpriteEffects.FlipVertically) ==
                   SpriteEffects.FlipVertically;
            set => Effects = value ? Effects | SpriteEffects.FlipVertically : Effects & ~SpriteEffects.FlipVertically;
        }

        public Vector2 RenderPosition
        {
            get => (Entity == null ? Vector2.Zero : Entity.Position) + Position;
            set => Position = value - (Entity == null ? Vector2.Zero : Entity.Position);
        }

        public void DrawOutline(int offset = 1)
        {
            DrawOutline(Color.Black, offset);
        }

        public void DrawOutline(Color color, int offset = 1)
        {
            var pos = Position;
            var was = Color;
            Color = color;

            for (var i = -1; i < 2; i++)
            for (var j = -1; j < 2; j++)
                if (i != 0 || j != 0)
                {
                    Position = pos + new Vector2(i * offset, j * offset);
                    Render();
                }

            Position = pos;
            Color = was;
        }
    }
}