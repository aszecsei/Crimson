using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Text.RegularExpressions;
using System.IO;

namespace Crimson
{
    public class BmFontCharacter
    {
        public int Character;
        public readonly CTexture Texture;
        public readonly int XOffset;
        public readonly int YOffset;
        public readonly int XAdvance;
        public readonly Dictionary<int, int> Kerning = new Dictionary<int, int>();

        public BmFontCharacter(int character, CTexture texture, XmlElement xml)
        {
            Character = character;
            Texture = texture.GetSubtexture(xml.AttrInt("x"), xml.AttrInt("y"), xml.AttrInt("width"), xml.AttrInt("height"));
            XOffset = xml.AttrInt("xoffset");
            YOffset = xml.AttrInt("yoffset");
            XAdvance = xml.AttrInt("xadvance");
        }
    }

    public class BmFontSize
    {
        public List<CTexture> Textures = new List<CTexture>();
        public Dictionary<int, BmFontCharacter> Characters = new Dictionary<int, BmFontCharacter>();
        public int LineHeight;
        public float Size;
        public bool Outline;

        private readonly StringBuilder _temp = new StringBuilder();

        public string AutoNewline(string text, int width)
        {
            if (string.IsNullOrEmpty(text))
                return text;

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

                    if (word.Equals(" "))
                        continue;
                }

                // this word is longer than the max-width, split where ever we can
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

        public BmFontCharacter? Get(int id)
        {
            return Characters.TryGetValue(id, out var val) ? val : null;
        }

        public Vector2 Measure(char text)
        {
            return Characters.TryGetValue(text, out var c) ? new Vector2(c.XAdvance, LineHeight) : Vector2.Zero;
        }

        public Vector2 Measure(string text)
        {
            if (string.IsNullOrEmpty(text))
                return Vector2.Zero;

            var size = new Vector2(0, LineHeight);
            var currentLineWidth = 0f;

            for (var i = 0; i < text.Length; i++)
            {
                if (text[i] == '\n')
                {
                    size.Y += LineHeight;
                    if (currentLineWidth > size.X)
                        size.X = currentLineWidth;
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
            }

            if (currentLineWidth > size.X)
                size.X = currentLineWidth;

            return size;
        }

        public float WidthToNextLine(string text, int start)
        {
            if (string.IsNullOrEmpty(text))
                return 0;

            var currentLineWidth = 0f;

            for (int i = start, j = text.Length; i < j; i++)
            {
                if (text[i] == '\n')
                    break;

                if (Characters.TryGetValue(text[i], out var c))
                {
                    currentLineWidth += c.XAdvance;

                    if (i < j - 1 && c.Kerning.TryGetValue(text[i + 1], out var kerning))
                        currentLineWidth += kerning;
                }
            }

            return currentLineWidth;
        }

        public float HeightOf(string text)
        {
            if (string.IsNullOrEmpty(text))
                return 0;

            var lines = 1;
            if (text.IndexOf('\n') >= 0)
                for (var i = 0; i < text.Length; i++)
                    if (text[i] == '\n')
                        lines++;
            return lines * LineHeight;
        }

        public void Draw(char character, Vector2 position, Vector2 justify, Vector2 scale, Color color)
        {
            if (char.IsWhiteSpace(character))
                return;

            if (Characters.TryGetValue(character, out var c))
            {
                var measure = Measure(character);
                var justified = new Vector2(measure.X * justify.X, measure.Y * justify.Y);
                var pos = position + (new Vector2(c.XOffset, c.YOffset) - justified) * scale;
                c.Texture.Draw(Mathf.Floor(pos), Vector2.Zero, color, scale);
            }
        }

        public void Draw(string text, Vector2 position, Vector2 justify, Vector2 scale, Color color, float edgeDepth, Color edgeColor, float stroke, Color strokeColor)
        {
            if (string.IsNullOrEmpty(text))
                return;

            var offset = Vector2.Zero;
            var lineWidth = (!Mathf.Approximately(justify.X, 0) ? WidthToNextLine(text, 0) : 0);
            var justified = new Vector2(lineWidth * justify.X, HeightOf(text) * justify.Y);

            for (var i = 0; i < text.Length; i++)
            {
                if (text[i] == '\n')
                {
                    offset.X = 0;
                    offset.Y += LineHeight;
                    if (!Mathf.Approximately(justify.X, 0))
                        justified.X = WidthToNextLine(text, i + 1) * justify.X;
                    continue;
                }

                if (Characters.TryGetValue(text[i], out var c))
                {
                    var pos = (position + (offset + new Vector2(c.XOffset, c.YOffset) - justified) * scale);

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
                            c.Texture.Draw(pos + new Vector2(-stroke, edgeDepth + stroke), Vector2.Zero, strokeColor, scale);
                            c.Texture.Draw(pos + new Vector2(0, edgeDepth + stroke), Vector2.Zero, strokeColor, scale);
                            c.Texture.Draw(pos + new Vector2(stroke, edgeDepth + stroke), Vector2.Zero, strokeColor, scale);
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
                    if (edgeDepth > 0)
                        c.Texture.Draw(pos + Vector2.UnitY * edgeDepth, Vector2.Zero, edgeColor, scale);

                    // draw normal character
                    c.Texture.Draw(pos, Vector2.Zero, color, scale);

                    offset.X += c.XAdvance;

                    if (i < text.Length - 1 && c.Kerning.TryGetValue(text[i + 1], out var kerning))
                        offset.X += kerning;
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

        public void DrawOutline(string text, Vector2 position, Vector2 justify, Vector2 scale, Color color, float stroke, Color strokeColor)
        {
            Draw(text, position, justify, scale, color, 0f, Color.Transparent, stroke, strokeColor);
        }

        public void DrawEdgeOutline(string text, Vector2 position, Vector2 justify, Vector2 scale, Color color, float edgeDepth, Color edgeColor, float stroke = 0f, Color strokeColor = default(Color))
        {
            Draw(text, position, justify, scale, color, edgeDepth, edgeColor, stroke, strokeColor);
        }
    }

    /// <summary>
    /// A class to deal with rasterized fonts. This uses the data format provided by
    /// <a href="https://www.angelcode.com/products/bmfont/">AngelCode BMFont</a>.
    /// </summary>
    public class BmFont : IDisposable, IFont
    {
        public  string               Face;
        public  List<BmFontSize>     Sizes            = new List<BmFontSize>();
        private List<VirtualTexture> _managedTextures = new List<VirtualTexture>();

        public BmFont(string face)
        {
            Face = face;
        }

        public BmFontSize AddFontSize(string path, Atlas atlas = null, bool outline = false)
        {
            var data = Utils.LoadXML(path)["font"];
            return AddFontSize(path, data, atlas, outline);
        }

        public BmFontSize AddFontSize(string path, XmlElement data, Atlas atlas = null, bool outline = false)
        {
            // check if size already exists
            var size = data["info"].AttrFloat("size");
            foreach (var fs in Sizes)
                if (Mathf.Approximately(fs.Size, size))
                    return fs;

            // get textures
            List<CTexture> textures = new List<CTexture>();
            var pages = data["pages"];
            foreach (XmlElement page in pages)
            {
                var file = page.Attr("file");
                var atlasPath = Utils.NormalizePath(Path.GetFileNameWithoutExtension(file));

                if (atlas != null && atlas.Has(atlasPath))
                {
                    textures.Add(atlas[atlasPath]);
                    continue;
                }

                VirtualTexture tex = VirtualContent.CreateTexture(
                    Path.Combine(Path.GetDirectoryName(path).Substring(Engine.ContentDirectory.Length + 1), file));
                textures.Add(new CTexture(tex));
                _managedTextures.Add(tex);
            }

            // create font size
            var fontSize = new BmFontSize()
            {
                Textures = textures,
                Characters = new Dictionary<int, BmFontCharacter>(),
                LineHeight = data["common"].AttrInt("lineHeight"),
                Size = size,
                Outline = outline
            };

            // get characters
            foreach (XmlElement character in data["chars"])
            {
                var id = character.AttrInt("id");
                var page = character.AttrInt("page", 0);
                fontSize.Characters.Add(id, new BmFontCharacter(id, textures[page], character));
            }

            // get kerning
            if (data["kernings"] != null)
                foreach (XmlElement kerning in data["kernings"])
                {
                    var from = kerning.AttrInt("first");
                    var to = kerning.AttrInt("second");
                    var push = kerning.AttrInt("amount");

                    if (fontSize.Characters.TryGetValue(from, out var c))
                        c.Kerning.Add(to, push);
                }

            // add font size
            Sizes.Add(fontSize);
            Sizes.Sort((a, b) => Math.Sign(a.Size - b.Size));

            return fontSize;
        }

        public BmFontSize Get(float size)
        {
            for (int i = 0, j = Sizes.Count - 1; i < j; i++)
                if (Sizes[i].Size >= size)
                    return Sizes[i];
            return Sizes[Sizes.Count - 1];
        }

        public void Draw(float baseSize, char character, Vector2 position, Vector2 justify, Vector2 scale, Color color)
        {
            var fontSize = Get(baseSize * Math.Max(scale.X, scale.Y));
            scale *= (baseSize / fontSize.Size);
            fontSize.Draw(character, position, justify, scale, color);
        }

        public void Draw(float baseSize, string text, Vector2 position, Vector2 justify, Vector2 scale, Color color, float edgeDepth, Color edgeColor, float stroke, Color strokeColor)
        {
            var fontSize = Get(baseSize * Math.Max(scale.X, scale.Y));
            scale *= (baseSize / fontSize.Size);
            fontSize.Draw(text, position, justify, scale, color, edgeDepth, edgeColor, stroke, strokeColor);
        }

        public void Draw(float baseSize, string text, Vector2 position, Color color)
        {
            var scale = Vector2.One;
            var fontSize = Get(baseSize * Math.Max(scale.X, scale.Y));
            scale *= (baseSize / fontSize.Size);
            fontSize.Draw(text, position, Vector2.Zero, scale, color, 0, Color.Transparent, 0, Color.Transparent);
        }

        public void Draw(float baseSize, string text, Vector2 position, Vector2 justify, Vector2 scale, Color color)
        {
            var fontSize = Get(baseSize * Math.Max(scale.X, scale.Y));
            scale *= (baseSize / fontSize.Size);
            fontSize.Draw(text, position, justify, scale, color, 0, Color.Transparent, 0, Color.Transparent);
        }

        public void DrawOutline(float baseSize, string text, Vector2 position, Vector2 justify, Vector2 scale, Color color, float stroke, Color strokeColor)
        {
            var fontSize = Get(baseSize * Math.Max(scale.X, scale.Y));
            scale *= (baseSize / fontSize.Size);
            fontSize.Draw(text, position, justify, scale, color, 0f, Color.Transparent, stroke, strokeColor);
        }

        public void DrawEdgeOutline(float baseSize, string text, Vector2 position, Vector2 justify, Vector2 scale, Color color, float edgeDepth, Color edgeColor, float stroke = 0f, Color strokeColor = default(Color))
        {
            var fontSize = Get(baseSize * Math.Max(scale.X, scale.Y));
            scale *= (baseSize / fontSize.Size);
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
            var scale = Vector2.One;
            var fontSize = Get(baseSize * Mathf.Max(scale.X, scale.Y));
            scale *= (baseSize / fontSize.Size);
            return fontSize.Measure(ch) * scale;
        }

        public void Dispose()
        {
            foreach (VirtualTexture managedTexture in _managedTextures)
                managedTexture.Dispose();
            Sizes.Clear();
        }
    }
}
