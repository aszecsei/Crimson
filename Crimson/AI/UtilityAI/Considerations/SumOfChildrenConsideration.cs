using System.Collections.Generic;

namespace Crimson.AI.UtilityAI
{
    /// <summary>
    /// Scores by summing the score of all child Appraisals
    /// </summary>
    public class SumOfChildrenConsideration<T> : IConsideration<T>
    {
        public IAction<T> Action { get; set; }
        private List<IAppraisal<T>> _appraisals = new List<IAppraisal<T>>();

        public SumOfChildrenConsideration<T> AddAppraisal(IAppraisal<T> appraisal)
        {
            _appraisals.Add(appraisal);
            return this;
        }
        
        public float GetScore(T context)
        {
            var score = 0f;
            for (var i = 0; i < _appraisals.Count; ++i)
                score += _appraisals[i].GetScore(context);
            return score;
        }
    }
}