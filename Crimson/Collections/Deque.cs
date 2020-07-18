using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Crimson.Collections
{
    // sourced from https://github.com/tejacques/Deque
    public class Deque<T> : IList<T>
    {
        private const int DEFAULT_CAPACITY = 16;
        
        private int _startOffset;
        private T[] _buffer;
        private int _capacityClosestPowerOfTwoMinusOne;
        
        public Deque() : this(DEFAULT_CAPACITY) {}

        public Deque(int capacity)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException(nameof(capacity), "capacity is less than 0");
            _buffer  = new T[0];
            Capacity = capacity;
        }

        public Deque(IEnumerable<T> collection) : this(collection.Count())
        {
            InsertRange(0, collection);
        }

        public int Capacity
        {
            get => _buffer.Length;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), "capacity is less than 0");
                else if (value < this.Count)
                    throw new InvalidOperationException("capacity cannot be set to a value less than Count");
                else if (_buffer != null && value == _buffer.Length)
                    return;

                int powOfTwo = Mathf.NextPowerOfTwo(value);

                value = powOfTwo;

                T[] newBuffer = new T[value];
                this.CopyTo(newBuffer, 0);

                _buffer = newBuffer;
                _startOffset = 0;
                _capacityClosestPowerOfTwoMinusOne = powOfTwo - 1;
            }
        }

        public bool IsFull => Count == Capacity;

        public bool IsEmpty => Count == 0;

        private void EnsureCapacityFor(int numElements)
        {
            if (Count + numElements > Capacity)
            {
                Capacity = Count + numElements;
            }
        }

        private int ToBufferIndex(int index)
        {
            return (index + _startOffset) & _capacityClosestPowerOfTwoMinusOne;
        }

        private void CheckIndexOutOfRange(int index)
        {
            if (index >= Count)
            {
                throw new IndexOutOfRangeException("index is greater than count");
            }
        }

        private static void CheckArgumentsOutOfRange(int length, int offset, int count)
        {
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), "invalid offset " + offset);
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), "invalid count " + count);
            if (length - offset < count)
                throw new ArgumentException(
                    $"Invalid offset ({offset}) or count + ({count}) for source length {length}");
        }

        private int ShiftStartOffset(int value)
        {
            _startOffset = ToBufferIndex(value);
            return _startOffset;
        }

        private int PreShiftStartOffset(int value)
        {
            int offset = _startOffset;
            ShiftStartOffset(value);
            return offset;
        }

        private int PostShiftStartOffset(int value)
        {
            return ShiftStartOffset(value);
        }

        #region IEnumerable

        public IEnumerator<T> GetEnumerator()
        {
            if (_startOffset + Count > Capacity)
            {
                for (int i = _startOffset; i < Capacity; ++i)
                {
                    yield return _buffer[i];
                }

                int endIndex = ToBufferIndex(Count);
                for (int i = 0; i < endIndex; ++i)
                {
                    yield return _buffer[i];
                }
            }
            else
            {
                int endIndex = _startOffset + Count;
                for (int i = _startOffset; i < endIndex; ++i)
                {
                    yield return _buffer[i];
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        
        #endregion

        #region ICollection

        public bool IsReadOnly => false;
        
        public int Count
        {
            get;
            private set;
        }

        private void IncrementCount(int value)
        {
            Count += value;
        }

        private void DecrementCount(int value)
        {
            Count = Mathf.Max(Count - value, 0);
        }

        public void Add(T item)
        {
            AddBack(item);
        }

        private void ClearBuffer(int logicalIndex, int length)
        {
            int offset = ToBufferIndex(logicalIndex);
            if (offset + length > Capacity)
            {
                int len = Capacity - offset;
                Array.Clear(_buffer, offset, len);
                len = ToBufferIndex(logicalIndex + length);
                Array.Clear(_buffer, 0, len);
            }
            else
            {
                Array.Clear(_buffer, offset, length);
            }
        }

        public void Clear()
        {
            if (Count > 0)
                ClearBuffer(0, Count);
            Count = 0;
            _startOffset = 0;
        }

        public bool Contains(T item)
        {
            return IndexOf(item) != -1;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array), "array is null");
            }

            if (_buffer == null)
            {
                return;
            }
            
            CheckArgumentsOutOfRange(array.Length, arrayIndex, Count);

            if (_startOffset != 0 && _startOffset + Count >= Capacity)
            {
                int lengthFromStart = Capacity - _startOffset;
                int lengthFromEnd = Count - lengthFromStart;
                
                Array.Copy(_buffer, _startOffset, array, 0, lengthFromStart);
                Array.Copy(_buffer, 0, array, lengthFromStart, lengthFromEnd);
            }
            else
            {
                Array.Copy(_buffer, _startOffset, array, 0, Count);
            }
        }

        public bool Remove(T item)
        {
            int index = IndexOf(item);

            if (index == -1)
            {
                return false;
            }
            else
            {
                RemoveAt(index);
                return true;
            }
        }

        #endregion

        #region List<T>

        public T this[int index]
        {
            get => Get(index);
            set => Set(index, value);
        }

        public void Insert(int index, T item)
        {
            EnsureCapacityFor(1);

            if (index == 0)
            {
                AddFront(item);
                return;
            }

            if (index == Count)
            {
                AddBack(item);
                return;
            }

            InsertRange(index, new[] {item});
        }

        public int IndexOf(T item)
        {
            int index = 0;
            foreach (var myItem in this)
            {
                if (myItem.Equals(item))
                    break;
                ++index;
            }

            if (index == Count)
                index = -1;
            
            return index;
        }

        public void RemoveAt(int index)
        {
            if (index == 0)
            {
                RemoveFront();
                return;
            }

            if (index == Count - 1)
            {
                RemoveBack();
                return;
            }

            RemoveRange(index, 1);
        }

        #endregion

        public void AddFront(T item)
        {
            EnsureCapacityFor(1);
            _buffer[PostShiftStartOffset(-1)] = item;
            IncrementCount(1);
        }

        public void AddBack(T item)
        {
            EnsureCapacityFor(1);
            _buffer[ToBufferIndex(Count)] = item;
            IncrementCount(1);
        }

        public T RemoveFront()
        {
            if (IsEmpty)
                throw new InvalidOperationException("The Deque is empty");

            T result = _buffer[_startOffset];
            _buffer[PreShiftStartOffset(1)] = default(T);
            DecrementCount(1);
            return result;
        }

        public T RemoveBack()
        {
            if (IsEmpty)
                throw new InvalidOperationException("The Deque is empty");
            
            DecrementCount(1);
            int endIndex = ToBufferIndex(Count);
            T result = _buffer[endIndex];
            _buffer[endIndex] = default(T);
            return result;
        }

        public void AddRange(IEnumerable<T> collection)
        {
            AddBackRange(collection);
        }

        public void AddFrontRange(IEnumerable<T> collection)
        {
            AddFrontRange(collection, 0, collection.Count());
        }

        public void AddFrontRange(IEnumerable<T> collection, int fromIndex, int count)
        {
            InsertRange(0, collection, fromIndex, count);
        }

        public void AddBackRange(IEnumerable<T> collection)
        {
            AddBackRange(collection, 0, collection.Count());
        }

        public void AddBackRange(IEnumerable<T> collection, int fromIndex, int count)
        {
            InsertRange(Count, collection, fromIndex, count);
        }

        public void InsertRange(int index, IEnumerable<T> collection)
        {
            InsertRange(index, collection, 0, collection.Count());
        }

        public void InsertRange(int index, IEnumerable<T> collection, int fromIndex, int count)
        {
            CheckIndexOutOfRange(index - 1);

            if (count == 0)
                return;
            
            EnsureCapacityFor(count);

            if (index < Count / 2)
            {
                // inserting into the first half of the list
                if (index > 0)
                {
                    // move items down:
                    // [0, index) ->
                    // [Capacity - count, Capacity - count + index)
                    int copyCount = index;
                    int shiftIndex = Capacity - count;
                    for (int j = 0; j < copyCount; ++j)
                    {
                        _buffer[ToBufferIndex(shiftIndex + j)] = _buffer[ToBufferIndex(j)];
                    }
                }

                ShiftStartOffset(-count);
            }
            else
            {
                // inserting into the second half of the list
                if (index < Count)
                {
                    // move items up:
                    // [index, Count) -> [index + count, count + Count)
                    int copyCount = Count - index;
                    int shiftIndex = index + count;
                    for (int j = 0; j < copyCount; ++j)
                    {
                        _buffer[ToBufferIndex(shiftIndex + j)] = _buffer[ToBufferIndex(index + j)];
                    }
                }
            }
            
            // Copy new items into place
            int i = index;
            foreach (T item in collection)
            {
                _buffer[ToBufferIndex(i)] = item;
                ++i;
            }
            
            // Adjust valid count
            IncrementCount(count);
        }
        
        public void RemoveRange(int index, int count)
        {
            if (this.IsEmpty)
            {
                throw new InvalidOperationException("The Deque is empty");
            }
            if (index > Count - count)
            {
                throw new IndexOutOfRangeException(
                    "The supplied index is greater than the Count");
            }

            // Clear out the underlying array
            ClearBuffer(index, count);

            if (index == 0)
            {
                // Removing from the beginning: shift the start offset
                ShiftStartOffset(count);
                Count -= count;
                return;
            }
            else if (index == Count - count)
            {
                // Removing from the ending: trim the existing view
                Count -= count;
                return;
            }

            if ((index + (count / 2)) < Count / 2)
            {
                // Removing from first half of list

                // Move items up:
                //  [0, index) -> [count, count + index)
                int copyCount = index;
                int writeIndex = count;
                for (int j = 0; j < copyCount; j++)
                {
                    _buffer[ToBufferIndex(writeIndex + j)]
                        = _buffer[ToBufferIndex(j)];
                }

                // Rotate to new view
                this.ShiftStartOffset(count);
            }
            else
            {
                // Removing from second half of list

                // Move items down:
                // [index + collectionCount, count) ->
                // [index, count - collectionCount)
                int copyCount = Count - count - index;
                int readIndex = index + count;
                for (int j = 0; j < copyCount; ++j)
                {
                    _buffer[ToBufferIndex(index + j)] =
                        _buffer[ToBufferIndex(readIndex + j)];
                }
            }

            // Adjust valid count
            DecrementCount(count);
        }
        
        public T Get(int index)
        {
            CheckIndexOutOfRange(index);
            return _buffer[ToBufferIndex(index)];
        }
        
        public void Set(int index, T item)
        {
            CheckIndexOutOfRange(index);
            _buffer[ToBufferIndex(index)] = item;
        }
    }
}