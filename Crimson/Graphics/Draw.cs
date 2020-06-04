#region Using Statements

using Crimson.Spatial;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Crimson
{
    public static class Draw
    {
        /// <summary>
        ///     A subtexture used to draw particle systems.
        ///     Will be generated at startup, but you can replace this with a subtexture from your texture atlas to reduce texture
        ///     swaps.
        ///     Should be a 2x2 white pixel.
        /// </summary>
        public static CTexture Particle;

        /// <summary>
        ///     A subtexture used to draw particle systems.
        ///     Will be generated at startup, but you can replace this with a subtexture from your texture atlas to reduce texture
        ///     swaps.
        ///     Use the top left pixel of your particle subtexture if you replace it!
        ///     Should be a 1x1 white pixel.
        /// </summary>
        public static CTexture Pixel;

        private static Rectangle s_rect;

        /// <summary>
        ///     The currently-active renderer.
        /// </summary>
        public static Renderer Renderer { get; internal set; }

        /// <summary>
        ///     All 2D rendering is done through this SpriteBatch instance
        /// </summary>
        public static SpriteBatch SpriteBatch { get; private set; }

        /// <summary>
        ///     The default Crimson font (Consolas 12). Loaded automatically at startup.
        /// </summary>
        public static SpriteFont DefaultFont { get; private set; }

        internal static void Initialize(GraphicsDevice graphicsDevice)
        {
            SpriteBatch = new SpriteBatch(graphicsDevice);
            DefaultFont = Engine.Instance.Content.Load<SpriteFont>(@"Crimson\CrimsonDefault");
            UseDebugPixelTexture();
        }

        public static void UseDebugPixelTexture()
        {
            var texture = new CTexture(2, 2, Color.White);
            Pixel = new CTexture(texture, 0, 0, 1, 1);
            Particle = new CTexture(texture, 0, 0, 2, 2);
        }

        public static void Point(Vector2 at, Color color)
        {
            SpriteBatch.Draw(Pixel.Texture, at, Pixel.ClipRect, color, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
        }

        #region Line

        public static void Line(Vector2 start, Vector2 end, Color color)
        {
            LineAngle(start, Mathf.Angle(start, end), Vector2.Distance(start, end), color);
        }

        public static void Line(Vector2 start, Vector2 end, Color color, float thickness)
        {
            LineAngle(start, Mathf.Angle(start, end), Vector2.Distance(start, end), color, thickness);
        }

        public static void Line(float x1, float y1, float x2, float y2, Color color)
        {
            Line(new Vector2(x1, y1), new Vector2(x2, y2), color);
        }

        #endregion

        #region Line Angle

        public static void LineAngle(Vector2 start, float angle, float length, Color color)
        {
            SpriteBatch.Draw(
                Pixel.Texture, start, Pixel.ClipRect, color, angle, Vector2.Zero, new Vector2(length, 1),
                SpriteEffects.None, 0);
        }

        public static void LineAngle(Vector2 start, float angle, float length, Color color, float thickness)
        {
            SpriteBatch.Draw(Pixel.Texture, start, Pixel.ClipRect, color, angle, new Vector2(0, .5f),
                new Vector2(length, thickness),
                SpriteEffects.None, 0);
        }

        public static void LineAngle(float startX, float startY, float angle, float length, Color color)
        {
            LineAngle(new Vector2(startX, startY), angle, length, color);
        }

        #endregion

        #region Circle

        public static void Circle(Vector2 position, float radius, Color color, int resolution)
        {
            var last = Vector2.UnitX * radius;
            var lastP = last.Perpendicular();
            for (var i = 1; i <= resolution; i++)
            {
                var at = Mathf.AngleToVector(i * MathHelper.PiOver2 / resolution, radius);
                var atP = at.Perpendicular();

                Line(position + last, position + at, color);
                Line(position - last, position - at, color);
                Line(position + lastP, position + atP, color);
                Line(position - lastP, position - atP, color);

                last = at;
                lastP = atP;
            }
        }

        public static void Circle(float x, float y, float radius, Color color, int resolution)
        {
            Circle(new Vector2(x, y), radius, color, resolution);
        }

        public static void Circle(Vector2 position, float radius, Color color, float thickness, int resolution)
        {
            var last = Vector2.UnitX * radius;
            var lastP = last.Perpendicular();
            for (var i = 1; i <= resolution; i++)
            {
                var at = Mathf.AngleToVector(i * MathHelper.PiOver2 / resolution, radius);
                var atP = at.Perpendicular();

                Line(position + last, position + at, color, thickness);
                Line(position - last, position - at, color, thickness);
                Line(position + lastP, position + atP, color, thickness);
                Line(position - lastP, position - atP, color, thickness);

                last = at;
                lastP = atP;
            }
        }

        public static void Circle(float x, float y, float radius, Color color, float thickness, int resolution)
        {
            Circle(new Vector2(x, y), radius, color, thickness, resolution);
        }

        #endregion

        #region Rect

        public static void Rect(float x, float y, float width, float height, Color color)
        {
            s_rect.X = (int) x;
            s_rect.Y = (int) y;
            s_rect.Width = (int) width;
            s_rect.Height = (int) height;
            SpriteBatch.Draw(Pixel.Texture, s_rect, Pixel.ClipRect, color);
        }

        public static void Rect(Vector2 position, float width, float height, Color color)
        {
            Rect(position.X, position.Y, width, height, color);
        }

        public static void Rect(Rectangle rect, Color color)
        {
            s_rect = rect;
            SpriteBatch.Draw(Pixel.Texture, rect, Pixel.ClipRect, color);
        }

        #endregion

        #region Hollow Rect

        public static void HollowRect(float x, float y, float width, float height, Color color)
        {
            s_rect.X = (int) x;
            s_rect.Y = (int) y;
            s_rect.Width = (int) width;
            s_rect.Height = 1;

            SpriteBatch.Draw(Pixel.Texture, s_rect, Pixel.ClipRect, color);

            s_rect.Y += (int) height - 1;

            SpriteBatch.Draw(Pixel.Texture, s_rect, Pixel.ClipRect, color);

            s_rect.Y -= (int) height - 1;
            s_rect.Width = 1;
            s_rect.Height = (int) height;

            SpriteBatch.Draw(Pixel.Texture, s_rect, Pixel.ClipRect, color);

            s_rect.X += (int) width - 1;

            SpriteBatch.Draw(Pixel.Texture, s_rect, Pixel.ClipRect, color);
        }

        public static void HollowRect(Vector2 position, float width, float height, Color color)
        {
            HollowRect(position.X, position.Y, width, height, color);
        }

        public static void HollowRect(Rectangle rect, Color color)
        {
            HollowRect(rect.X, rect.Y, rect.Width, rect.Height, color);
        }

        #endregion

        #region Text

        public static void Text(SpriteFont font, string text, Vector2 position, Color color)
        {
            SpriteBatch.DrawString(font, text, VectorExt.Floor(position), color);
        }

        public static void Text(
            SpriteFont font,
            string text,
            Vector2 position,
            Color color,
            Vector2 origin,
            Vector2 scale,
            float rotation
        )
        {
            SpriteBatch.DrawString(font, text, VectorExt.Floor(position), color, rotation, origin, scale, SpriteEffects.None,
                0);
        }

        public static void TextJustified(SpriteFont font, string text, Vector2 position, Color color, Vector2 justify)
        {
            var origin = font.MeasureString(text);
            origin.X *= justify.X;
            origin.Y *= justify.Y;

            SpriteBatch.DrawString(font, text, VectorExt.Floor(position), color, 0, origin, 1, SpriteEffects.None, 0);
        }

        public static void TextJustified(
            SpriteFont font,
            string text,
            Vector2 position,
            Color color,
            float scale,
            Vector2 justify
        )
        {
            var origin = font.MeasureString(text);
            origin.X *= justify.X;
            origin.Y *= justify.Y;
            SpriteBatch.DrawString(font, text, VectorExt.Floor(position), color, 0, origin, scale, SpriteEffects.None, 0);
        }

        public static void TextCentered(SpriteFont font, string text, Vector2 position)
        {
            Text(font, text, position - font.MeasureString(text) * .5f, Color.White);
        }

        public static void TextCentered(SpriteFont font, string text, Vector2 position, Color color)
        {
            Text(font, text, position - font.MeasureString(text) * .5f, color);
        }

        public static void TextCentered(SpriteFont font, string text, Vector2 position, Color color, float scale)
        {
            Text(font, text, position, color, font.MeasureString(text) * .5f, Vector2.One * scale, 0);
        }

        public static void TextCentered(
            SpriteFont font,
            string text,
            Vector2 position,
            Color color,
            float scale,
            float rotation
        )
        {
            Text(font, text, position, color, font.MeasureString(text) * .5f, Vector2.One * scale, rotation);
        }

        public static void OutlineTextCentered(SpriteFont font, string text, Vector2 position, Color color, float scale)
        {
            var origin = font.MeasureString(text) / 2;

            for (var i = -1; i < 2; i++)
            for (var j = -1; j < 2; j++)
                if (i != 0 || j != 0)
                    SpriteBatch.DrawString(font, text, VectorExt.Floor(position) + new Vector2(i, j), Color.Black, 0, origin,
                        scale, SpriteEffects.None, 0);

            SpriteBatch.DrawString(font, text, VectorExt.Floor(position), color, 0, origin, scale, SpriteEffects.None, 0);
        }

        public static void OutlineTextCentered(
            SpriteFont font,
            string text,
            Vector2 position,
            Color color,
            Color outlineColor
        )
        {
            var origin = font.MeasureString(text) / 2;

            for (var i = -1; i < 2; i++)
            for (var j = -1; j < 2; j++)
                if (i != 0 || j != 0)
                    SpriteBatch.DrawString(font, text, VectorExt.Floor(position) + new Vector2(i, j), outlineColor, 0,
                        origin, 1, SpriteEffects.None, 0);

            SpriteBatch.DrawString(font, text, VectorExt.Floor(position), color, 0, origin, 1, SpriteEffects.None, 0);
        }

        public static void OutlineTextCentered(
            SpriteFont font,
            string text,
            Vector2 position,
            Color color,
            Color outlineColor,
            float scale
        )
        {
            var origin = font.MeasureString(text) / 2;

            for (var i = -1; i < 2; i++)
            for (var j = -1; j < 2; j++)
                if (i != 0 || j != 0)
                    SpriteBatch.DrawString(font, text, VectorExt.Floor(position) + new Vector2(i, j), outlineColor, 0,
                        origin, scale, SpriteEffects.None, 0);

            SpriteBatch.DrawString(font, text, VectorExt.Floor(position), color, 0, origin, scale, SpriteEffects.None, 0);
        }

        public static void OutlineTextJustify(
            SpriteFont font,
            string text,
            Vector2 position,
            Color color,
            Color outlineColor,
            Vector2 justify
        )
        {
            var origin = font.MeasureString(text) * justify;

            for (var i = -1; i < 2; i++)
            for (var j = -1; j < 2; j++)
                if (i != 0 || j != 0)
                    SpriteBatch.DrawString(font, text, VectorExt.Floor(position) + new Vector2(i, j), outlineColor, 0,
                        origin, 1, SpriteEffects.None, 0);

            SpriteBatch.DrawString(font, text, VectorExt.Floor(position), color, 0, origin, 1, SpriteEffects.None, 0);
        }

        public static void OutlineTextJustify(
            SpriteFont font,
            string text,
            Vector2 position,
            Color color,
            Color outlineColor,
            Vector2 justify,
            float scale
        )
        {
            var origin = font.MeasureString(text) * justify;

            for (var i = -1; i < 2; i++)
            for (var j = -1; j < 2; j++)
                if (i != 0 || j != 0)
                    SpriteBatch.DrawString(font, text, VectorExt.Floor(position) + new Vector2(i, j), outlineColor, 0,
                        origin, scale, SpriteEffects.None, 0);

            SpriteBatch.DrawString(font, text, VectorExt.Floor(position), color, 0, origin, scale, SpriteEffects.None, 0);
        }

        #endregion
    }
}