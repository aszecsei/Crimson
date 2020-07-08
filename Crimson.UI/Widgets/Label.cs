using System;
using Microsoft.Xna.Framework;

namespace Crimson.UI
{
    public class Label : LeafWidget
    {
        private BmFont _font;
        private float _size;
        private Vector2 _prefSize;
        private float _kerning = 0f;
        private float _lineHeight = 1f;

        private bool _prefSizeInvalid;
        private string _text;
        private Letter[] _letters;
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
                UpdateLetters();
                
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

        public float Kerning
        {
            get => _kerning;
            set
            {
                if (Mathf.Approximately(_kerning, value)) return;

                _kerning = value;
                _prefSizeInvalid = true;
                Invalidate();
            }
        }

        public float LineHeight
        {
            get => _lineHeight;
            set
            {
                if (Mathf.Approximately(_lineHeight, value)) return;

                _lineHeight = value;
                _prefSizeInvalid = true;
                Invalidate();
            }
        }

        public Letter this[int index]
        {
            get => _letters[index];
            set
            {
                if (_letters[index] == value) return;
                
                _letters[index] = value;
                _text = _text.ReplaceAt(index, value.Char);
                _prefSizeInvalid = true;
                Invalidate();
            }
        }

        public Label(string text, BmFont font, float size = 32f)
        {
            _text = text;
            _letters = new Letter[0];
            UpdateLetters();
            _font = font;
            _size = size;

            _prefSizeInvalid = true;
        }

        private void UpdateLetters()
        {
            if (_letters.Length != _text.Length)
            {
                Letter[] newLetters = new Letter[_text.Length];
                Array.Copy(_letters, newLetters, Mathf.Min(_letters.Length, newLetters.Length));
                _letters = newLetters;
            }

            for (int i = 0; i < _text.Length; ++i)
            {
                if (_letters[i] == null) _letters[i] = new Letter(_text[i]);
                else _letters[i].Char = _text[i];
            }
        }

        private void ComputePrefSize()
        {
            _prefSizeInvalid = false;
            
            var scale = Vector2.One;
            var fontSize = _font.Get(_size * Mathf.Max(scale.X, scale.Y));
            scale *= (_size / fontSize.Size);

            _prefSize = new Vector2(0, LineHeight * fontSize.LineHeight);
            var currentLineWidth = 0f;

            for (var i = 0; i < _text.Length; i++)
            {
                if (_text[i] == '\n')
                {
                    _prefSize.Y += LineHeight * fontSize.LineHeight;
                    if (currentLineWidth > _prefSize.X)
                        _prefSize.X = currentLineWidth;
                    currentLineWidth = 0f;
                }
                else
                {
                    if (fontSize.Characters.TryGetValue(_text[i], out var c))
                    {
                        currentLineWidth += c.XAdvance;

                        if (i < _letters.Length - 1 && c.Kerning.TryGetValue(_text[i + 1], out var kerning))
                            currentLineWidth += kerning + (_kerning / scale.X);
                    }
                }
            }

            if (currentLineWidth > _prefSize.X)
                _prefSize.X = currentLineWidth;

            _prefSize *= scale;
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

        public override void Update()
        {
            base.Update();

            for (int i = 0; i < _letters.Length; ++i)
                _letters[i].TimeExisted += Time.DeltaTime;
        }

        public override void Render(float parentAlpha)
        {
            base.Render(parentAlpha);
            
            if (string.IsNullOrEmpty(_text))
                return;

            var color = new Color(Color, Color.A * parentAlpha * RenderOpacity);
            var position = Geometry.Position;
            
            var scale = Vector2.One;
            var fontSize = _font.Get(_size * Mathf.Max(scale.X, scale.Y));
            scale *= (_size / fontSize.Size);

            var offset = Vector2.Zero;

            for (var i = 0; i < _text.Length; i++)
            {
                if (_text[i] == '\n')
                {
                    offset.X = 0;
                    offset.Y += LineHeight * fontSize.LineHeight;
                    continue;
                }

                if (fontSize.Characters.TryGetValue(_text[i], out var c))
                {
                    var pos = (position + (offset + new Vector2(c.XOffset, c.YOffset)) * scale);
                    pos += _letters[i].TransformedPosition / scale;

                    // draw normal character
                    c.Texture.Draw(pos, Vector2.Zero, color, scale * _letters[i].TransformedScale);

                    offset.X += c.XAdvance;

                    if (i < _text.Length - 1 && c.Kerning.TryGetValue(_text[i + 1], out var kerning))
                        offset.X += kerning + (_kerning / scale.X);
                }
            }
        }

        public override bool CanSupportFocus => false;
    }
}