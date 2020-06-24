using Crimson.Tweening.Plugins.Options;
using Microsoft.Xna.Framework;

namespace Crimson.Tweening.Plugins
{
    public class RectPlugin : ITweenPlugin<Rectangle, NoOptions>
    {
        public void Reset(TweenCore<Rectangle, NoOptions> t)
        {
            
        }

        public void EvaluateAndApply(NoOptions options, Animation t, Getter<Rectangle> getter, Setter<Rectangle> setter, float elapsed, float duration,
            Rectangle startValue, Rectangle endValue)
        {
            float easeVal = t.Easer.Invoke(elapsed / duration);
            startValue.X = Mathf.RoundToInt(Mathf.Lerp(startValue.X, endValue.X, easeVal));
            startValue.Y = Mathf.RoundToInt(Mathf.Lerp(startValue.Y, endValue.Y, easeVal));
            startValue.Width = Mathf.RoundToInt(Mathf.Lerp(startValue.Width, endValue.Width, easeVal));
            startValue.Height = Mathf.RoundToInt(Mathf.Lerp(startValue.Height, endValue.Height, easeVal));
            
            setter.Invoke(startValue);
        }
    }
}