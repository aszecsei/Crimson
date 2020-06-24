using Crimson.Tweening.Plugins.Options;

namespace Crimson.Tweening.Plugins
{
    public class IntPlugin : ITweenPlugin<int, NoOptions>
    {
        public void Reset(TweenCore<int, NoOptions> t)
        {
            
        }

        public void EvaluateAndApply(NoOptions options, Animation t, Getter<int> getter, Setter<int> setter, float elapsed, float duration,
            int startValue, int endValue)
        {
            setter.Invoke(Mathf.RoundToInt(Mathf.Lerp(startValue, endValue, t.Easer.Invoke(elapsed / duration))));
        }
    }
}