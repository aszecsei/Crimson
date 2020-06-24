using Microsoft.Xna.Framework;

namespace Crimson.Tweening.Plugins.Options
{
    public struct QuaternionOptions : IPlugOptions
    {
        public RotateMode RotateMode;

        public void Reset()
        {
            RotateMode = RotateMode.Fast;
        }
    }
}