namespace Crimson
{
    public class VirtualMap3<T>
    {
        public const int SEGMENT_SIZE = 50;
        public readonly T EmptyValue;
        public readonly int Columns;
        public readonly int Rows;
        public readonly int Slices;
        public readonly int SegmentColumns;
        public readonly int SegmentRows;
        public readonly int SegmentSlices;

        private readonly T[,,][,,] _segments;

        public VirtualMap3(int columns, int rows, int slices, T emptyValue = default)
        {
            Columns = columns;
            Rows = rows;
            Slices = slices;
            SegmentColumns = columns / SEGMENT_SIZE + 1;
            SegmentRows = rows / SEGMENT_SIZE + 1;
            SegmentSlices = slices / SEGMENT_SIZE + 1;
            _segments = new T[SegmentColumns, SegmentRows, SegmentSlices][,,];
            EmptyValue = emptyValue;
        }

        public VirtualMap3(T[,,] map, T emptyValue = default) : this(map.GetLength(0), map.GetLength(1), map.GetLength(2), emptyValue)
        {
            for (var x = 0; x < Columns; x++)
                for (var y = 0; y < Rows; y++)
                    for (var z = 0; z < Slices; z++)
                        this[x, y, z] = map[x, y, z];
        }

        public T this[int x, int y, int z]
        {
            get
            {
                var cx = x / SEGMENT_SIZE;
                var cy = y / SEGMENT_SIZE;
                var cz = z / SEGMENT_SIZE;

                var seg = _segments[cx, cy, cz];
                if (seg == null) return EmptyValue;

                return seg[x - cx * SEGMENT_SIZE, y - cy * SEGMENT_SIZE, z - cz * SEGMENT_SIZE];
            }

            set
            {
                var cx = x / SEGMENT_SIZE;
                var cy = y / SEGMENT_SIZE;
                var cz = z / SEGMENT_SIZE;
                if (_segments[cx, cy, cz] == null)
                {
                    _segments[cx, cy, cz] = new T[SEGMENT_SIZE, SEGMENT_SIZE, SEGMENT_SIZE];

                    // fill with custom empty value data
                    if (EmptyValue != null && !EmptyValue.Equals(default(T)))
                        for (var tx = 0; tx < SEGMENT_SIZE; tx++)
                            for (var ty = 0; ty < SEGMENT_SIZE; ty++)
                                for (var tz = 0; tz < SEGMENT_SIZE; tz++)
                                    _segments[cx, cy, cz][tx, ty, tz] = EmptyValue;
                }

                _segments[cx, cy, cz][x - cx * SEGMENT_SIZE, y - cy * SEGMENT_SIZE, z - cz * SEGMENT_SIZE] = value;
            }
        }

        public bool AnyInSegmentAtTile(int x, int y, int z)
        {
            var cx = x / SEGMENT_SIZE;
            var cy = y / SEGMENT_SIZE;
            var cz = z / SEGMENT_SIZE;
            return _segments[cx, cy, cz] != null;
        }

        public bool AnyInSegment(int segmentX, int segmentY, int segmentZ)
        {
            return _segments[segmentX, segmentY, segmentZ] != null;
        }

        public T InSegment(int segmentX, int segmentY, int segmentZ, int x, int y, int z)
        {
            return _segments[segmentX, segmentY, segmentZ][x, y, z];
        }

        public T[,,] GetSegment(int segmentX, int segmentY, int segmentZ)
        {
            return _segments[segmentX, segmentY, segmentZ];
        }

        public T SafeCheck(int x, int y, int z)
        {
            if (x >= 0 && y >= 0 && z >= 0 && x < Columns && y < Rows && z < Slices) return this[x, y, z];

            return EmptyValue;
        }

        public bool IsInBounds(int x, int y, int z)
        {
            return (x >= 0 && y >= 0 && z >= 0 && x < Columns && y < Rows && z < Slices);
        }

        public T[,,] ToArray()
        {
            var array = new T[Columns, Rows, Slices];
            for (var x = 0; x < Columns; x++)
                for (var y = 0; y < Rows; y++)
                    for (var z = 0; z < Slices; z++)
                        array[x, y, z] = this[x, y, z];

            return array;
        }

        public VirtualMap3<T> Clone()
        {
            var clone = new VirtualMap3<T>(Columns, Rows, Slices, EmptyValue);
            for (var x = 0; x < Columns; x++)
                for (var y = 0; y < Rows; y++)
                    for (var z = 0; z < Slices; z++)
                        clone[x, y, z] = this[x, y, z];

            return clone;
        }
    }
}