using System.Collections.Generic;

namespace Crimson.Collections
{
    /// <summary>
    /// Implements a directed graph.
    /// </summary>
    public class DirectedGraph<T>
        where T : notnull
    {
        private struct Vertex
        {
            public T Data;
            public HashSet<Vertex> Connections;

            public Vertex(T data)
            {
                Data = data;
                Connections = new HashSet<Vertex>();
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

        public void AddEdge(T from, T to)
        {
            if (!_vertices.ContainsKey(from)) _vertices[from] = new Vertex(from);
            if (!_vertices.ContainsKey(to)) _vertices[to] = new Vertex(to);

            var toVertex = _vertices[to];
            _vertices[from].Connections.Add(toVertex);
        }

        public FastList<T> Tarjan()
        {
            var stack = new Stack<Vertex>();
            var visited = new HashSet<Vertex>();

            foreach (var vertex in _vertices.Values)
            {
                if (visited.Contains(vertex)) continue;
                StronglyConnected(vertex);
            }
            
            _result.EnsureCapacity(stack.Count);
            for (var i = 0; i < _result.Length; ++i)
            {
                _result[i] = stack.Pop().Data;
            }

            return _result;

            void StronglyConnected(Vertex v)
            {
                visited.Add(v);

                foreach (var neighbor in v.Connections)
                {
                    if (visited.Contains(neighbor)) continue;
                    StronglyConnected(neighbor);
                }
                
                stack.Push(v);
            }
        }
    }
}