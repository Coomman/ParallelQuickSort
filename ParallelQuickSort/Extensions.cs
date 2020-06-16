using System;

namespace ParallelQuickSort
{
    public static class Extensions
    {
        public static bool CheckSort<T>(this T[] arr) where  T : IComparable
        {
            if (arr is null || arr.Length < 2)
                return true;

            for (int i = 0; i < arr.Length - 1; i++)
            {
                if (arr[i].CompareTo(arr[i + 1]) > 0)
                    return false;
            }

            return true;
        }
        public static void Swap<T>(this T[] arr, int first, int second)
        {
            var temp = arr[first];
            arr[first] = arr[second];
            arr[second] = temp;
        }

        public static void Write(this ConsoleColor newColor, string str)
        {
            var oldColor = Console.ForegroundColor;

            Console.ForegroundColor = newColor;
            Console.Write(str);
            Console.ForegroundColor = oldColor;
        }
        public static void WriteLine(this ConsoleColor newColor, string str)
        {
            var oldColor = Console.ForegroundColor;

            Console.ForegroundColor = newColor;
            Console.WriteLine(str);
            Console.ForegroundColor = oldColor;
        }
    }
}
