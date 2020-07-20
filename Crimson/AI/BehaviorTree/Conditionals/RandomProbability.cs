using System;

namespace Crimson.AI.BehaviorTree
{
    [AITag("Random")]
    public class RandomProbability<T> : Behavior, IConditional
    {
        private float _successProbability;

        public RandomProbability(float probability)
        {
            _successProbability = probability;
        }

        public override TaskStatus Update(Blackboard context)
        {
            if (Utils.Random.NextFloat() > _successProbability)
                return TaskStatus.Success;
            return TaskStatus.Failure;
        }
    }
}