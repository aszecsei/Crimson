namespace Crimson.AI.BehaviorTree
{
    public abstract class Decorator : Behavior
    {
        public Behavior Child;

        public override void Invalidate()
        {
            base.Invalidate();
            Child.Invalidate();
        }

        public override float Utility => Child.Utility;
    }
}