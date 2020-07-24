namespace Crimson.AI.UtilityAI
{
    /// <summary>
    /// Encapsulates an action and generates a score that a <see cref="Reasoner"/>
    /// can use to decide which <see cref="IConsideration"/> to use
    /// </summary>
    public interface IConsideration
    {
        Action? Action { get; set; }
        float GetScore(Blackboard context);
    }
}