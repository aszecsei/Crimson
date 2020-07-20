namespace Crimson.AI.BehaviorTree
{
    [AITag("RandomSequence")]
    public class RandomSequence : Sequence
    {
        public override void OnStart()
        {
            Children.Shuffle();
        }
    }
}