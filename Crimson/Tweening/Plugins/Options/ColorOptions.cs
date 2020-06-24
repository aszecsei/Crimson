namespace Crimson.Tweening.Plugins.Options
{
    public struct ColorOptions : IPlugOptions
    {
        public bool AlphaOnly;

        public void Reset()
        {
            AlphaOnly = false;
        }
    }
}