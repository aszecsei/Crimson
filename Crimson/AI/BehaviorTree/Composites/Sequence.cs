namespace Crimson.AI.BehaviorTree
{
    public class Sequence<T> : Composite<T>
    {
        public Sequence(AbortType abortType = AbortType.None)
        {
            AbortType = abortType;
        }

        public override TaskStatus Update(T context)
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

        private void HandleConditionalAborts(T context)
        {
            if (HasLowerPriorityConditionalAbort)
                UpdateLowerPriorityAbortConditional(context, TaskStatus.Success);

            if (AbortType.HasFlag(AbortType.Self))
                UpdateSelfAbortConditional(context, TaskStatus.Success);
        }
    }
}