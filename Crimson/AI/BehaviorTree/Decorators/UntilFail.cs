namespace Crimson.AI.BehaviorTree
{
    [AITag("UntilFail")]
    public class UntilFail : Decorator
    {
        public override TaskStatus Update(Blackboard context)
        {
            Assert.IsNotNull(ChildInstance, "child must not be null");

            var status = ChildInstance!.Tick(context);
            
            if (status != TaskStatus.Failure)
                return TaskStatus.Running;
            
            return TaskStatus.Success;
        }
    }
}