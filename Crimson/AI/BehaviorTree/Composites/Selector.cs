namespace Crimson.AI.BehaviorTree
{
    /// <summary>
    /// The selector task is similar to an "or" operation. It will return success as soon as one of its child tasks return success. If a
    /// child task returns failure then it will sequentially run the next task. If no child task returns success then it will return failure.
    /// </summary>
    [AITag("Selector")]
    public class Selector : Composite
    {
        protected override TaskStatus Tick(Blackboard context)
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

        private void HandleConditionalAborts(Blackboard context)
        {
            if (HasLowerPriorityConditionalAbort)
                UpdateLowerPriorityAbortConditional(context, TaskStatus.Failure);
            
            if (HasSelfConditionalAbort)
                UpdateSelfAbortConditional(context, TaskStatus.Failure);
        }
    }
}