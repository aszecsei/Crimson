namespace Crimson.AI.BehaviorTree
{
    public class AlwaysFail<T> : Decorator<T>
    {
        public override TaskStatus Update(T context)
        {
            Assert.IsNotNull(Child, "child must not be null");

            var status = Child.Tick(context);

            if (status == TaskStatus.Running)
                return TaskStatus.Running;

            return TaskStatus.Failure;
        }
    }
}