namespace Crimson.AI.UtilityAI
{
    public class HighestScoreReasoner : Reasoner
    {
        protected override IConsideration SelectBestConsideration(Blackboard context)
        {
            var highestScore = DefaultConsideration.GetScore(context);
            IConsideration? consideration = null;
            for (var i = 0; i < _considerations.Count; ++i)
            {
                var score = _considerations[i].GetScore(context);
                if (score > highestScore)
                {
                    highestScore = score;
                    consideration = _considerations[i];
                }
            }

            if (consideration == null)
                return DefaultConsideration;
            return consideration;
        }
    }
}