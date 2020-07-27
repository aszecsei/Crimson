using System;

namespace Crimson.AI.BehaviorTree
{
    [AITag("Random")]
    public class RandomProbability : IConditional
    {
        private readonly float _successProbability;

        public RandomProbability(float probability)
        {
            _successProbability = probability;
        }

        public TaskStatus Update(Blackboard context)
        {
            if (Utils.Random.NextFloat() > _successProbability)
                return TaskStatus.Success;
            return TaskStatus.Failure;
        }
    }
}