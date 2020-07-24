namespace Crimson.AI.BehaviorTree
{
    [AITag("Succeed")]
    public class AlwaysSucceed : Decorator
    {
        public override TaskStatus Update(Blackboard context)
        {
            Assert.IsNotNull(ChildInstance, "child must not be null");

            var status = ChildInstance!.Tick(context);

            if (status == TaskStatus.Running)
                return TaskStatus.Running;

            return TaskStatus.Success;
        }
    }
}