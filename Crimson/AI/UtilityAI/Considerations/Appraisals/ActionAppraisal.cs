using System;

namespace Crimson.AI.UtilityAI
{
    /// <summary>
    /// Wraps a <see cref="Func{T, TResult}"/> for use as an
    /// <see cref="IAppraisal{T}"/> without having to create a subclass
    /// </summary>
    public class ActionAppraisal : IAppraisal
    {
        private readonly Func<Blackboard, float> _appraisalAction;

        public ActionAppraisal(Func<Blackboard, float> appraisalAction)
        {
            _appraisalAction = appraisalAction;
        }

        public float GetScore(Blackboard context)
        {
            return _appraisalAction(context);
        }
    }
}