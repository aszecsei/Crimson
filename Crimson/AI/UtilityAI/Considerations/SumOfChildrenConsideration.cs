using System.Collections.Generic;

namespace Crimson.AI.UtilityAI
{
    /// <summary>
    /// Scores by summing the score of all child Appraisals
    /// </summary>
    public class SumOfChildrenConsideration : IConsideration
    {
        public IAction Action { get; set; }
        private List<IAppraisal> _appraisals = new List<IAppraisal>();

        public SumOfChildrenConsideration AddAppraisal(IAppraisal appraisal)
        {
            _appraisals.Add(appraisal);
            return this;
        }
        
        public float GetScore(Blackboard context)
        {
            var score = 0f;
            for (var i = 0; i < _appraisals.Count; ++i)
                score += _appraisals[i].GetScore(context);
            return score;
        }
    }
}