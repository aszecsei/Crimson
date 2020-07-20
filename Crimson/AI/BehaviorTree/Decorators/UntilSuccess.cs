namespace Crimson.AI.BehaviorTree
{
    [AITag("UntilSuccess")]
    public class UntilSuccess : Decorator
    {
        public override TaskStatus Update(Blackboard context)
        {
            Assert.IsNotNull(Child, "child must not be null");

            var status = Child.Tick(context);
            
            if (status != TaskStatus.Success)
                return TaskStatus.Running;
            
            return TaskStatus.Success;
        }
    }
}