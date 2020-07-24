namespace Crimson.AI.UtilityAI
{
    /// <summary>
    /// Always returns a fixed score. Serves double duty as a default <see cref="IConsideration{T}"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FixedScoreConsideration : IConsideration
    {
        public float Score;
        
        public Action? Action { get; set; }

        public FixedScoreConsideration(float score = 1)
        {
            Score = score;
        }

        public float GetScore(Blackboard context)
        {
            return Score;
        }
    }
}