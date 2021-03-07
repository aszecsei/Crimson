using Microsoft.Xna.Framework;

namespace Crimson
{
    public class VirtualMap<T>
    {
        public const int SEGMENT_SIZE = 50;
        public readonly int Columns;
        public readonly T EmptyValue;
        public readonly int Rows;
        public readonly int SegmentColumns;
        public readonly int SegmentRows;

        private readonly T[,][,] segments;

        public VirtualMap(int columns, int rows, T emptyValue = default)
        {
            Columns = columns;
            Rows = rows;
            SegmentColumns = columns / SEGMENT_SIZE + 1;
            SegmentRows = rows / SEGMENT_SIZE + 1;
            segments = new T[SegmentColumns, SegmentRows][,];
            EmptyValue = emptyValue;
        }

        public VirtualMap(T[,] map, T emptyValue = default) : this(map.GetLength(0), map.GetLength(1), emptyValue)
        {
            for (var x = 0; x < Columns; x++)
            for (var y = 0; y < Rows; y++)
                this[x, y] = map[x, y];
        }

        public T this[int x, int y]
        {
            get
            {
                var cx = x / SEGMENT_SIZE;
                var cy = y / SEGMENT_SIZE;

                var seg = segments[cx, cy];
                if (seg == null) return EmptyValue;

                return seg[x - cx * SEGMENT_SIZE, y - cy * SEGMENT_SIZE];
            }

            set
            {
                var cx = x / SEGMENT_SIZE;
                var cy = y / SEGMENT_SIZE;
                if (segments[cx, cy] == null)
                {
                    segments[cx, cy] = new T[SEGMENT_SIZE, SEGMENT_SIZE];

                    // fill with custom empty value data
                    if (EmptyValue != null && !EmptyValue.Equals(default(T)))
                        for (var tx = 0; tx < SEGMENT_SIZE; tx++)
                        for (var ty = 0; ty < SEGMENT_SIZE; ty++)
                            segments[cx, cy][tx, ty] = EmptyValue;
                }

                segments[cx, cy][x - cx * SEGMENT_SIZE, y - cy * SEGMENT_SIZE] = value;
            }
        }

        public bool AnyInSegmentAtTile(int x, int y)
        {
            var cx = x / SEGMENT_SIZE;
            var cy = y / SEGMENT_SIZE;
            return segments[cx, cy] != null;
        }

        public bool AnyInSegment(int segmentX, int segmentY)
        {
            return segments[segmentX, segmentY] != null;
        }

        public T InSegment(int segmentX, int segmentY, int x, int y)
        {
            return segments[segmentX, segmentY][x, y];
        }

        public T[,] GetSegment(int segmentX, int segmentY)
        {
            return segments[segmentX, segmentY];
        }

        public T SafeCheck(int x, int y)
        {
            if (x >= 0 && y >= 0 && x < Columns && y < Rows) return this[x, y];

            return EmptyValue;
        }

        public T[,] ToArray()
        {
            var array = new T[Columns, Rows];
            for (var x = 0; x < Columns; x++)
            for (var y = 0; y < Rows; y++)
                array[x, y] = this[x, y];

            return array;
        }

        public VirtualMap<T> Clone()
        {
            var clone = new VirtualMap<T>(Columns, Rows, EmptyValue);
            for (var x = 0; x < Columns; x++)
            for (var y = 0; y < Rows; y++)
                clone[x, y] = this[x, y];

            return clone;
        }
    }

    public static class VirtualMapExt
    {
        public struct RaycastCollisionData
        {
            public Vector2 Location;
            public Point   Tile;
        }

        public static bool Raycast(this VirtualMap<bool> solids, Vector2 start, Vector2 end, out RaycastCollisionData collisionData, float maxDistance = Mathf.INFINITY)
        {
            Vector2 dir  = end - start;
            float   dydx = dir.Y / dir.X;
            float   dxdy = dir.X / dir.Y;
            float   sx   = Mathf.Sqrt(1 + dydx * dydx);
            float   sy   = Mathf.Sqrt(1 + dxdy * dxdy);

            Point   mapCheck    = new Point(Mathf.FloorToInt(start.X), Mathf.FloorToInt(start.Y));
            Vector2 rayLength1D = Vector2.Zero;
            Point   step        = new Point(Mathf.Sign(dir.X), Mathf.Sign(dir.Y));
            if ( dir.X < 0 )
            {
                rayLength1D.X = (start.X - mapCheck.X) * sx;
            }
            else
            {
                rayLength1D.X = (mapCheck.X + 1 - start.X) * sx;
            }

            if ( dir.Y < 0 )
            {
                rayLength1D.Y = (start.Y - mapCheck.Y) * sy;
            }
            else
            {
                rayLength1D.Y = (mapCheck.Y + 1 - start.Y) * sy;
            }

            bool        tileFound   = false;
            float       distance    = 0f;
            while ( !tileFound && distance < maxDistance )
            {
                if ( rayLength1D.X < rayLength1D.Y )
                {
                    mapCheck.X    += step.X;
                    distance      =  rayLength1D.X;
                    rayLength1D.X += sx;
                }
                else
                {
                    mapCheck.Y    += step.Y;
                    distance      =  rayLength1D.Y;
                    rayLength1D.Y += sy;
                }

                if ( solids.SafeCheck(mapCheck.X, mapCheck.Y) )
                {
                    tileFound = true;
                }
                else if ( mapCheck.X < 0 || mapCheck.Y < 0 || mapCheck.X >= solids.Columns ||
                          mapCheck.Y >= solids.Rows )
                {
                    break;
                }
            }

            if ( tileFound )
            {
                collisionData.Location = start + dir.SafeNormalize(distance);
                collisionData.Tile = new Point(Mathf.FloorToInt(collisionData.Location.X), Mathf.FloorToInt(collisionData.Location.Y));
                return true;
            }
            else
            {
                collisionData.Location = Vector2.Zero;
                collisionData.Tile     = Point.Zero;
                return false;
            }
        }
    }
}
