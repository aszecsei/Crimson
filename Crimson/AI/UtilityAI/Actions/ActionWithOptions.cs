using System.Collections.Generic;

namespace Crimson.AI.UtilityAI
{
    public abstract class ActionWithOptions<T> : Action
    {
        protected List<IActionOptionAppraisal<T>> _appraisals = new List<IActionOptionAppraisal<T>>();

        public T GetBestOption(Blackboard context, List<T> options)
        {
            var result = default(T);
            var bestScore = Mathf.NEG_INFINITY;

            for (var i = 0; i < options.Count; ++i)
            {
                var option = options[i];
                var current = 0f;
                for (var j = 0; j < _appraisals.Count; ++j)
                    current += _appraisals[j].GetScore(context, option);

                if (current > bestScore)
                {
                    bestScore = current;
                    result = option;
                }
            }

            return result;
        }

        public ActionWithOptions<T> AddScorer(IActionOptionAppraisal<T> scorer)
        {
            _appraisals.Add(scorer);
            return this;
        }
    }
}