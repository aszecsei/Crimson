using Crimson.Tweening.Plugins.Options;

namespace Crimson.Tweening.Plugins
{
    public class LongPlugin : ITweenPlugin<long, NoOptions>
    {
        public void Reset(TweenCore<long, NoOptions> t)
        {
            
        }

        public void EvaluateAndApply(NoOptions options, Animation t, Getter<long> getter, Setter<long> setter, float elapsed, float duration,
            long startValue, long endValue)
        {
            setter.Invoke((long)Mathf.Round(Mathf.Lerp(startValue, endValue, t.Easer.Invoke(elapsed / duration))));
        }
    }
}