namespace Crimson.AI.BehaviorTree
{
    [AITag("Succeed")]
    public class AlwaysSucceed<T> : Decorator<T>
    {
        public override TaskStatus Update(T context)
        {
            Assert.IsNotNull(Child, "child must not be null");

            var status = Child.Tick(context);

            if (status == TaskStatus.Running)
                return TaskStatus.Running;

            return TaskStatus.Success;
        }
    }
}