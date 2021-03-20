using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Microsoft.Xna.Framework;

namespace Crimson
{
    public class PixelFontCharacter
    {
        public readonly int Character;
        public readonly Dictionary<int, int> Kerning = new Dictionary<int, int>();
        public readonly CTexture Texture;
        public readonly int XAdvance;
        public readonly int XOffset;
        public readonly int YOffset;

        public PixelFontCharacter(
            int character,
            CTexture texture,
            int packX,
            int packY,
            int width,
            int height,
            int xOffset,
            int yOffset,
            int xAdvance
        )
        {
            Character = character;
            Texture = texture.GetSubtexture(packX, packY, width, height);
            XOffset = xOffset;
            YOffset = yOffset;
            XAdvance = xAdvance;
        }
    }

    public class PixelFontSize
    {
        private readonly StringBuilder _temp = new StringBuilder();
        public Dictionary<int, PixelFontCharacter> Characters = new Dictionary<int, PixelFontCharacter>();
        public int LineHeight;

        public bool Outline;

        // TODO: Implement ascent
        public float Size;
        public List<CTexture> Textures = new List<CTexture>();

        public string AutoNewline(string text, int width)
        {
            if (string.IsNullOrEmpty(text)) return text;

            _temp.Clear();

            var words = Regex.Split(text, @"(\s)");
            var lineWidth = 0f;

            foreach (var word in words)
            {
                var wordWidth = Measure(word).X;
                if (wordWidth + lineWidth > width)
                {
                    _temp.Append('\n');
                    lineWidth = 0;

                    if (word.Equals(" ")) continue;
                }

                // this word is longer than the max width, split wherever we can
                if (wordWidth > width)
                {
                    int i = 1, start = 0;
                    for (; i < word.Length; i++)
                        if (i - start > 1 && Measure(word.Substring(start, i - start - 1)).X > width)
                        {
                            _temp.Append(word.Substring(start, i - start - 1));
                            _temp.Append('\n');
                            start = i - 1;
                        }

                    var remaining = word.Substring(start, word.Length - start);
                    _temp.Append(remaining);
                    lineWidth += Measure(remaining).X;
                }

                // normal word, add it
                else
                {
                    lineWidth += wordWidth;
                    _temp.Append(word);
                }
            }

            return _temp.ToString();
        }

        public PixelFontCharacter? Get(int id)
        {
            return Characters.TryGetValue(id, out var val) ? val : null;
        }

        public Vector2 Measure(char text)
        {
            return Characters.TryGetValue(text, out var c) ? new Vector2(c.XAdvance, LineHeight) : Vector2.Zero;
        }

        public Vector2 Measure(string text)
        {
            if (string.IsNullOrEmpty(text)) return Vector2.Zero;

            var size = new Vector2(0, LineHeight);
            var currentLineWidth = 0f;

            for (var i = 0; i < text.Length; i++)
                if (text[i] == '\n')
                {
                    size.Y += LineHeight;
                    if (currentLineWidth > size.X) size.X = currentLineWidth;

                    currentLineWidth = 0f;
                }
                else
                {
                    if (Characters.TryGetValue(text[i], out var c))
                    {
                        currentLineWidth += c.XAdvance;
                        if (i < text.Length - 1 && c.Kerning.TryGetValue(text[i + 1], out var kerning))
                            currentLineWidth += kerning;
                    }
                }

            if (currentLineWidth > size.X) size.X = currentLineWidth;

            return size;
        }

        public float WidthToNextLine(string text, int start)
        {
            if (string.IsNullOrEmpty(text)) return 0;

            var currentLineWidth = 0f;

            for (int i = start, j = text.Length; i < j; i++)
            {
                if (text[i] == '\n') break;

                if (Characters.TryGetValue(text[i], out var c))
                {
                    currentLineWidth += c.XAdvance;

                    if (i < j - 1 && c.Kerning.TryGetValue(text[i + 1], out var kerning)) currentLineWidth += kerning;
                }
            }

            return currentLineWidth;
        }

        public float HeightOf(string text)
        {
            if (string.IsNullOrEmpty(text)) return 0;

            var lines = 1;
            if (text.IndexOf('\n') >= 0)
                for (var i = 0; i < text.Length; i++)
                    if (text[i] == '\n')
                        lines++;

            return lines * LineHeight;
        }

        public void Draw(char character, Vector2 position, Vector2 justify, Vector2 scale, Color color)
        {
            if (char.IsWhiteSpace(character)) return;

            if (Characters.TryGetValue(character, out var c))
            {
                var measure = Measure(character);
                var justified = new Vector2(measure.X * justify.X, measure.Y * justify.Y);
                var pos = position + (new Vector2(c.XOffset, c.YOffset) - justified) * scale;
                c.Texture.Draw(Mathf.Floor(pos), Vector2.Zero, color, scale);
            }
        }

        public void Draw(
            string text,
            Vector2 position,
            Vector2 justify,
            Vector2 scale,
            Color color,
            float edgeDepth,
            Color edgeColor,
            float stroke,
            Color strokeColor
        )
        {
            if (string.IsNullOrEmpty(text)) return;

            var offset = Vector2.Zero;
            var lineWidth = !Mathf.Approximately(justify.X, 0) ? WidthToNextLine(text, 0) : 0;
            var justified = new Vector2(lineWidth * justify.X, HeightOf(text) * justify.Y);

            for (var i = 0; i < text.Length; i++)
            {
                if (text[i] == '\n')
                {
                    offset.X = 0;
                    offset.Y += LineHeight;
                    if (!Mathf.Approximately(justify.X, 0)) justified.X = WidthToNextLine(text, i + 1) * justify.X;

                    continue;
                }

                if (Characters.TryGetValue(text[i], out var c))
                {
                    var pos = position + (offset + new Vector2(c.XOffset, c.YOffset) - justified) * scale;

                    // draw stroke
                    if (stroke > 0 && !Outline)
                    {
                        if (edgeDepth > 0)
                        {
                            c.Texture.Draw(pos + new Vector2(0, -stroke), Vector2.Zero, strokeColor, scale);
                            for (var j = -stroke; j < edgeDepth + stroke; j += stroke)
                            {
                                c.Texture.Draw(pos + new Vector2(-stroke, j), Vector2.Zero, strokeColor, scale);
                                c.Texture.Draw(pos + new Vector2(stroke, j), Vector2.Zero, strokeColor, scale);
                            }

                            c.Texture.Draw(pos + new Vector2(-stroke, edgeDepth + stroke), Vector2.Zero, strokeColor,
                                scale);
                            c.Texture.Draw(pos + new Vector2(0, edgeDepth + stroke), Vector2.Zero, strokeColor, scale);
                            c.Texture.Draw(pos + new Vector2(stroke, edgeDepth + stroke), Vector2.Zero, strokeColor,
                                scale);
                        }
                        else
                        {
                            c.Texture.Draw(pos + new Vector2(-1, -1) * stroke, Vector2.Zero, strokeColor, scale);
                            c.Texture.Draw(pos + new Vector2(0, -1) * stroke, Vector2.Zero, strokeColor, scale);
                            c.Texture.Draw(pos + new Vector2(1, -1) * stroke, Vector2.Zero, strokeColor, scale);
                            c.Texture.Draw(pos + new Vector2(-1, 0) * stroke, Vector2.Zero, strokeColor, scale);
                            c.Texture.Draw(pos + new Vector2(1, 0) * stroke, Vector2.Zero, strokeColor, scale);
                            c.Texture.Draw(pos + new Vector2(-1, 1) * stroke, Vector2.Zero, strokeColor, scale);
                            c.Texture.Draw(pos + new Vector2(0, 1) * stroke, Vector2.Zero, strokeColor, scale);
                            c.Texture.Draw(pos + new Vector2(1, 1) * stroke, Vector2.Zero, strokeColor, scale);
                        }
                    }

                    // draw edge
                    if (edgeDepth > 0) c.Texture.Draw(pos + Vector2.UnitY * edgeDepth, Vector2.Zero, edgeColor, scale);

                    // draw normal character
                    c.Texture.Draw(pos, Vector2.Zero, color, scale);

                    offset.X += c.XAdvance;

                    if (i < text.Length - 1 && c.Kerning.TryGetValue(text[i + 1], out var kerning)) offset.X += kerning;
                }
            }
        }

        public void Draw(string text, Vector2 position, Color color)
        {
            Draw(text, position, Vector2.Zero, Vector2.One, color, 0, Color.Transparent, 0, Color.Transparent);
        }

        public void Draw(string text, Vector2 position, Vector2 justify, Vector2 scale, Color color)
        {
            Draw(text, position, justify, scale, color, 0, Color.Transparent, 0, Color.Transparent);
        }

        public void DrawOutline(
            string text,
            Vector2 position,
            Vector2 justify,
            Vector2 scale,
            Color color,
            float stroke,
            Color strokeColor
        )
        {
            Draw(text, position, justify, scale, color, 0f, Color.Transparent, stroke, strokeColor);
        }

        public void DrawEdgeOutline(
            string text,
            Vector2 position,
            Vector2 justify,
            Vector2 scale,
            Color color,
            float edgeDepth,
            Color edgeColor,
            float stroke = 0f,
            Color strokeColor = default
        )
        {
            Draw(text, position, justify, scale, color, edgeDepth, edgeColor, stroke, strokeColor);
        }
    }

    /// <summary>
    /// A class to deal with rasterized fonts. This uses the data format provided by
    /// <a href="https://chevyray.itch.io/pixel-fonts">Chevy Ray</a>.
    /// </summary>
    public class PixelFont : IDisposable, IFont
    {
        public string Face;
        public List<PixelFontSize> Sizes = new List<PixelFontSize>();
        public List<CTexture> Textures = new List<CTexture>();

        public PixelFont(string face)
        {
            Face = face;
        }

        public PixelFontSize AddFontSize(string path, CTexture texture, bool outline = false)
        {
            var data = Utils.LoadXML(path)["metrics"];
            return AddFontSize(path, texture, data, outline);
        }

        public PixelFontSize AddFontSize(string path, CTexture texture, XmlElement data, bool outline = false)
        {
            // check if size already exists
            var size = data["size"].InnerFloat();
            foreach (var fs in Sizes)
                if (Mathf.Approximately(fs.Size, size))
                    return fs;

            // create font size
            var fontSize = new PixelFontSize
            {
                Textures = Textures,
                Characters = new Dictionary<int, PixelFontCharacter>(),
                LineHeight = data["ascent"].InnerInt() - data["descent"].InnerInt(),
                Size = size,
                Outline = outline
            };

            // get characters
            var charCount = data["char_count"].InnerInt();
            var charIds = Utils.ReadCSVInt(data["chars"].InnerText);
            var advance = Utils.ReadCSVInt(data["advance"].InnerText);
            var offsetX = Utils.ReadCSVInt(data["offset_x"].InnerText);
            var offsetY = Utils.ReadCSVInt(data["offset_y"].InnerText);
            var width = Utils.ReadCSVInt(data["width"].InnerText);
            var height = Utils.ReadCSVInt(data["height"].InnerText);
            var packX = Utils.ReadCSVInt(data["pack_x"].InnerText);
            var packY = Utils.ReadCSVInt(data["pack_y"].InnerText);
            for (var i = 0; i < charCount; i++)
            {
                var id = charIds[i];
                fontSize.Characters.Add(id, new PixelFontCharacter(
                    id, texture, packX[i], packY[i], width[i], height[i], offsetX[i],
                    offsetY[i], advance[i]
                ));
            }

            // get kerning
            var kerningCount = data["kerning_count"].InnerInt();
            var kernings = Utils.ReadCSVInt(data["kerning"].InnerText);
            for (var i = 0; i < kerningCount; i++)
            {
                var from = kernings[i * 3 + 0];
                var to = kernings[i * 3 + 1];
                var push = kernings[i * 3 + 2];

                if (fontSize.Characters.TryGetValue(from, out var c)) c.Kerning.Add(to, push);
            }

            // add font size
            Sizes.Add(fontSize);
            Sizes.Sort((a, b) => { return Mathf.Sign(a.Size - b.Size); });

            return fontSize;
        }

        public PixelFontSize Get(float size)
        {
            for (int i = 0, j = Sizes.Count - 1; i < j; i++)
                if (Sizes[i].Size >= size)
                    return Sizes[i];

            return Sizes[Sizes.Count - 1];
        }

        public void Draw(float baseSize, char character, Vector2 position, Vector2 justify, Vector2 scale, Color color)
        {
            var fontSize = Get(baseSize * Mathf.Max(scale.X, scale.Y));
            scale *= baseSize / fontSize.Size;
            fontSize.Draw(character, position, justify, scale, color);
        }

        public void Draw(
            float baseSize,
            string text,
            Vector2 position,
            Vector2 justify,
            Vector2 scale,
            Color color,
            float edgeDepth,
            Color edgeColor,
            float stroke,
            Color strokeColor
        )
        {
            var fontSize = Get(baseSize * Mathf.Max(scale.X, scale.Y));
            scale *= baseSize / fontSize.Size;
            fontSize.Draw(text, position, justify, scale, color, edgeDepth, edgeColor, stroke, strokeColor);
        }

        public void Draw(float baseSize, string text, Vector2 position, Color color)
        {
            var scale = Vector2.One;
            var fontSize = Get(baseSize * Mathf.Max(scale.X, scale.Y));
            scale *= baseSize / fontSize.Size;
            fontSize.Draw(text, position, Vector2.Zero, scale, color, 0, Color.Transparent, 0, Color.Transparent);
        }

        public void Draw(float baseSize, string text, Vector2 position, Vector2 justify, Vector2 scale, Color color)
        {
            var fontSize = Get(baseSize * Mathf.Max(scale.X, scale.Y));
            scale *= baseSize / fontSize.Size;
            fontSize.Draw(text, position, justify, scale, color, 0, Color.Transparent, 0, Color.Transparent);
        }

        public void DrawOutline(
            float baseSize,
            string text,
            Vector2 position,
            Vector2 justify,
            Vector2 scale,
            Color color,
            float stroke,
            Color strokeColor
        )
        {
            var fontSize = Get(baseSize * Mathf.Max(scale.X, scale.Y));
            scale *= baseSize / fontSize.Size;
            fontSize.Draw(text, position, justify, scale, color, 0f, Color.Transparent, stroke, strokeColor);
        }

        public void DrawEdgeOutline(
            float baseSize,
            string text,
            Vector2 position,
            Vector2 justify,
            Vector2 scale,
            Color color,
            float edgeDepth,
            Color edgeColor,
            float stroke = 0f,
            Color strokeColor = default
        )
        {
            var fontSize = Get(baseSize * Mathf.Max(scale.X, scale.Y));
            scale *= baseSize / fontSize.Size;
            fontSize.Draw(text, position, justify, scale, color, edgeDepth, edgeColor, stroke, strokeColor);
        }

        public Vector2 MeasureString(float baseSize, string text)
        {
            var scale = Vector2.One;
            var fontSize = Get(baseSize * Mathf.Max(scale.X, scale.Y));
            scale *= (baseSize / fontSize.Size);
            return fontSize.Measure(text) * scale;
        }

        public Vector2 MeasureChar(float baseSize, char ch)
        {
            var scale    = Vector2.One;
            var fontSize = Get(baseSize * Mathf.Max(scale.X, scale.Y));
            scale *= (baseSize / fontSize.Size);
            return fontSize.Measure(ch) * scale;
        }

        public void Dispose()
        {
            foreach (var tex in Textures) tex.Dispose();

            Sizes.Clear();
        }
    }
}
