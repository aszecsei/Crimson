using Crimson.Tweening.Plugins.Options;

namespace Crimson.Tweening.Plugins
{
    public class UlongPlugin : ITweenPlugin<ulong, NoOptions>
    {
        public void Reset(TweenCore<ulong, NoOptions> t)
        {
            
        }

        public void EvaluateAndApply(NoOptions options, Animation t, Getter<ulong> getter, Setter<ulong> setter, float elapsed, float duration,
            ulong startValue, ulong endValue)
        {
            setter.Invoke((ulong)Mathf.Lerp(startValue, endValue, (decimal)t.Easer.Invoke(elapsed / duration)));
        }
    }
}