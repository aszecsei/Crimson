using System;

namespace Crimson.AI.BehaviorTree
{
    public class ExecuteAction : Behavior
    {
        private Func<Blackboard, TaskStatus> _action;
        private float _utility;

        public ExecuteAction(Func<Blackboard, TaskStatus> action, float utility = 1f)
        {
            _action = action;
            _utility = utility;
        }

        public override float Utility => _utility;

        public override TaskStatus Update(Blackboard context)
        {
            Assert.IsNotNull(_action, "action must not be null");
            return _action(context);
        }
    }
}