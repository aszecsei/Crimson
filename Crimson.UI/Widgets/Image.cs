using Microsoft.Xna.Framework;

namespace Crimson.UI
{
    public class Image : LeafWidget
    {
        /// <summary>
        /// Describes how an image is resized to fill its allocated space.
        /// </summary>
        public enum Scalings
        {
            /// <summary>
            /// The image preserves its original size.
            /// </summary>
            None = 0,
            /// <summary>
            /// The content is resized to fill the destination
            /// dimensions. The aspect ratio is not preserved.
            /// </summary>
            Fill = 1,
            /// <summary>
            /// The content is resized to fit in the destination
            /// dimensions while it preserves its native aspect ratio.
            /// </summary>
            Uniform = 2,
            /// <summary>
            /// The content is resized to fill the destination dimensions
            /// while it preserves its native aspect ratio. If the aspect
            /// ratio of the destination differs from the source, the
            /// source content is clipped to fit in the destination
            /// dimensions.
            /// </summary>
            UniformToFill = 3
        }
        
        private Scalings _scaling;

        private CTexture _texture;

        public Image(CTexture texture = null) => Texture = texture;

        public CTexture Texture
        {
            get => _texture;
            set
            {
                if ( _texture != value )
                {
                    if ( value          == null || _texture == null || _texture.Height != value.Height ||
                         _texture.Width != value.Width )
                    {
                        Invalidate();
                    }

                    _texture = value;
                }
            }
        }

        public Scalings Scaling
        {
            get => _scaling;
            set
            {
                if ( _scaling != value )
                {
                    Invalidate();
                    _scaling = value;
                }
            }
        }

        public override Size PrefSize
        {
            get
            {
                if ( _texture == null )
                {
                    return new Size(0, 0);
                }

                return new Size(_texture.Width, _texture.Height);
            }
        }

        public override bool CanSupportFocus => false;
        public override Size MinSize => new Size(0,              0);
        public override Size MaxSize => new Size(Mathf.INFINITY, Mathf.INFINITY);

        public override void Render(float parentAlpha)
        {
            base.Render(parentAlpha);

            if ( _texture == null )
            {
                return;
            }

            float width  = _texture.Width;
            float height = _texture.Height;
            switch ( Scaling )
            {
            case Scalings.Fill:
                width  = Geometry.Width;
                height = Geometry.Height;
                break;
            case Scalings.None:
                break;
            case Scalings.Uniform:
            {
                float aspect = width / height;
                if ( aspect * Geometry.Height > Geometry.Width )
                {
                    // Fit to width
                    width  = Geometry.Width;
                    height = width / aspect;
                }
                else
                {
                    height = Geometry.Height;
                    width  = height * aspect;
                }
            }
                break;
            case Scalings.UniformToFill:
            {
                float aspect = width / height;
                if ( aspect * Geometry.Height > Geometry.Width )
                {
                    // Fill width
                    height = Geometry.Height;
                    width  = height * aspect;
                }
                else
                {
                    width  = Geometry.Width;
                    height = width / aspect;
                }
            }
                break;
            }

            float offsetX = (Geometry.Width  - width)  / 2;
            float offsetY = (Geometry.Height - height) / 2;

            var scale = new Vector2(width / _texture.Width, height / _texture.Height);
            var color = new Color(Color.White, (int)(RenderOpacity * parentAlpha));
            Texture.Draw(Geometry.Position + new Vector2(offsetX, offsetY), Vector2.Zero, color, scale);
        }
    }
}