using System.Collections.Generic;
using Priority_Queue;

namespace Crimson.AI.Pathfinding
{
    public static class AStarPathfinder
    {
        private const int MAX_NODES = 1000;
        
        private class AStarNode<T> : FastPriorityQueueNode
        {
            public readonly T Data;

            public AStarNode(T data)
            {
                Data = data;
            }
        }

        public static bool Search<T>(IAStarGraph<T> graph, T start, T goal, out Dictionary<T, T> cameFrom)
        {
            cameFrom = new Dictionary<T, T> {{start, start}};

            var costSoFar = new Dictionary<T, int>();
            var frontier = new FastPriorityQueue<AStarNode<T>>(MAX_NODES);
            frontier.Enqueue(new AStarNode<T>(start), 0);

            costSoFar[start] = 0;

            while (frontier.Count > 0)
            {
                var current = frontier.Dequeue();

                if (current.Data!.Equals(goal))
                {
                    return true;
                }

                foreach (var next in graph.GetNeighbors(current.Data))
                {
                    var newCost = costSoFar[current.Data] + graph.Cost(current.Data, next);
                    if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
                    {
                        costSoFar[next] = newCost;
                        var priority = newCost + graph.Heuristic(next, goal);
                        frontier.Enqueue(new AStarNode<T>(next), priority);
                        cameFrom[next] = current.Data;
                    }
                }
            }
            
            return false;
        }

        public static List<T>? Search<T>(IAStarGraph<T> graph, T start, T goal)
        {
            var foundPath = Search(graph, start, goal, out var cameFrom);
            return foundPath ? ReconstructPath(cameFrom, start, goal) : null;
        }

        public static List<T> ReconstructPath<T>(Dictionary<T, T> cameFrom, T start, T goal)
        {
            var path = new List<T>();
            var current = goal;
            path.Add(goal);
            while (!current!.Equals(start))
            {
                current = cameFrom[current];
                path.Add(current);
            }
            path.Reverse();

            return path;
        }
    }
}