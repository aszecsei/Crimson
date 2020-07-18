using System;

namespace Crimson.AI.BehaviorTree
{
    public class ExecuteActionConditional<T> : ExecuteAction<T>, IConditional<T>
    {
        public ExecuteActionConditional(Func<T, TaskStatus> action) : base(action)
        {
        }
    }
}