using System;

namespace Crimson.AI
{
    public class ExecuteOperator : Operator
    {
        private readonly Func<Blackboard, TaskStatus> _action;
        
        public ExecuteOperator(Func<Blackboard, TaskStatus> action, int utility = 1, int cost = 1)
        {
            _action = action;
            Utility = utility;
            Cost = cost;
        }

        public override int Utility { get; }

        public override int Cost { get; }

        public override TaskStatus Update(Blackboard context)
        {
            Assert.IsNotNull(_action, "action must not be null");
            return _action(context);
        }
    }
}