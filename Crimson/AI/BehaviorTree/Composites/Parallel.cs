namespace Crimson.AI.BehaviorTree
{
    public class Parallel<T> : Composite<T>
    {
        public override TaskStatus Update(T context)
        {
            var didAllSucceed = true;
            for (var i = 0; i < Children.Count; ++i)
            {
                var child = Children[i];
                child.Tick(context);

                if (child.Status == TaskStatus.Failure)
                    return TaskStatus.Failure;
                
                else if (child.Status != TaskStatus.Success)
                    didAllSucceed = false;
            }

            if (didAllSucceed)
                return TaskStatus.Success;

            return TaskStatus.Running;
        }
    }
}