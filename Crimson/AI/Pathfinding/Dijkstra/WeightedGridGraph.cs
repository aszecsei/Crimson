using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Crimson.AI.Pathfinding
{
    /// <summary>
	/// basic grid graph with support for one type of weighted node
	/// </summary>
	public class WeightedGridGraph : IWeightedGraph<Point>
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

		private int _width, _height;
		private Point[] _dirs;
		private List<Point> _neighbors = new List<Point>(4);


		public WeightedGridGraph(int width, int height, bool allowDiagonalSearch = false)
		{
			_width = width;
			_height = height;
			_dirs = allowDiagonalSearch ? CompassDirs : CardinalDirs;
		}

		/// <summary>
		/// ensures the node is in the bounds of the grid graph
		/// </summary>
		/// <returns><c>true</c>, if node in bounds was ised, <c>false</c> otherwise.</returns>
		bool IsNodeInBounds(Point node)
		{
			return 0 <= node.X && node.X < _width && 0 <= node.Y && node.Y < _height;
		}

		/// <summary>
		/// checks if the node is passable. Walls are impassable.
		/// </summary>
		/// <returns><c>true</c>, if node passable was ised, <c>false</c> otherwise.</returns>
		public bool IsNodePassable(Point node) => !Walls.Contains(node);

		/// <summary>
		/// convenience shortcut for calling AStarPathfinder.search
		/// </summary>
		public List<Point> Search(Point start, Point goal) => WeightedPathfinder.Search(this, start, goal);

		#region IWeightedGraph implementation

		public IEnumerable<Point> GetNeighbors(Point node)
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

		int IWeightedGraph<Point>.Cost(Point from, Point to)
		{
			return WeightedNodes.ContainsKey(to) ? WeightedNodes[to] : DefaultWeight;
		}

		#endregion
	}
}