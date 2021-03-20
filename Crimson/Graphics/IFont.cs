using Microsoft.Xna.Framework;

namespace Crimson
{
    public interface IFont
    {
        public void Draw(float baseSize, char character, Vector2 position, Vector2 justify, Vector2 scale, Color color);

        public void Draw(
            float   baseSize,
            string  text,
            Vector2 position,
            Vector2 justify,
            Vector2 scale,
            Color   color,
            float   edgeDepth,
            Color   edgeColor,
            float   stroke,
            Color   strokeColor
        );

        public void Draw(float baseSize, string text, Vector2 position, Color color);

        public void Draw(float baseSize, string text, Vector2 position, Vector2 justify, Vector2 scale, Color color);

        public void DrawOutline(
            float   baseSize,
            string  text,
            Vector2 position,
            Vector2 justify,
            Vector2 scale,
            Color   color,
            float   stroke,
            Color   strokeColor
        );

        public void DrawEdgeOutline(
            float   baseSize,
            string  text,
            Vector2 position,
            Vector2 justify,
            Vector2 scale,
            Color   color,
            float   edgeDepth,
            Color   edgeColor,
            float   stroke      = 0f,
            Color   strokeColor = default
        );

        public Vector2 MeasureString(float baseSize, string text);

        public Vector2 MeasureChar(float baseSize, char ch);
    }
}
