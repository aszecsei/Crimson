using System.Collections.Generic;

namespace Crimson.Collections
{
    /// <summary>
    /// Implements a directed graph.
    /// </summary>
    public class DirectedGraph<T>
        where T : notnull
    {
        private class Vertex
        {
            public T Data;
            public FastList<Vertex> Connections;
            public bool Visited;

            public Vertex(T data)
            {
                Data = data;
                Connections = new FastList<Vertex>();
                Visited = false;
            }
        }

        private Dictionary<T, Vertex> _vertices;
        private FastList<T> _result;

        public DirectedGraph()
        {
            _vertices = new Dictionary<T, Vertex>();
            _result = new FastList<T>();
        }

        public void AddNode(T node)
        {
            if (_vertices.ContainsKey(node)) return;
            _vertices[node] = new Vertex(node);
        }

        /// <summary>
        /// Returns whether or not a vertex exists within the graph.
        /// </summary>
        public bool Contains(T node) => _vertices.ContainsKey(node);

        /// <summary>
        /// Add an edge from <paramref name="from"/> to <paramref name="to"/>. Requires that vertices for both values
        /// already exist in the graph!
        /// </summary>
        public void AddEdge(T from, T to)
        {
            var toVertex = _vertices[to];
            _vertices[from].Connections.Add(toVertex);
        }

        /// <summary>
        /// Removes all edges from <paramref name="from"/>. Requires that a vertex for that value already exists in the
        /// graph.
        /// </summary>
        public void ClearEdges(T from)
        {
            _vertices[from].Connections.Reset();
        }

        /// <summary>
        /// Performs Tarjan's strongly connected components algorithm on the graph to construct a topological sort. Although
        /// Tarjan's algorithm returns a reverse topological sort, the return value is a <b>proper</b> topological sort.
        /// </summary>
        public FastList<T> Tarjan()
        {
            var stack = Pool<Stack<Vertex>>.Obtain();
            
            foreach (var vertex in _vertices.Values)
            {
                vertex.Visited = false;
            }

            foreach (var vertex in _vertices.Values)
            {
                if (vertex.Visited) continue;
                StronglyConnected(vertex);
            }
            
            _result.EnsureCapacity(stack.Count);
            _result.Reset();
            for (var i = 0; i < _result.Length; ++i)
            {
                _result[i] = stack.Pop().Data;
            }
            
            Pool<Stack<Vertex>>.Free(stack);

            return _result;

            void StronglyConnected(Vertex v)
            {
                v.Visited = true;

                for (var i = 0; i < v.Connections.Length; ++i)
                {
                    var neighbor = v.Connections[i];
                    if (neighbor.Visited) continue;
                    StronglyConnected(neighbor);
                }
                
                stack.Push(v);
            }
        }
    }
}