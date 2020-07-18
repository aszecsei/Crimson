using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Crimson.AI.Pathfinding
{
    public class UnweightedGridGraph : IUnweightedGraph<Point>
    {
        private static readonly Point[] CardinalDirs = {
            new Point(1, 0),
            new Point(0, -1),
            new Point(-1, 0),
            new Point(0, 1),
        };

        private static readonly Point[] CompassDirs = {
            new Point(1, 0),
            new Point(1, -1),
            new Point(0, -1),
            new Point(-1, -1),
            new Point(-1, 0),
            new Point(-1, 1),
            new Point(0, 1),
            new Point(1, 1),
        };

        public HashSet<Point> Walls = new HashSet<Point>();

        private readonly int _width, _height;
        private readonly Point[] _dirs;
        private readonly List<Point> _neighbors = new List<Point>(4);

        public UnweightedGridGraph(int width, int height, bool allowDiagonalSearch = false)
        {
            _width = width;
            _height = height;
            _dirs = allowDiagonalSearch ? CompassDirs : CardinalDirs;
        }

        public bool IsNodeInBounds(Point node)
        {
            return 0 <= node.X && node.X < _width && 0 <= node.Y && node.Y < _height;
        }

        public bool IsNodePassable(Point node) => !Walls.Contains(node);

        IEnumerable<Point> IUnweightedGraph<Point>.GetNeighbors(Point node)
        {
            _neighbors.Clear();

            foreach (var dir in _dirs)
            {
                var next = new Point(node.X + dir.X, node.Y + dir.Y);
                if (IsNodeInBounds(next) && IsNodePassable(next))
                    _neighbors.Add(next);
            }

            return _neighbors;
        }

        /// <summary>
        /// convenience shortcut for calling BreadthFirstPathfinder.search
        /// </summary>
        public List<Point>? Search(Point start, Point goal) => BreadthFirstPathfinder.Search(this, start, goal);
    }
}