using System.Collections.Generic;

namespace Crimson.AI.Pathfinding
{
    public interface IWeightedGraph<T> : IUnweightedGraph<T>
    {
        /// <summary>
        /// Calculates the cost to get from 'from' to 'to'
        /// </summary>
        int Cost(T from, T to);
    }
}