namespace Crimson.AI.BehaviorTree
{
    [AITag("Fail")]
    public class AlwaysFail : Decorator
    {
        protected override TaskStatus Tick(Blackboard context)
        {
            Assert.IsNotNull(ChildInstance, "child must not be null");

            var status = ChildInstance!.Tick(context);

            if (status == TaskStatus.Running)
                return TaskStatus.Running;

            return TaskStatus.Failure;
        }

        public AlwaysFail() : base(false)
        {
        }
    }
}