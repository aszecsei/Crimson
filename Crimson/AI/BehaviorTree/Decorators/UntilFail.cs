namespace Crimson.AI.BehaviorTree
{
    [AITag("UntilFail")]
    public class UntilFail : Decorator
    {
        public override TaskStatus Update(Blackboard context)
        {
            Assert.IsNotNull(Child, "child must not be null");

            var status = Child.Tick(context);
            
            if (status != TaskStatus.Failure)
                return TaskStatus.Running;
            
            return TaskStatus.Success;
        }
    }
}