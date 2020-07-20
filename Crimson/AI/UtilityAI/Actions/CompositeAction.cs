using System.Collections.Generic;

namespace Crimson.AI.UtilityAI
{
    public class CompositeAction : IAction
    {
        private List<IAction> _actions = new List<IAction>();

        public void Execute(Blackboard context)
        {
            for (var i = 0; i < _actions.Count; ++i)
                _actions[i].Execute(context);
        }

        public CompositeAction AddAction(IAction action)
        {
            _actions.Add(action);
            return this;
        }
    }
}