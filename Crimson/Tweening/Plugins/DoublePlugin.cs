using Crimson.Tweening.Plugins.Options;

namespace Crimson.Tweening.Plugins
{
    public class DoublePlugin : ITweenPlugin<double, NoOptions>
    {
        public void Reset(TweenCore<double, NoOptions> t)
        {
            
        }

        public void EvaluateAndApply(NoOptions options, Animation t, Getter<double> getter, Setter<double> setter, float elapsed, float duration,
            double startValue, double endValue)
        {
            setter.Invoke(Mathf.Lerp(startValue, endValue, t.Easer.Invoke(elapsed / duration)));
        }
    }
}