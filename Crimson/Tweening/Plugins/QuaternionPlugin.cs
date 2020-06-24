using Crimson.Tweening.Plugins.Options;
using Microsoft.Xna.Framework;

namespace Crimson.Tweening.Plugins
{
    public class QuaternionPlugin : ITweenPlugin<Quaternion, QuaternionOptions>
    {
        public void Reset(TweenCore<Quaternion, QuaternionOptions> t)
        {
            
        }

        public void EvaluateAndApply(QuaternionOptions options, Animation t, Getter<Quaternion> getter, Setter<Quaternion> setter, float elapsed,
            float duration, Quaternion startValue, Quaternion endValue)
        {
            float easeVal = t.Easer.Invoke(elapsed / duration);
            switch (options.RotateMode)
            {
                case RotateMode.Fast:
                    setter(Quaternion.Lerp(startValue, endValue, easeVal));
                    break;
                case RotateMode.Precise:
                    setter(Quaternion.Slerp(startValue, endValue, easeVal));
                    break;
            }
        }
    }
}