using System;
using Microsoft.Xna.Framework;

namespace Crimson.UI
{
    [Flags]
    public enum Style
    {
        None = 0,
        Bold = 1 << 0,
        Italic = 1 << 1,
        Outline = 1 << 2,
    }
    public class Letter
    {
        public Vector2 TransformedPosition = Vector2.Zero;
        public Vector2 TransformedScale = Vector2.One;
        public float TimeExisted = 0f;
        
        public char Char;

        public Style Style;

        public Letter(char ch = ' ')
        {
            Char = ch;
        }
    }
}