using System.Collections.Generic;

namespace Crimson.Collections
{
    /// <summary>
    /// Simple static class that can be used to pool any object
    /// </summary>
    public static class Pool<T> where T : new()
    {
        private static Queue<T> _queue = new Queue<T>(16);

        /// <summary>
        /// Warms up the cache, filling it with a max of <see cref="cacheCount"/> objects
        /// </summary>
        /// <param name="cacheCount"></param>
        public static void WarmCache(int cacheCount)
        {
            cacheCount -= _queue.Count;
            if (cacheCount > 0)
            {
                for (var i = 0; i < cacheCount; ++i)
                    _queue.Enqueue(new T());
            }
        }

        /// <summary>
        /// Trims the cache down to <see cref="cacheCount"/> items.
        /// </summary>
        public static void TrimCache(int cacheCount)
        {
            while (cacheCount > _queue.Count)
                _queue.Dequeue();
        }

        /// <summary>
        /// Clears out the cache
        /// </summary>
        public static void ClearCache()
        {
            _queue.Clear();
        }

        /// <summary>
        /// Pops an item off the stack if available, creating a new item as necessary
        /// </summary>
        public static T Obtain()
        {
            if (_queue.Count > 0)
                return _queue.Dequeue();
            return new T();
        }

        /// <summary>
        /// Pushes an item back on the stack
        /// </summary>
        public static void Free(T obj)
        {
            _queue.Enqueue(obj);
            if (obj is IPoolable)
            {
                ((IPoolable) obj).Reset();
            }
        }
    }

    public interface IPoolable
    {
        /// <summary>
        /// Resets the object for reuse. Object references should be nulled and fields may be set to default values.
        /// </summary>
        void Reset();
    }
}