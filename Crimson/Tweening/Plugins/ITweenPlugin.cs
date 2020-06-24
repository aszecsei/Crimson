using Crimson.Tweening.Plugins.Options;

namespace Crimson.Tweening.Plugins
{
    public interface ITweenPlugin<T, TPlugOptions> : IBaseTweenPlugin where TPlugOptions : struct, IPlugOptions
    {
        public void Reset(TweenCore<T, TPlugOptions> t);

        public void EvaluateAndApply(TPlugOptions options, Animation t, Getter<T> getter, Setter<T> setter,
            float elapsed, float duration, T startValue, T endValue);
    }
}