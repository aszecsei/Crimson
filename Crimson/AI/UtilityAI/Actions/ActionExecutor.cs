using System;

namespace Crimson.AI.UtilityAI
{
    /// <summary>
    /// Wraps an <see cref="Action"/> for use as an <see cref="IAction"/> without
    /// having to create a new subclass
    /// </summary>
    public class ActionExecutor : Action
    {
        private Action<Blackboard> _action;

        public ActionExecutor(Action<Blackboard> action)
        {
            _action = action;
        }

        public override TaskStatus Update(Blackboard context)
        {
            _action(context);
            return TaskStatus.Success;
        }
    }
}