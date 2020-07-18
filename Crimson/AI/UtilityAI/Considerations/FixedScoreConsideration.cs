namespace Crimson.AI.UtilityAI
{
    /// <summary>
    /// Always returns a fixed score. Serves double duty as a default <see cref="IConsideration{T}"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FixedScoreConsideration<T> : IConsideration<T>
    {
        public float Score;
        
        public IAction<T>? Action { get; set; }

        public FixedScoreConsideration(float score = 1)
        {
            Score = score;
        }

        public float GetScore(T context)
        {
            return Score;
        }
    }
}