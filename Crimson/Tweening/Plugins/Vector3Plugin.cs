using Crimson.Tweening.Plugins.Options;
using Microsoft.Xna.Framework;

namespace Crimson.Tweening.Plugins
{
    public class Vector3Plugin : ITweenPlugin<Vector3, VectorOptions>
    {
        public void Reset(TweenCore<Vector3, VectorOptions> t)
        {
            
        }

        public void EvaluateAndApply(VectorOptions options, Animation t, Getter<Vector3> getter, Setter<Vector3> setter, float elapsed, float duration,
            Vector3 startValue, Vector3 endValue)
        {
            float easeVal = t.Easer.Invoke(elapsed / duration);
            Vector3 res = getter();
            
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

            setter(res);
        }
    }
}