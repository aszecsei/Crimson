namespace Crimson.AI.BehaviorTree
{
    [AITag("Sequence")]
    public class Sequence : Composite
    {
        public Sequence(AbortType abortType = AbortType.None)
        {
            AbortType = abortType;
        }

        public override TaskStatus Update(Blackboard context)
        {
            if (CurrentChildIndex != 0)
                HandleConditionalAborts(context);

            var current = Children[CurrentChildIndex];
            var status = current.Tick(context);

            if (status != TaskStatus.Success)
                return status;

            CurrentChildIndex++;

            if (CurrentChildIndex == Children.Count)
            {
                CurrentChildIndex = 0;
                return TaskStatus.Success;
            }

            return TaskStatus.Running;
        }

        private void HandleConditionalAborts(Blackboard context)
        {
            if (HasLowerPriorityConditionalAbort)
                UpdateLowerPriorityAbortConditional(context, TaskStatus.Success);

            if (AbortType.HasFlag(AbortType.Self))
                UpdateSelfAbortConditional(context, TaskStatus.Success);
        }
    }
}