namespace Crimson.AI.BehaviorTree
{
    [AITag("Succeed")]
    public class AlwaysSucceed : Decorator
    {
        public AlwaysSucceed() : base(false) {}

        protected override TaskStatus Tick(Blackboard context)
        {
            Assert.IsNotNull(ChildInstance, "child must not be null");

            var status = ChildInstance!.Tick(context);

            if (status == TaskStatus.Running)
                return TaskStatus.Running;

            return TaskStatus.Success;
        }
    }
}