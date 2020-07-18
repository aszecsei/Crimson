using System;

namespace Crimson.AI.BehaviorTree
{
    public class ExecuteAction<T> : Behavior<T>
    {
        private Func<T, TaskStatus> _action;
        private float _utility;

        public ExecuteAction(Func<T, TaskStatus> action, float utility = 1f)
        {
            _action = action;
            _utility = utility;
        }

        public override float Utility => _utility;

        public override TaskStatus Update(T context)
        {
            Assert.IsNotNull(_action, "action must not be null");
            return _action(context);
        }
    }
}