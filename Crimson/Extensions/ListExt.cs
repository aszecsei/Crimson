using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Crimson
{
    public static class ListExt
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInRange<T>(this List<T> list, int index)
        {
            return index >= 0 && index < list.Count;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AddIfNotPresent<T>(this IList<T> list, T item)
        {
            if (list.Contains(item))
                return false;
            list.Add(item);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T FirstItem<T>(this IList<T> list)
        {
            return list[0];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T LastItem<T>(this IList<T> list)
        {
            return list[list.Count - 1];
        }
        
        public static void Shuffle<T>(this IList<T> list, Random random)
        {
            var i = list.Count;
            int j;
            T t;

            while (--i > 0)
            {
                t = list[i];
                list[i] = list[j = random.Next(i + 1)];
                list[j] = t;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Shuffle<T>(this IList<T> list)
        {
            list.Shuffle(Utils.Random);
        }

        public static void ShuffleSetFirst<T>(this IList<T> list, Random random, T first)
        {
            var amount = 0;
            while (list.Contains(first))
            {
                list.Remove(first);
                amount++;
            }

            list.Shuffle(random);

            for (var i = 0; i < amount; i++) list.Insert(0, first);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ShuffleSetFirst<T>(this IList<T> list, T first)
        {
            list.ShuffleSetFirst(Utils.Random, first);
        }

        public static void ShuffleNotFirst<T>(this IList<T> list, Random random, T notFirst)
        {
            var amount = 0;
            while (list.Contains(notFirst))
            {
                list.Remove(notFirst);
                amount++;
            }

            list.Shuffle(random);

            for (var i = 0; i < amount; i++) list.Insert(random.Next(list.Count - 1) + 1, notFirst);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ShuffleNotFirst<T>(this IList<T> list, T notFirst)
        {
            list.ShuffleNotFirst(Utils.Random, notFirst);
        }
    }
}