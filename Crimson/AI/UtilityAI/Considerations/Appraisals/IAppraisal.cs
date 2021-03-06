﻿namespace Crimson.AI.UtilityAI
{
    /// <summary>
    /// Scorer for use with an <see cref="IConsideration{T}"/>
    /// </summary>
    public interface IAppraisal
    {
        float GetScore(Blackboard context);
    }
}