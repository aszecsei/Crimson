namespace Crimson.AI.BehaviorTree
{
    public class ConditionalDecorator : Decorator, IConditional
    {
        private IConditional _conditional;
        private bool _shouldReevaluate;
        private TaskStatus _conditionalStatus;

        public ConditionalDecorator(IConditional conditional, bool shouldReevaluate = true)
        {
            Assert.IsNotNull(conditional, "conditional must not be null");
            _conditional = conditional;
            _shouldReevaluate = shouldReevaluate;
        }

        public override void OnEnd()
        {
            base.OnEnd();
            _conditionalStatus = TaskStatus.Invalid;
        }

        public override void OnStart()
        {
            base.OnStart();
            _conditionalStatus = TaskStatus.Invalid;
        }

        public override TaskStatus Update(Blackboard context)
        {
            Assert.IsNotNull(ChildInstance, "child must not be null");

            _conditionalStatus = ExecuteConditional(context);

            if (_conditionalStatus == TaskStatus.Success)
                return ChildInstance!.Tick(context);

            return TaskStatus.Failure;
        }

        internal TaskStatus ExecuteConditional(Blackboard context, bool forceUpdate = false)
        {
            if (forceUpdate || _shouldReevaluate || _conditionalStatus == TaskStatus.Invalid)
                _conditionalStatus = _conditional.Update(context);
            return _conditionalStatus;
        }
    }
}