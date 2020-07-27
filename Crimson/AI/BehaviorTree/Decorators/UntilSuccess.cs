namespace Crimson.AI.BehaviorTree
{
    [AITag("UntilSuccess")]
    public class UntilSuccess : Decorator
    {
        public UntilSuccess() : base(false) {}

        protected override TaskStatus Tick(Blackboard context)
        {
            Assert.IsNotNull(ChildInstance, "child must not be null");

            var status = ChildInstance!.Tick(context);
            
            if (status != TaskStatus.Success)
                return TaskStatus.Running;
            
            return TaskStatus.Success;
        }
    }
}