using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Crimson.Tweening.Plugins.Options;

namespace Crimson.Tweening.Plugins
{
    public class StringPlugin : ITweenPlugin<string, StringOptions>
    {
        private static readonly StringBuilder Buffer = new StringBuilder();
        private static readonly List<Char> OpenedTags = new List<char>();
        
        public void Reset(TweenCore<string, StringOptions> t)
        {
            t.StartValue = t.EndValue = "";
        }

        public void EvaluateAndApply(StringOptions options, Animation t, Getter<string> getter, Setter<string> setter, float elapsed, float duration,
            string startValue, string endValue)
        {
            Buffer.Remove(0, Buffer.Length);

            int startValueLen = startValue.Length;
            int endValueLen = endValue.Length;
            int len = Mathf.RoundToInt(Mathf.Lerp(startValueLen, endValueLen, t.Easer.Invoke(elapsed / duration)));
            len = Mathf.Clamp(len, 0, endValueLen);

            if (options.ScrambleMode != ScrambleMode.None)
            {
                setter.Invoke(Append(endValue, 0, len).AppendScrambledChars(endValueLen - len, ScrambledCharsToUse(options)).ToString());
                return;
            }

            int diff = startValueLen - endValueLen;
            int startValueMaxLen = startValueLen;
            if (diff > 0)
            {
                // String to be replaced is longer than endValue: remove parts of it while tweening
                float perc = (float) len / endValueLen;
                startValueMaxLen -= (int) (startValueLen * perc);
            }
            else
            {
                startValueMaxLen -= len;
            }

            Append(endValue, 0, len);
            if (len < endValueLen && len < startValueLen)
            {
                Append(startValue, len, startValueMaxLen);
                setter(Buffer.ToString());
            }
        }

        private StringBuilder Append(string value, int startIndex, int length)
        {
            Buffer.Append(value, startIndex, length);
            return Buffer;
        }

        private char[] ScrambledCharsToUse(StringOptions options)
        {
            switch (options.ScrambleMode)
            {
                case ScrambleMode.Uppercase:
                    return StringPluginExtensions.ScrambledCharsUppercase;
                case ScrambleMode.Lowercase:
                    return StringPluginExtensions.ScrambledCharsLowercase;
                case ScrambleMode.Numerals:
                    return StringPluginExtensions.ScrambledCharsNumerals;
                case ScrambleMode.Custom:
                    return options.ScrambledChars!;
                default:
                    return StringPluginExtensions.ScrambledCharsAll;
            }
        }
    }

    internal static class StringPluginExtensions
    {
        public static readonly char[] ScrambledCharsAll = new[] {
            'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','X','Y','Z',
            'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','x','y','z',
            '1','2','3','4','5','6','7','8','9','0'
        };
        public static readonly char[] ScrambledCharsUppercase = new[] {
            'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','X','Y','Z'
        };
        public static readonly char[] ScrambledCharsLowercase = new[] {
            'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','x','y','z'
        };
        public static readonly char[] ScrambledCharsNumerals = new[] {
            '1','2','3','4','5','6','7','8','9','0'
        };
        static int s_lastRndSeed;

        static StringPluginExtensions()
        {
            ScrambledCharsAll.ScrambleChars();
            ScrambledCharsUppercase.ScrambleChars();
            ScrambledCharsLowercase.ScrambleChars();
            ScrambledCharsNumerals.ScrambleChars();
        }

        internal static void ScrambleChars(this char[] chars)
        {
            // Shuffle chars (uses Knuth shuggle algorithm)
            int len = chars.Length;
            for (int i = 0; i < len; i++) {
                char tmp = chars[i];
                int r = Utils.Random.Range(i, len);
                chars[i] = chars[r];
                chars[r] = tmp;
            }
        }

        internal static StringBuilder AppendScrambledChars(this StringBuilder buffer, int length, char[] chars)
        {
            if (length <= 0) return buffer;

            // Make sure random seed is different from previous one used
            int len = chars.Length;
            int rndSeed = s_lastRndSeed;
            while (rndSeed == s_lastRndSeed) {
                rndSeed = Utils.Random.Range(0, len);
            }
            s_lastRndSeed = rndSeed;
            // Append
            for (int i = 0; i < length; ++i) {
                if (rndSeed >= len) rndSeed = 0;
                buffer.Append(chars[rndSeed]);
                rndSeed += 1;
            }
            return buffer;
        }
    }
}