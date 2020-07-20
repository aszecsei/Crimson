namespace Crimson.AI.BehaviorTree
{
    [AITag("RandomSequence")]
    public class RandomSequence<T> : Sequence<T>
    {
        public override void OnStart()
        {
            Children.Shuffle();
        }
    }
}