﻿namespace Crimson.AI.BehaviorTree
{
    /// <summary>
    /// The selector task is similar to an "or" operation. It will return success as soon as one of its child tasks return success. If a
    /// child task returns failure then it will sequentially run the next task. If no child task returns success then it will return failure.
    /// </summary>
    [AITag("Selector")]
    public class Selector : Composite
    {
        public Selector(AbortType abortType = AbortType.None)
        {
            AbortType = abortType;
        }

        public override TaskStatus Update(Blackboard context)
        {
            if (CurrentChildIndex != 0)
                HandleConditionalAborts(context);

            var current = ChildrenInstances[CurrentChildIndex];
            var status = current.Tick(context);

            if (status != TaskStatus.Failure)
                return status;

            CurrentChildIndex++;

            if (CurrentChildIndex == ChildrenInstances.Length)
            {
                CurrentChildIndex = 0;
                return TaskStatus.Failure;
            }

            return TaskStatus.Running;
        }
        
        void HandleConditionalAborts(Blackboard context)
        {
            // check any lower priority tasks to see if they changed to a success
            if (HasLowerPriorityConditionalAbort)
                UpdateLowerPriorityAbortConditional(context, TaskStatus.Failure);

            if (AbortType.HasFlag(AbortType.Self))
                UpdateSelfAbortConditional(context, TaskStatus.Failure);
        }
    }
}