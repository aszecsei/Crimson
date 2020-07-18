namespace Crimson.AI.BehaviorTree
{
    public class ConditionalDecorator<T> : Decorator<T>, IConditional<T>
    {
        private IConditional<T> _conditional;
        private bool _shouldReevaluate;
        private TaskStatus _conditionalStatus;

        public ConditionalDecorator(IConditional<T> conditional, bool shouldReevaluate = true)
        {
            Assert.IsNotNull(conditional, "conditional must not be null");
            _conditional = conditional;
            _shouldReevaluate = shouldReevaluate;
        }

        public override void Invalidate()
        {
            base.Invalidate();
            _conditionalStatus = TaskStatus.Invalid;
        }

        public override void OnStart()
        {
            _conditionalStatus = TaskStatus.Invalid;
        }

        public override TaskStatus Update(T context)
        {
            Assert.IsNotNull(Child, "child must not be null");

            _conditionalStatus = ExecuteConditional(context);

            if (_conditionalStatus == TaskStatus.Success)
                return Child.Tick(context);

            return TaskStatus.Failure;
        }

        internal TaskStatus ExecuteConditional(T context, bool forceUpdate = false)
        {
            if (forceUpdate || _shouldReevaluate || _conditionalStatus == TaskStatus.Invalid)
                _conditionalStatus = _conditional.Update(context);
            return _conditionalStatus;
        }
    }
}