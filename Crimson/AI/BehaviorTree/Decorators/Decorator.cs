namespace Crimson.AI.BehaviorTree
{
    public abstract class Decorator : Behavior
    {
        public Behavior? Child;
        protected TaskInstance? ChildInstance;

        public override void OnStart()
        {
            ChildInstance = Child?.Instance();
        }

        public override void OnEnd()
        {
            base.OnEnd();
            ChildInstance?.OnEnd();
            ChildInstance?.Invalidate();
        }

        public override int Cost => ChildInstance!.Cost;
        public override int Utility => ChildInstance!.Utility;
    }
}