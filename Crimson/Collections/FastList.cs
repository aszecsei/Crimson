using System;
using System.Collections;
using System.Collections.Generic;

namespace Crimson.Collections
{
    public class FastList<T> : IEnumerable<T>
    {
        public T[] Buffer;

        private const int DEFAULT_CAPACITY = 16;
        
        public int Length { get; private set; } = 0;

        public int Capacity
        {
            get => Buffer.Length;
            set
            {
                if (value >= Buffer.Length)
                {
                    int nextPowerOfTwo = Mathf.NextPowerOfTwo(value);
                    Array.Resize(ref Buffer, nextPowerOfTwo);
                }
            }
        }

        public FastList(int size)
        {
            int nextPowerOfTwo = Mathf.NextPowerOfTwo(size);
            Buffer = new T[nextPowerOfTwo];
        }

        public FastList() : this(DEFAULT_CAPACITY)
        {
        }

        public T this[int index]
        {
            get => Buffer[index];
            set => Buffer[index] = value;
        }

        public void Clear()
        {
            Array.Clear(Buffer, 0, Length);
        }

        public void Reset()
        {
            Length = 0;
        }

        public void Add(T item)
        {
            if (Length == Capacity)
                Capacity += 1;
            Buffer[Length++] = item;
        }

        public void Remove(T item)
        {
            var comp = EqualityComparer<T>.Default;
            for (var i = 0; i < Length; ++i)
            {
                if (comp.Equals(Buffer[i], item))
                {
                    RemoveAt(i);
                    return;
                }
            }
        }

        public void RemoveAt(int index)
        {
            Assert.IsTrue(index < Length, "index out of range");

            Length--;
            if (index < Length)
                Array.Copy(Buffer, index + 1, Buffer, index, Length - index);
            Buffer[Length] = default(T);
        }

        public void RemoveAtWithSwap(int index)
        {
            Assert.IsTrue(index < Length, "index out of range");

            Buffer[index] = Buffer[Length - 1];
            Buffer[Length - 1] = default(T);
            --Length;
        }

        public bool Contains(T item)
        {
            var comp = EqualityComparer<T>.Default;
            for (var i = 0; i < Length; ++i)
            {
                if (comp.Equals(Buffer[i], item))
                {
                    return true;
                }
            }

            return false;
        }

        public void EnsureCapacity(int additionalItemCount = 1)
        {
            if (Length + additionalItemCount > Capacity)
                Capacity = Length + additionalItemCount;
        }

        public void AddRange(IEnumerable<T> array)
        {
            EnsureCapacity(array.Count());
            foreach (var item in array)
            {
                Buffer[Length++] = item;
            }
        }

        public void Sort()
        {
            Array.Sort(Buffer, 0, Length);
        }

        public void Sort(IComparer comparer)
        {
            Array.Sort(Buffer, 0, Length, comparer);
        }

        public void Sort(IComparer<T> comparer)
        {
            Array.Sort(Buffer, 0, Length, comparer);
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < Length; ++i)
                yield return Buffer[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}