namespace Crimson.AI.BehaviorTree
{
    [AITag("Invert")]
    public class Inverter : Decorator
    {
        public override TaskStatus Update(Blackboard context)
        {
            Assert.IsNotNull(ChildInstance, "child must not be null");

            var status = ChildInstance!.Tick(context);

            if (status == TaskStatus.Success)
                return TaskStatus.Failure;
            if (status == TaskStatus.Failure)
                return TaskStatus.Success;
            return TaskStatus.Running;
        }
    }
}