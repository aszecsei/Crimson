using Microsoft.Xna.Framework;

namespace Crimson
{
    public class Image : GraphicsComponent
    {
        public CTexture Texture;

        public Image(CTexture texture)
            : base(false)
        {
            Texture = texture;
        }

        internal Image(CTexture texture, bool active)
            : base(active)
        {
            Texture = texture;
        }

        public bool Outline = false;

        public virtual float Width => Texture.Width;

        public virtual float Height => Texture.Height;

        public override void Render()
        {
            if ( !Outline )
                Texture?.Draw(RenderPosition, Origin, Color, Scale, Rotation, Effects);
            else
                Texture?.DrawOutline(RenderPosition, Origin, Color, Scale, Rotation, Effects);
        }

        public Image SetOrigin(float x, float y)
        {
            Origin.X = x;
            Origin.Y = y;
            return this;
        }

        public Image CenterOrigin()
        {
            Origin.X = Width / 2f;
            Origin.Y = Height / 2f;
            return this;
        }

        public Image JustifyOrigin(Vector2 at)
        {
            Origin.X = Width * at.X;
            Origin.Y = Height * at.Y;
            return this;
        }

        public Image JustifyOrigin(float x, float y)
        {
            Origin.X = Width * x;
            Origin.Y = Height * y;
            return this;
        }
    }
}
