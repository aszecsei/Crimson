using System;

namespace Crimson.AI.UtilityAI
{
    /// <summary>
    /// Wraps an <see cref="Action"/> for use as an <see cref="IAction{T}"/> without
    /// having to create a new subclass
    /// </summary>
    public class ActionExecutor<T> : IAction<T>
    {
        private Action<T> _action;

        public ActionExecutor(Action<T> action)
        {
            _action = action;
        }

        public void Execute(T context)
        {
            _action(context);
        }
    }
}