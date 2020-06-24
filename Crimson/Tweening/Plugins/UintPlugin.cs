using Crimson.Tweening.Plugins.Options;

namespace Crimson.Tweening.Plugins
{
    public class UintPlugin : ITweenPlugin<uint, NoOptions>
    {
        public void Reset(TweenCore<uint, NoOptions> t)
        {
            
        }

        public void EvaluateAndApply(NoOptions options, Animation t, Getter<uint> getter, Setter<uint> setter, float elapsed, float duration,
            uint startValue, uint endValue)
        {
            setter.Invoke((uint) Mathf.Round(Mathf.Lerp(startValue, endValue, t.Easer.Invoke(elapsed / duration))));
        }
    }
}