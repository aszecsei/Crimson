using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Crimson.AI.GOAP
{
    public class ActionPlanner : IEnumerable<Action>
    {
        private List<Action> _actions = new List<Action>();
        private List<Action> _viableActions = new List<Action>();

        public void Add(Action action)
        {
            _actions.Add(action);
        }

        public Stack<Action>? Plan(Blackboard startState, Blackboard goalState, List<AStarNode>? selectedNodes = null)
        {
            _viableActions.Clear();
            for (var i = 0; i < _actions.Count; ++i)
            {
                if (_actions[i].Validate(startState))
                    _viableActions.Add(_actions[i]);
            }

            return AStar.Plan(this, startState, goalState, selectedNodes);
        }

        public IEnumerator<Action> GetEnumerator()
        {
            return _actions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        internal List<AStarNode> GetPossibleTransitions(Blackboard context)
        {
            var result = new List<AStarNode>();
            for (var i = 0; i < _viableActions.Count; ++i)
            {
                // see if precondition is met
                if (_viableActions[i].Validate(context))
                {
                    var node = new AStarNode();
                    node.Action = _viableActions[i];
                    node.CostSoFar = _viableActions[i].Cost;
                    node.WorldState = (Blackboard)context.Clone();
                    _viableActions[i].Execute(node.WorldState);
                    result.Add(node);
                }
            }

            return result;
        }
    }
}