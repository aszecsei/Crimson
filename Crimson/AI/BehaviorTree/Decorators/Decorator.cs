namespace Crimson.AI.BehaviorTree
{
    public abstract class Decorator<T> : Behavior<T>
    {
        public Behavior<T> Child;

        public override void Invalidate()
        {
            base.Invalidate();
            Child.Invalidate();
        }

        public override float Utility => Child.Utility;
    }
}