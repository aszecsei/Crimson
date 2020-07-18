namespace Crimson.AI.UtilityAI
{
    /// <summary>
    /// Encapsulates an action and generates a score that a <see cref="Reasoner{T}"/>
    /// can use to decide which <see cref="IConsideration{T}"/> to use
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IConsideration<T>
    {
        IAction<T>? Action { get; set; }
        float GetScore(T context);
    }
}