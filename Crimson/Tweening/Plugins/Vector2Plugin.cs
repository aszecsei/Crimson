using Crimson.Tweening.Plugins.Options;
using Microsoft.Xna.Framework;

namespace Crimson.Tweening.Plugins
{
    public class Vector2Plugin : ITweenPlugin<Vector2, VectorOptions>
    {
        public void Reset(TweenCore<Vector2, VectorOptions> t)
        {
            
        }

        public void EvaluateAndApply(VectorOptions options, Animation t, Getter<Vector2> getter, Setter<Vector2> setter, float elapsed, float duration,
            Vector2 startValue, Vector2 endValue)
        {
            float easeVal = t.Easer.Invoke(elapsed / duration);
            Vector2 res = getter();
            if (options.AxisConstraint.HasFlag(AxisConstraint.X))
            {
                res.X = Mathf.Lerp(startValue.X, endValue.X, easeVal);
                if (options.Snapping)
                {
                    res.X = Mathf.Round(res.X);
                }
            }
            if (options.AxisConstraint.HasFlag(AxisConstraint.Y))
            {
                res.Y = Mathf.Lerp(startValue.Y, endValue.Y, easeVal);
                if (options.Snapping)
                {
                    res.Y = Mathf.Round(res.Y);
                }
            }

            setter(res);
        }
    }
}