namespace Crimson.AI.BehaviorTree
{
    [AITag("UntilSuccess")]
    public class UntilSuccess<T> : Decorator<T>
    {
        public override TaskStatus Update(T context)
        {
            Assert.IsNotNull(Child, "child must not be null");

            var status = Child.Tick(context);
            
            if (status != TaskStatus.Success)
                return TaskStatus.Running;
            
            return TaskStatus.Success;
        }
    }
}