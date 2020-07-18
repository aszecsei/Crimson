namespace Crimson.AI.BehaviorTree
{
    public class RandomSequence<T> : Sequence<T>
    {
        public override void OnStart()
        {
            Children.Shuffle();
        }
    }
}