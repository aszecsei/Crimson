using System.Collections.Generic;

namespace Crimson.AI.UtilityAI
{
    public class CompositeAction : Action
    {
        private List<Action> _actions = new List<Action>();

        public override TaskStatus Update(Blackboard context)
        {
            for (var i = 0; i < _actions.Count; ++i)
                _actions[i].Update(context);
            return TaskStatus.Success;
        }

        public CompositeAction AddAction(Action action)
        {
            _actions.Add(action);
            return this;
        }
    }
}