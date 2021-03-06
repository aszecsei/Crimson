﻿namespace Crimson.AI.BehaviorTree
{
    [AITag("ParallelSelector")]
    public class ParallelSelector : Composite
    {
        protected override TaskStatus Tick(Blackboard context)
        {
            var didAllFail = true;
            for (var i = 0; i < ChildrenInstances.Length; ++i)
            {
                var child = ChildrenInstances[i];
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