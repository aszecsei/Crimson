using System.Runtime.CompilerServices;

namespace Crimson
{
    public static class ArrayExt
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains<T>(this T[] array, T value)
        {
            return System.Array.IndexOf(array, value) >= 0;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInRange<T>(this T[] array, int index)
        {
            return index >= 0 && index < array.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] Array<T>(params T[] items)
        {
            return items;
        }

        public static T[] VerifyLength<T>(this T[] array, int length)
        {
            if (array == null) return new T[length];

            if (array.Length != length)
            {
                var newArray = new T[length];
                for (var i = 0; i < Mathf.Min(length, array.Length); i++) newArray[i] = array[i];

                return newArray;
            }

            return array;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[][] VerifyLength<T>(this T[][] array, int length0, int length1)
        {
            array = VerifyLength(array, length0);
            for (var i = 0; i < array.Length; i++) array[i] = VerifyLength(array[i], length1);

            return array;
        }
    }
}