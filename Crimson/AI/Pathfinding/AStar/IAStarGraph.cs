using System.Collections.Generic;

namespace Crimson.AI.Pathfinding
{
    public interface IAStarGraph<T> : IWeightedGraph<T>
    {
        /// <summary>
        /// Calculates the heuristic/estimate to get from 'node' to 'goal'.
        /// <seealso cref="WeightedGridGraph"/> for the common Manhattan method.
        /// </summary>
        int Heuristic(T node, T goal);
    }
}