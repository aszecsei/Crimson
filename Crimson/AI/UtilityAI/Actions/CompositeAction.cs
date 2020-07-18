using System.Collections.Generic;

namespace Crimson.AI.UtilityAI
{
    public class CompositeAction<T> : IAction<T>
    {
        private List<IAction<T>> _actions = new List<IAction<T>>();

        public void Execute(T context)
        {
            for (var i = 0; i < _actions.Count; ++i)
                _actions[i].Execute(context);
        }

        public CompositeAction<T> AddAction(IAction<T> action)
        {
            _actions.Add(action);
            return this;
        }
    }
}