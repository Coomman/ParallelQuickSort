using System;
using System.Linq;

namespace ParallelQuickSort
{
    public static class SortHelper
    {
        private static Random Rand { get; } = new Random();

        public static int[] GenerateArray(int count)
        {
            return Enumerable.Repeat(0, count)
                .Select(num => Rand.Next(int.MinValue, int.MaxValue))
                .ToArray();
        }
    }
}
