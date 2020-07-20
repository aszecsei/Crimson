using System;

namespace Crimson.AI.BehaviorTree
{
    [AITag("Random", "probability")]
    public class RandomProbability<T> : Behavior<T>, IConditional<T>
    {
        private float _successProbability;

        public RandomProbability(float successProbability)
        {
            _successProbability = successProbability;
        }

        public override TaskStatus Update(T context)
        {
            if (Utils.Random.NextFloat() > _successProbability)
                return TaskStatus.Success;
            return TaskStatus.Failure;
        }
    }
}