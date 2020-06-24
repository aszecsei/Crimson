namespace Crimson.Tweening.Plugins.Options
{
    public struct VectorOptions : IPlugOptions
    {
        public AxisConstraint AxisConstraint;
        public bool Snapping;

        public void Reset()
        {
            AxisConstraint = AxisConstraint.All;
            Snapping = false;
        }
    }
}