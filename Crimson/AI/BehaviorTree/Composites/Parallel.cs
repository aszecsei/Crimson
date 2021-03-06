﻿namespace Crimson.AI.BehaviorTree
{
    [AITag("Parallel")]
    public class Parallel : Composite
    {
        protected override TaskStatus Tick(Blackboard context)
        {
            var didAllSucceed = true;
            for (var i = 0; i < ChildrenInstances.Length; ++i)
            {
                var child = ChildrenInstances[i];
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