namespace Crimson.AI.UtilityAI
{
    public class FirstScoreReasoner : Reasoner
    {
        protected override IConsideration SelectBestConsideration(Blackboard context)
        {
            var defaultScore = DefaultConsideration.GetScore(context);
            for (var i = 0; i < _considerations.Count; ++i)
            {
                if (_considerations[i].GetScore(context) >= defaultScore)
                    return _considerations[i];
            }

            return DefaultConsideration;
        }
    }
}