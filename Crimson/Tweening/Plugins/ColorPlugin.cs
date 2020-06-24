using Crimson.Tweening.Plugins.Options;
using Microsoft.Xna.Framework;

namespace Crimson.Tweening.Plugins
{
    public class ColorPlugin : ITweenPlugin<Color, ColorOptions>
    {
        public void Reset(TweenCore<Color, ColorOptions> t) { }

        public void EvaluateAndApply(ColorOptions options, Animation t, Getter<Color> getter, Setter<Color> setter, float elapsed, float duration,
            Color startValue, Color endValue)
        {
            float easeVal = t.Easer.Invoke(elapsed / duration);
            if (!options.AlphaOnly)
            {
                startValue.R = (byte)Mathf.Lerp(startValue.R, endValue.R, easeVal);
                startValue.G = (byte)Mathf.Lerp(startValue.G, endValue.G, easeVal);
                startValue.B = (byte)Mathf.Lerp(startValue.B, endValue.B, easeVal);
                startValue.A = (byte)Mathf.Lerp(startValue.A, endValue.A, easeVal);
                setter.Invoke(startValue);
                return;
            }
            
            // Alpha only
            Color res = getter();
            res.A = (byte)Mathf.Lerp(startValue.A, endValue.A, easeVal);
            setter.Invoke(res);
        }
    }
}