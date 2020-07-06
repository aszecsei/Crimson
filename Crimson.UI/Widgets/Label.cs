using Microsoft.Xna.Framework;

namespace Crimson.UI
{
    public class Label : LeafWidget
    {
        private readonly float _lastPrefHeight;
        private BmFont _font;
        private float _size;
        private Vector2 _prefSize;

        private bool _prefSizeInvalid;
        private string _text;
        private Vector2 _textPosition;
        
        // TODO: Justify text

        public Color Color = Color.White;

        public string Text
        {
            get => _text;
            set
            {
                if (_text == value) return;
                
                _text = value;
                _prefSizeInvalid = true;
                Invalidate();
            }
        }

        public BmFont Font
        {
            get => _font;
            set
            {
                if (_font == value) return;

                _font = value;
                _prefSizeInvalid = true;
                Invalidate();
            }
        }

        public float Size
        {
            get => _size;
            set
            {
                if (Mathf.Approximately(_size, value)) return;

                _size = value;
                _prefSizeInvalid = true;
                Invalidate();
            }
        }

        public Label(string text, BmFont font, float size = 32f)
        {
            Text = text;
            Font = font;
            Size = size;
        }

        private void ComputePrefSize()
        {
            _prefSizeInvalid = false;
            _prefSize = _font.MeasureString(_size, _text);
        }

        public override Size MinSize => PrefSize;

        public override Size MaxSize => PrefSize;

        public override Size PrefSize
        {
            get
            {
                if (_prefSizeInvalid) ComputePrefSize();
                return new Size(_prefSize.X, _prefSize.Y);
            }
        }

        public override void Layout()
        {
            base.Layout();
            
            // TODO: Update text position
        }

        public override void Render(float parentAlpha)
        {
            base.Render(parentAlpha);

            var color = new Color(Color, Color.A * parentAlpha * RenderOpacity);
            _font.Draw(_size, _text, Geometry.Position, color);
        }

        public override bool CanSupportFocus => false;
    }
}