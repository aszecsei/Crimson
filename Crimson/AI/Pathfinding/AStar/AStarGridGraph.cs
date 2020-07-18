using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Crimson.AI.Pathfinding
{
    public class AStarGridGraph : IAStarGraph<Point>
    {
        private static readonly Point[] CardinalDirs = {
            new Point(1, 0),
            new Point(0, -1),
            new Point(-1, 0),
            new Point(0, 1)
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
        public Dictionary<Point, int> WeightedNodes = new Dictionary<Point, int>();
        public int DefaultWeight = 1;

        private readonly int _width, _height;
        private Point[] _dirs;
        private readonly List<Point> _neighbors = new List<Point>(4);

        public AStarGridGraph(int width, int height, bool allowDiagonalSearch = false)
        {
            _width = width;
            _height = height;
            _dirs = allowDiagonalSearch ? CompassDirs : CardinalDirs;
        }

        private bool IsNodeInBounds(Point node)
        {
            return 0 <= node.X && node.X < _width && 0 <= node.Y && node.Y < _height;
        }

        private bool IsNodePassable(Point node) => !Walls.Contains(node);

        public List<Point>? Search(Point start, Point goal) => AStarPathfinder.Search(this, start, goal);

        #region IAStarGraph implementation

        public IEnumerable<Point> GetNeighbors(Point node)
        {
            _neighbors.Clear();

            foreach (var (x, y) in _dirs)
            {
                var next = new Point(node.X + x, node.Y + y);
                if (IsNodeInBounds(next) && IsNodePassable(next))
                    _neighbors.Add(next);
            }

            return _neighbors;
        }

        public int Cost(Point @from, Point to)
        {
            return WeightedNodes.ContainsKey(to) ? WeightedNodes[to] : DefaultWeight;
        }

        public int Heuristic(Point node, Point goal)
        {
            var dx = Mathf.Abs(node.X - goal.X);
            var dy = Mathf.Abs(node.Y - goal.Y);
            if (_dirs == CardinalDirs)
                return dx + dy;
            else
                return Mathf.Max(dx, dy);
        }

        #endregion
    }
}