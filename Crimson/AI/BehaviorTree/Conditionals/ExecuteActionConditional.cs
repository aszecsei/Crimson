using System;

namespace Crimson.AI.BehaviorTree
{
    public class ExecuteActionConditional : ExecuteAction, IConditional
    {
        public ExecuteActionConditional(Func<Blackboard, TaskStatus> action) : base(action)
        {
        }
    }
}