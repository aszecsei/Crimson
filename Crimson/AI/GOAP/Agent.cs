using System.Collections.Generic;
using Crimson.Collections;

namespace Crimson.AI.GOAP
{
    public abstract class Agent
    {
        public Stack<Action>? Actions;
        protected ActionPlanner Planner;

        public Blackboard WorldState;
        public Blackboard GoalState;

        protected Agent()
        {
            Planner = new ActionPlanner();
        }

        public bool Plan(bool debugPlan = false)
        {
            List<AStarNode>? nodes = null;
            if (debugPlan)
                nodes = new List<AStarNode>();

            Actions = Planner.Plan(WorldState, GoalState, nodes);

            if (nodes != null && nodes.Count > 0)
            {
                Utils.Log("---- ActionPlanner plan ----");
                Utils.Log($"plan cost = {nodes[nodes.Count - 1].CostSoFar}");
                for (var i = 0; i < nodes.Count; ++i)
                {
                    Utils.Log($"{i}: {nodes[i].Action!.GetType().Name.PadRight(15)}\t{nodes[i].WorldState}");
                    Pool<AStarNode>.Free(nodes[i]);
                }
            }

            return HasActionPlan();
        }

        public bool HasActionPlan()
        {
            return Actions != null && Actions.Count > 0;
        }
    }
}