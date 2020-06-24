namespace Crimson.Tweening.Plugins.Options
{
    public struct FRectOptions : IPlugOptions
    {
        public bool Snapping;

        public void Reset()
        {
            Snapping = false;
        }
    }
}