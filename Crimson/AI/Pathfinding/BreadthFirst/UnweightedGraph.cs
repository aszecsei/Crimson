using System.Collections.Generic;

namespace Crimson.AI.Pathfinding
{
    public class UnweightedGraph<T> : IUnweightedGraph<T>
    {
        public Dictionary<T, T[]> Edges = new Dictionary<T, T[]>();

        public UnweightedGraph<T> AddEdgesForNode(T node, T[] edges)
        {
            Edges[node] = edges;
            return this;
        }

        public IEnumerable<T> GetNeighbors(T node)
        {
            return Edges[node];
        }
    }
}