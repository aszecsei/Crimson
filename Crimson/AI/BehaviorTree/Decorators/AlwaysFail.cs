namespace Crimson.AI.BehaviorTree
{
    [AITag("Fail")]
    public class AlwaysFail : Decorator
    {
        public override TaskStatus Update(Blackboard context)
        {
            Assert.IsNotNull(Child, "child must not be null");

            var status = Child.Tick(context);

            if (status == TaskStatus.Running)
                return TaskStatus.Running;

            return TaskStatus.Failure;
        }
    }
}