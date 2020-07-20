using System.Collections.Generic;

namespace Crimson.AI.UtilityAI
{
    /// <summary>
    /// Scores by summing child Appraisals until a child scores below the threshold
    /// </summary>
    public class ThresholdConsideration : IConsideration
    {
        public float Threshold;
        public IAction Action { get; set; }
        private List<IAppraisal> _appraisals = new List<IAppraisal>();

        public ThresholdConsideration(float threshold = 0)
        {
            Threshold = threshold;
        }

        public ThresholdConsideration AddAppraisal(IAppraisal appraisal)
        {
            _appraisals.Add(appraisal);
            return this;
        }

        public float GetScore(Blackboard context)
        {
            var sum = 0f;
            for (var i = 0; i < _appraisals.Count; ++i)
            {
                var score = _appraisals[i].GetScore(context);
                if (score < Threshold)
                    return sum;
                sum += score;
            }

            return sum;
        }
    }
}