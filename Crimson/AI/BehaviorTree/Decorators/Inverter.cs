namespace Crimson.AI.BehaviorTree
{
    [AITag("Invert")]
    public class Inverter : Decorator
    {
        public override TaskStatus Update(Blackboard context)
        {
            Assert.IsNotNull(Child, "child must not be null");

            var status = Child.Tick(context);

            if (status == TaskStatus.Success)
                return TaskStatus.Failure;
            if (status == TaskStatus.Failure)
                return TaskStatus.Success;
            return TaskStatus.Running;
        }
    }
}