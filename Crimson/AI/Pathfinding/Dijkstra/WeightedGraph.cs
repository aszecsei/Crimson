using System;
using System.Collections.Generic;

namespace Crimson.AI.Pathfinding
{
    public class WeightedGraph<T> : IWeightedGraph<T>
    {
        public Dictionary<T, Tuple<T,int>[]> Edges = new Dictionary<T, Tuple<T, int>[]>();

        public WeightedGraph<T> AddEdgesForNode(T node, Tuple<T, int>[] edges)
        {
            Edges[node] = edges;
            return this;
        }

        public IEnumerable<T> GetNeighbors(T node)
        {
            var edges = Edges[node];
            T[] res = new T[edges.Length];
            for (int i = 0; i < edges.Length; ++i)
                res[i] = edges[i].Item1;
            return res;
        }

        public int Cost(T from, T to)
        {
            var edges = Edges[from];
            for (int i = 0; i < edges.Length; ++i)
                if (edges[i].Item1!.Equals(to))
                    return edges[i].Item2;
            throw new InvalidOperationException("cost requested between nodes that do not share an edge");
        }
    }
}