using System.Collections;
using System.Collections.Generic;

namespace Crimson
{
    public static class IEnumerableExt
    {
        public static int Count<TSource>(this IEnumerable<TSource> source)
        {
            Assert.IsNotNull(source, "source cannot be null");

            if (source is ICollection<TSource> genericCollection)
            {
                return genericCollection.Count;
            }

            if (source is ICollection nonGenericCollection)
            {
                return nonGenericCollection.Count;
            }

            checked
            {
                int count = 0;
                using (var iterator = source.GetEnumerator())
                {
                    while (iterator.MoveNext())
                        count++;
                }

                return count;
            }
        }
    }
}