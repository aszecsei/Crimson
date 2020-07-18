using System.Collections.Generic;

namespace Crimson.AI.UtilityAI
{
    public abstract class ActionWithOptions<T, U> : IAction<T>
    {
        protected List<IActionOptionAppraisal<T, U>> _appraisals = new List<IActionOptionAppraisal<T, U>>();

        public U GetBestOption(T context, List<U> options)
        {
            var result = default(U);
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

        public abstract void Execute(T context);

        public ActionWithOptions<T, U> AddScorer(IActionOptionAppraisal<T, U> scorer)
        {
            _appraisals.Add(scorer);
            return this;
        }
    }
}