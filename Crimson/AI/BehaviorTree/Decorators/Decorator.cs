namespace Crimson.AI.BehaviorTree
{
    /// <summary>
    /// Decorators are supporting nodes placed on parent-child connections that receive notifications about execution
    /// flow and can be ticked
    /// </summary>
    public abstract class Decorator : Node
    {
        public readonly AbortMode AbortMode;
        public Node? Child;
        public TaskInstance? ChildInstance;
        public bool IsInversed { get; private set; } = false;

        protected Decorator(bool isInversed, AbortMode abortMode = AbortMode.None)
        {
            IsInversed = isInversed;
            AbortMode = abortMode;
        }

        public override string OperatorType
        {
            get
            {
                var prefix = IsInversed ? "~" : "";
                return $"{prefix}Decorator (abort {AbortMode})";
            }
        } 

        public override void OnStart()
        {
            base.OnStart();
            ChildInstance = Child?.Instance();
        }

        public override void OnEnd()
        {
            base.OnEnd();
            ChildInstance?.Invalidate();
            ChildInstance = null;
        }
    }
}