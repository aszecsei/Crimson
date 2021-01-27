using Microsoft.Xna.Framework;

namespace Crimson
{
    [Tracked(false)]
    public class Text : GraphicsComponent
    {
        public enum HorizontalAlign
        {
            Left,
            Center,
            Right
        };

        public enum VerticalAlign
        {
            Top,
            Center,
            Bottom
        };

        private BmFont _font;
        private float _fontSize;
        private string _text;
        private HorizontalAlign _horizontalAlign;
        private VerticalAlign _verticalAlign;
        private Vector2 _size;

        public Text(BmFont font, float fontSize, string text, Vector2 position, Color color,
            HorizontalAlign horizontalAlign = HorizontalAlign.Center,
            VerticalAlign verticalAlign = VerticalAlign.Center)
            : base(false)
        {
            _font = font;
            _fontSize = fontSize;
            _text = text;
            Position = position;
            Color = color;
            _horizontalAlign = horizontalAlign;
            _verticalAlign = verticalAlign;
            UpdateSize();
        }

        public Text(BmFont font, float fontSize, string text, Vector2 position,
            HorizontalAlign horizontalAlign = HorizontalAlign.Center,
            VerticalAlign verticalAlign = VerticalAlign.Center)
            : this(font, fontSize, text, position, Microsoft.Xna.Framework.Color.White, horizontalAlign, verticalAlign)
        {
            
        }

        public BmFont Font
        {
            get => _font;
            set
            {
                _font = value;
                UpdateSize();
            }
        }

        public float FontSize
        {
            get => _fontSize;
            set
            {
                _fontSize = value;
                UpdateSize();
            }
        }

        public string DrawText
        {
            get => _text;
            set
            {
                _text = value;
                UpdateSize();
            }
        }

        public HorizontalAlign HorizontalOrigin
        {
            get => _horizontalAlign;
            set
            {
                _horizontalAlign = value;
                UpdateCentering();
            }
        }
        
        public VerticalAlign VerticalOrigin
        {
            get => _verticalAlign;
            set
            {
                _verticalAlign = value;
                UpdateCentering();
            }
        }

        public float Width => _size.X;

        public float Height => _size.Y;

        private void UpdateSize()
        {
            _size = _font.MeasureString(_fontSize, _text);
            UpdateCentering();
        }
        
        private void UpdateCentering()
        {
            if (_horizontalAlign == HorizontalAlign.Left)
                Origin.X = 0;
            else if (_horizontalAlign == HorizontalAlign.Center)
                Origin.X = _size.X / 2;
            else
                Origin.X = _size.X;

            if (_verticalAlign == VerticalAlign.Top)
                Origin.Y = 0;
            else if (_verticalAlign == VerticalAlign.Center)
                Origin.Y = _size.Y / 2;
            else
                Origin.Y = _size.Y;

            Origin = Mathf.Floor(Origin);
        }

        public override void Render()
        {
            _font.Draw(_fontSize, _text, RenderPosition - Origin, Color);
        }
    }
}
