using System.Collections.Generic;
using System.Text;

namespace Crimson.AI.GOAP
{
    public class ActionPlanner
    {
        public const int MAX_CONDITIONS = 64;
        
        public string?[] ConditionNames = new string[MAX_CONDITIONS];
        
        private List<Action> _actions = new List<Action>();
        private List<Action> _viableActions = new List<Action>();
        
        private WorldState[] _preConditions = new WorldState[MAX_CONDITIONS];
        private WorldState[] _postConditions = new WorldState[MAX_CONDITIONS];
        private int _numConditionNames;

        public ActionPlanner()
        {
            _numConditionNames = 0;
            for (var i = 0; i < MAX_CONDITIONS; ++i)
            {
                ConditionNames[i] = null;
                _preConditions[i] = WorldState.Create(this);
                _postConditions[i] = WorldState.Create(this);
            }
        }

        public WorldState CreateWorldState()
        {
            return WorldState.Create(this);
        }

        public void AddAction(Action action)
        {
            var actionId = FindActionIndex(action);
            if (actionId == -1)
                throw new KeyNotFoundException("could not find or create action");

            foreach (var preCondition in action.PreConditions)
            {
                var conditionId = FindConditionNameIndex(preCondition.Key);
                if (conditionId == -1)
                    throw new KeyNotFoundException("could not find or create conditionName");

                _preConditions[actionId].Set(conditionId, preCondition.Value);
            }

            foreach (var postCondition in action.PostConditions)
            {
                var conditionId = FindConditionNameIndex(postCondition.Key);
                if (conditionId == -1)
                    throw new KeyNotFoundException("could not find or create conditionName");

                _postConditions[actionId].Set(conditionId, postCondition.Value);
            }
        }

        public Stack<Action>? Plan(WorldState startState, WorldState goalState, List<AStarNode>? selectedNodes = null)
        {
            _viableActions.Clear();
            for (var i = 0; i < _actions.Count; ++i)
            {
                if (_actions[i].Validate())
                    _viableActions.Add(_actions[i]);
            }

            return AStar.Plan(this, startState, goalState, selectedNodes);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            for (var a = 0; a < _actions.Count; ++a)
            {
                sb.AppendLine(_actions[a].GetType().Name);

                var pre = _preConditions[a];
                var post = _postConditions[a];
                for (var i = 0; i < MAX_CONDITIONS; ++i)
                {
                    if ((pre.DontCare & (1L << i)) == 0)
                    {
                        bool v = (pre.Values & (1L << i)) != 0;
                        sb.AppendFormat("\t{0}=={1}\n", ConditionNames[i], v ? 1 : 0);
                    }
                }
                for (var i = 0; i < MAX_CONDITIONS; ++i)
                {
                    if ((post.DontCare & (1L << i)) == 0)
                    {
                        bool v = (post.Values & (1L << i)) != 0;
                        sb.AppendFormat("\t{0}=>{1}\n", ConditionNames[i], v ? 1 : 0);
                    }
                }
            }

            return sb.ToString();
        }

        internal int FindConditionNameIndex(string conditionName)
        {
            int idx;
            for (idx = 0; idx < _numConditionNames; ++idx)
            {
                if (string.Equals(ConditionNames[idx], conditionName))
                    return idx;
            }

            if (idx < MAX_CONDITIONS - 1)
            {
                ConditionNames[idx] = conditionName;
                _numConditionNames++;
                return idx;
            }

            return -1;
        }

        internal int FindActionIndex(Action action)
        {
            var idx = _actions.IndexOf(action);
            if (idx > -1)
                return idx;
            _actions.Add(action);
            return _actions.Count - 1;
        }

        internal List<AStarNode> GetPossibleTransitions(WorldState fr)
        {
            var result = new List<AStarNode>();
            for (var i = 0; i < _viableActions.Count; ++i)
            {
                // see if precondition is met
                var pre = _preConditions[i];
                var care = (pre.DontCare ^ -1L);
                bool met = ((pre.Values & care) == (fr.Values & care));
                if (met)
                {
                    var node = new AStarNode();
                    node.Action = _viableActions[i];
                    node.CostSoFar = _viableActions[i].Cost;
                    node.WorldState = ApplyPostConditions(this, i, fr);
                    result.Add(node);
                }
            }

            return result;
        }

        internal WorldState ApplyPostConditions(ActionPlanner ap, int actionnr, WorldState fr)
        {
            var pst = ap._postConditions[actionnr];
            long unaffected = pst.DontCare;
            long affected = (unaffected ^ -1L);

            fr.Values = (fr.Values & unaffected) | (pst.Values & affected);
            fr.DontCare &= pst.DontCare;
            return fr;
        }
    }
}