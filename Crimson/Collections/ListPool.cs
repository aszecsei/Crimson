using System.Collections.Generic;

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

        public static void ClearCache()
        {
            s_objectQueue.Clear();
        }

        public static List<T> Obtain()
        {
            if (s_objectQueue.Count > 0)
                return s_objectQueue.Dequeue();
            return new List<T>();
        }

        public static void Free(List<T> obj)
        {
            s_objectQueue.Enqueue(obj);
            obj.Clear();
        }
    }
}