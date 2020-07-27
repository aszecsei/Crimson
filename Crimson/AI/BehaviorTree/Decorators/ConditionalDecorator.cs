namespace Crimson.AI.BehaviorTree
{
    public class ConditionalDecorator : Decorator
    {
        private readonly IConditional _conditional;
        private readonly bool _shouldReevaluate;
        private TaskStatus _conditionalStatus;

        public ConditionalDecorator(IConditional conditional, bool shouldReevaluate = true, bool isInversed = false, AbortMode abortMode = AbortMode.None) : base(isInversed, abortMode)
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

        protected override TaskStatus Tick(Blackboard context)
        {
            var status = ExecuteConditional(context);
            if (status == TaskStatus.Success)
                return ChildInstance!.Tick(context);
            return status;
        }

        internal TaskStatus ExecuteConditional(Blackboard context, bool forceUpdate = false)
        {
            if (forceUpdate || _shouldReevaluate || _conditionalStatus == TaskStatus.Invalid)
                _conditionalStatus = _conditional.Update(context);
            return _conditionalStatus;
        }
    }
}