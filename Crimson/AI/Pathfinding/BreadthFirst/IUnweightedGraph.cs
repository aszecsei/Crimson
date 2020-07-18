using System.Collections.Generic;

namespace Crimson.AI.Pathfinding
{
    public interface IUnweightedGraph<T>
    {
        /// <summary>
        /// Gets any neighbor nodes that can be reached from the passed-in node.
        /// </summary>
        IEnumerable<T> GetNeighbors(T node);
    }
}