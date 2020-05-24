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
}