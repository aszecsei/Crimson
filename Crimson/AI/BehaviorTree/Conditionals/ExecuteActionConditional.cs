using System;

namespace Crimson.AI.BehaviorTree
{
    /// <summary>
    /// Wraps an ExecuteAction so it can be used as a conditional
    /// </summary>
    public class ExecuteActionConditional : ExecuteAction, IConditional
    {
        public ExecuteActionConditional(Func<Blackboard, TaskStatus> action) : base(action)
        {
        }
    }
}