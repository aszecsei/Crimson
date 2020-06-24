namespace Crimson.Tweening.Plugins.Options
{
    public struct FloatOptions : IPlugOptions
    {
        public bool Snapping;

        public void Reset()
        {
            Snapping = false;
        }
    }
}