namespace Crimson.Tweening.Plugins.Options
{
    public struct StringOptions : IPlugOptions
    {
        public ScrambleMode ScrambleMode;
        public char[]? ScrambledChars;
        

        public void Reset()
        {
            ScrambleMode = ScrambleMode.None;
            ScrambledChars = null;
        }
    }
}