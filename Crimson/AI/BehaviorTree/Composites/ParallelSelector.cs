namespace Crimson.AI.BehaviorTree
{
    [AITag("ParallelSelector")]
    public class ParallelSelector : Composite
    {
        public override TaskStatus Update(Blackboard context)
        {
            var didAllFail = true;
            for (var i = 0; i < Children.Count; ++i)
            {
                var child = Children[i];
                child.Tick(context);

                if (child.Status == TaskStatus.Success)
                    return TaskStatus.Success;

                if (child.Status != TaskStatus.Failure)
                    didAllFail = false;
            }

            if (didAllFail)
                return TaskStatus.Failure;

            return TaskStatus.Running;
        }
    }
}