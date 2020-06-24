using Crimson.Tweening.Plugins.Options;

namespace Crimson.Tweening.Plugins
{
    public class FloatPlugin : ITweenPlugin<float, FloatOptions>
    {
        public void Reset(TweenCore<float, FloatOptions> t)
        {
        }

        public void EvaluateAndApply(FloatOptions options, Animation t, Getter<float> getter, Setter<float> setter,
            float elapsed, float duration,
            float startValue, float endValue)
        {
            float res = Mathf.Lerp(startValue, endValue, t.Easer.Invoke(elapsed / duration));
            setter.Invoke(
                !options.Snapping
                    ? res
                    : Mathf.Round(res));
        }
    }
}