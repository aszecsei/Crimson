using Crimson.Tweening.Plugins.Options;

namespace Crimson.Tweening.Plugins
{
    public class FRectPlugin : ITweenPlugin<FRect, FRectOptions>
    {
        public void Reset(TweenCore<FRect, FRectOptions> t)
        {
            
        }

        public void EvaluateAndApply(FRectOptions options, Animation t, Getter<FRect> getter, Setter<FRect> setter, float elapsed, float duration,
            FRect startValue, FRect endValue)
        {
            float easeVal = t.Easer.Invoke(elapsed / duration);
            startValue.X = Mathf.Lerp(startValue.X, endValue.X, easeVal);
            startValue.Y = Mathf.Lerp(startValue.Y, endValue.Y, easeVal);
            startValue.Width = Mathf.Lerp(startValue.Width, endValue.Width, easeVal);
            startValue.Height = Mathf.Lerp(startValue.Height, endValue.Height, easeVal);

            if (options.Snapping)
            {
                startValue.X = Mathf.Round(startValue.X);
                startValue.Y = Mathf.Round(startValue.Y);
                startValue.Width = Mathf.Round(startValue.Width);
                startValue.Height = Mathf.Round(startValue.Height);
            }
            
            setter.Invoke(startValue);
        }
    }
}