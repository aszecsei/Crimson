using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Crimson.Collections
{
    public static class ListPool<T>
    {
        private static readonly Queue<List<T>> s_objectQueue = new Queue<List<T>>();

        public static void WarmCache(int cacheCount)
        {
            cacheCount -= s_objectQueue.Count;
            if (cacheCount > 0)
            {
                for (var i = 0; i < cacheCount; ++i)
                    s_objectQueue.Enqueue(new List<T>());
            }
        }

        public static void TrimCache(int cacheCount)
        {
            cacheCount = s_objectQueue.Count - cacheCount;
            if (cacheCount > 0)
            {
                for (var i = 0; i < cacheCount; ++i)
                    s_objectQueue.Dequeue();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ClearCache()
        {
            s_objectQueue.Clear();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static List<T> Obtain()
        {
            if (s_objectQueue.Count > 0)
                return s_objectQueue.Dequeue();
            return new List<T>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Free(List<T> obj)
        {
            s_objectQueue.Enqueue(obj);
            obj.Clear();
        }

        /// <summary>
        /// Frees the list and returns an array with the list's contents.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] FreeToArray(List<T> obj)
        {
            T[] arr = obj.ToArray();
            Free(obj);
            return arr;
        }
    }
}