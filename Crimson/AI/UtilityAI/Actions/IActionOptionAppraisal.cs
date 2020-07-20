namespace Crimson.AI.UtilityAI
{
    /// <summary>
    /// Appraisal for use with an <see cref="ActionWithOptions{T,U}"/>
    /// </summary>
    public interface IActionOptionAppraisal<T>
    {
        float GetScore(Blackboard context, T option);
    }
}