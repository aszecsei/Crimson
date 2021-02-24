using System;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

namespace Crimson
{
    public static class ColorExt
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color Invert(this Color color)
        {
            return new Color(255 - color.R, 255 - color.G, 255 - color.B, color.A);
        }

        public static Color HexToColor(string hex)
        {
            if (hex.Length >= 6)
            {
                var r = (Mathf.HexToByte(hex[0]) * 16 + Mathf.HexToByte(hex[1])) / 255.0f;
                var g = (Mathf.HexToByte(hex[2]) * 16 + Mathf.HexToByte(hex[3])) / 255.0f;
                var b = (Mathf.HexToByte(hex[4]) * 16 + Mathf.HexToByte(hex[5])) / 255.0f;
                return new Color(r, g, b);
            }

            return Color.White;
        }

        public static string ColorToHex(this Color color)
        {
            return $"{color.R:X2}{color.G:X2}{color.B:X2}";
        }

        public static Color HexToColor(int hex)
        {
            var r = (byte) (hex >> 16);
            var g = (byte) (hex >> 8);
            var b = (byte) (hex >> 0);
            return new Color(r, g, b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color Grayscale(this Color color)
        {
            int val = (int) (color.R * 0.3 + color.G * 0.59 + color.B * 0.11);
            return new Color(val, val, val, color.A);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color Add(this Color color, Color other)
        {
            return new Color(color.R + other.R, color.G + other.G, color.B + other.B, color.A + other.A);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color Subtract(this Color color, Color other)
        {
            return new Color(color.R - other.R, color.G - other.G, color.B - other.B, color.A - other.A);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color Multiply(this Color color, Color other)
        {
            return new Color
            {
                R = (byte)(color.R * other.R / 255),
                G = (byte)(color.G * other.G / 255),
                B = (byte)(color.B * other.B / 255),
                A = (byte)(color.A * other.A / 255)
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color Lerp(Color from, Color to, float t)
        {
            return new Color(Mathf.Lerp(from.R, to.R, t),
                Mathf.Lerp(from.G, to.G, t), Mathf.Lerp(from.B, to.B, t), Mathf.Lerp(from.A, to.A, t));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Lerp(Color from, Color to, out Color result, float t)
        {
            result = new Color(Mathf.Lerp(from.R, to.R, t),
                Mathf.Lerp(from.G, to.G, t), Mathf.Lerp(from.B, to.B, t), Mathf.Lerp(from.A, to.A, t));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Numerics.Vector4 ToVector4SysNum(this Color color)
        {
            return new System.Numerics.Vector4(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);
        }

        public static Color ColorFromHSV(float hue, float saturation, float value)
        {
            int    hi = Mathf.FloorToInt(hue / 60) % 6;
            float  f  = hue / 60 - Mathf.Floor(hue / 60);

            value = value * 255;
            int v = Mathf.RoundToInt(value);
            int p = Mathf.RoundToInt(value * (1 - saturation));
            int q = Mathf.RoundToInt(value * (1 - f       * saturation));
            int t = Mathf.RoundToInt(value * (1 - (1 - f) * saturation));

            if (hi == 0)
                return new Color(v, t, p, 255);
            else if (hi == 1)
                return new Color(q, v, p, 255);
            else if (hi == 2)
                return new Color(p, v, t, 255);
            else if (hi == 3)
                return new Color(p, q, v, 255);
            else if (hi == 4)
                return new Color(t, p, v, 255);
            else
                return new Color(v, p, q, 255);
        }
    }
}
