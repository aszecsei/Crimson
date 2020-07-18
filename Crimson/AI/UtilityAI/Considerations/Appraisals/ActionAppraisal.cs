using System;

namespace Crimson.AI.UtilityAI
{
    /// <summary>
    /// Wraps a <see cref="Func{T, TResult}"/> for use as an
    /// <see cref="IAppraisal{T}"/> without having to create a subclass
    /// </summary>
    public class ActionAppraisal<T> : IAppraisal<T>
    {
        private readonly Func<T, float> _appraisalAction;

        public ActionAppraisal(Func<T, float> appraisalAction)
        {
            _appraisalAction = appraisalAction;
        }

        public float GetScore(T context)
        {
            return _appraisalAction(context);
        }
    }
}