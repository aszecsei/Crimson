using Crimson.Tweening.Plugins.Options;
using Microsoft.Xna.Framework;

namespace Crimson.Tweening.Plugins
{
    public class Vector4Plugin : ITweenPlugin<Vector4, VectorOptions>
    {
        public void Reset(TweenCore<Vector4, VectorOptions> t)
        {
            
        }

        public void EvaluateAndApply(VectorOptions options, Animation t, Getter<Vector4> getter, Setter<Vector4> setter, float elapsed, float duration,
            Vector4 startValue, Vector4 endValue)
        {
            float easeVal = t.Easer.Invoke(elapsed / duration);
            Vector4 res = getter();
            
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
            if (options.AxisConstraint.HasFlag(AxisConstraint.Z))
            {
                res.Z = Mathf.Lerp(startValue.Z, endValue.Z, easeVal);
                if (options.Snapping)
                {
                    res.Z = Mathf.Round(res.Z);
                }
            }
            if (options.AxisConstraint.HasFlag(AxisConstraint.W))
            {
                res.W = Mathf.Lerp(startValue.W, endValue.W, easeVal);
                if (options.Snapping)
                {
                    res.W = Mathf.Round(res.W);
                }
            }

            setter(res);
        }
    }
}