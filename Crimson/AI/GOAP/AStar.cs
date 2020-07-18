using System;
using System.Collections.Generic;
using Crimson.Collections;

namespace Crimson.AI.GOAP
{
    public class AStarNode : IComparable<AStarNode>, IEquatable<AStarNode>, IPoolable, ICloneable
    {
        public WorldState WorldState;
        public int CostSoFar;
        public int HeuristicCost;
        public int CostSoFarAndHeuristicCost;
        public Action? Action;

        public AStarNode? Parent;
        public WorldState? ParentWorldState;
        public int Depth;

        #region IEquatable and IComparable

        public bool Equals(AStarNode other)
        {
            long care = WorldState.DontCare ^ -1L;
            return (WorldState.Values & care) == (other.WorldState.Values & care);
        }

        public int CompareTo(AStarNode other)
        {
            return CostSoFarAndHeuristicCost.CompareTo(other.CostSoFarAndHeuristicCost);
        }

        #endregion

        public void Reset()
        {
            Action = null;
            Parent = null;
        }

        public object Clone()
        {
            return (AStarNode) MemberwiseClone();
        }

        public override string ToString()
        {
            return $"[cost: {CostSoFar} | heuristic: {HeuristicCost}]: {Action}";
        }
    }
    
    public class AStar
    {
        private static AStarStorage s_storage = new AStarStorage();
        
        /* from: http://theory.stanford.edu/~amitp/GameProgramming/ImplementationNotes.html
		OPEN = priority queue containing START
		CLOSED = empty set
		while lowest rank in OPEN is not the GOAL:
		  current = remove lowest rank item from OPEN
		  add current to CLOSED
		  for neighbors of current:
		    cost = g(current) + movementcost(current, neighbor)
		    if neighbor in OPEN and cost less than g(neighbor):
		      remove neighbor from OPEN, because new path is better
		    if neighbor in CLOSED and cost less than g(neighbor): **
		      remove neighbor from CLOSED
		    if neighbor not in OPEN and neighbor not in CLOSED:
		      set g(neighbor) to cost
		      add neighbor to OPEN
		      set priority queue rank to g(neighbor) + h(neighbor)
		      set neighbor's parent to current
        */

		/// <summary>
		/// Make a plan of actions that will reach desired world state
		/// </summary>
		public static Stack<Action>? Plan(ActionPlanner ap, WorldState start, WorldState goal,
		                                 List<AStarNode>? selectedNodes = null)
		{
			s_storage.Clear();

			var currentNode = Pool<AStarNode>.Obtain();
			currentNode.WorldState = start;
			currentNode.ParentWorldState = start;
			currentNode.CostSoFar = 0; // g
			currentNode.HeuristicCost = CalculateHeuristic(start, goal); // h
			currentNode.CostSoFarAndHeuristicCost = currentNode.CostSoFar + currentNode.HeuristicCost; // f
			currentNode.Depth = 1;

			s_storage.AddToOpenList(currentNode);

			while (true)
			{
				// nothing left open so we failed to find a path
				if (!s_storage.HasOpened())
				{
					s_storage.Clear();
					return null;
				}

				currentNode = s_storage.RemoveCheapestOpenNode()!;

				s_storage.AddToClosedList(currentNode);

				// all done. we reached our goal
				if (goal.Equals(currentNode.WorldState))
				{
					var plan = ReconstructPlan(currentNode, selectedNodes);
					s_storage.Clear();
					return plan;
				}

				var neighbors = ap.GetPossibleTransitions(currentNode.WorldState);
				for (var i = 0; i < neighbors.Count; i++)
				{
					var cur = neighbors[i];
					var opened = s_storage.FindOpened(cur);
					var closed = s_storage.FindClosed(cur);
					var cost = currentNode.CostSoFar + cur.CostSoFar;

					// if neighbor in OPEN and cost less than g(neighbor):
					if (opened != null && cost < opened.CostSoFar)
					{
						// remove neighbor from OPEN, because new path is better
						s_storage.RemoveOpened(opened);
						opened = null;
					}

					// if neighbor in CLOSED and cost less than g(neighbor):
					if (closed != null && cost < closed.CostSoFar)
					{
						// remove neighbor from CLOSED
						s_storage.RemoveClosed(closed);
					}

					// if neighbor not in OPEN and neighbor not in CLOSED:
					if (opened == null && closed == null)
					{
						var nb = Pool<AStarNode>.Obtain();
						nb.WorldState = cur.WorldState;
						nb.CostSoFar = cost;
						nb.HeuristicCost = CalculateHeuristic(cur.WorldState, goal);
						nb.CostSoFarAndHeuristicCost = nb.CostSoFar + nb.HeuristicCost;
						nb.Action = cur.Action;
						nb.ParentWorldState = currentNode.WorldState;
						nb.Parent = currentNode;
						nb.Depth = currentNode.Depth + 1;
						s_storage.AddToOpenList(nb);
					}
				}
			}
		}


		/// <summary>
		/// internal function to reconstruct the plan by tracing from last node to initial node
		/// </summary>
		static Stack<Action> ReconstructPlan(AStarNode goalNode, List<AStarNode>? selectedNodes)
		{
			var totalActionsInPlan = goalNode.Depth - 1;
			var plan = new Stack<Action>(totalActionsInPlan);

			var curnode = goalNode;
			for (var i = 0; i <= totalActionsInPlan - 1; i++)
			{
				// optionally add the node to the List if we have been passed one
				if (selectedNodes != null)
					selectedNodes.Add((AStarNode)curnode.Clone());
				plan.Push(curnode.Action!);
				curnode = curnode.Parent!;
			}

			// our nodes went from the goal back to the start so reverse them
			if (selectedNodes != null)
				selectedNodes.Reverse();

			return plan;
		}


		/// <summary>
		/// This is our heuristic: estimate for remaining distance is the nr of mismatched atoms that matter.
		/// </summary>
		static int CalculateHeuristic(WorldState fr, WorldState to)
		{
			long care = (to.DontCare ^ -1L);
			long diff = (fr.Values & care) ^ (to.Values & care);
			int dist = 0;

			for (var i = 0; i < ActionPlanner.MAX_CONDITIONS; ++i)
				if ((diff & (1L << i)) != 0)
					dist++;
			return dist;
		}
    }
}