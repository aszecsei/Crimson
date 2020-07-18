﻿using System.Collections.Generic;

namespace Crimson.AI.UtilityAI
{
    /// <summary>
    /// Only scores if all child Appraisals score above the threshold
    /// </summary>
    public class AllOrNothingConsideration<T> : IConsideration<T>
    {
        public float Threshold;
        public IAction<T> Action { get; set; }
        private List<IAppraisal<T>> _appraisals = new List<IAppraisal<T>>();

        public AllOrNothingConsideration(float threshold = 0)
        {
            Threshold = threshold;
        }

        public AllOrNothingConsideration<T> AddAppraisal(IAppraisal<T> appraisal)
        {
            _appraisals.Add(appraisal);
            return this;
        }

        public float GetScore(T context)
        {
            var sum = 0f;
            for (var i = 0; i < _appraisals.Count; ++i)
            {
                var score = _appraisals[i].GetScore(context);
                if (score < Threshold)
                    return 0;
                sum += score;
            }

            return sum;
        }
    }
}