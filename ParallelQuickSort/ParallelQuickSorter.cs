using System;
using System.Threading.Tasks;

namespace ParallelQuickSort
{
    public class ParallelQuickSorter<T> where T : IComparable
    {
        private int Cores { get; } = Environment.ProcessorCount;
        public int InsertionSortBound { get; set; } = 80;

        private static void InsertionSort(T[] arr, int left, int right)
        {
            if (right - left < 1)
                return;

            for (int i = left + 1; i <= right; i++)
                for (int j = i - 1; j >= 0; j--)
                    if (arr[j + 1].CompareTo(arr[j]) < 0)
                        arr.Swap(j, j + 1);
                    else
                        break;
        }
        private static (int leftBound, int rightBound) Partition(T[] arr, int left, int right)
        {
            int l = left;
            int r = right;
            var pivot = arr[left];
            int cur = left + 1;

            while (cur <= r)
            {
                int cmp = arr[cur].CompareTo(pivot);

                if (cmp < 0)
                    arr.Swap(l++, cur++);
                else if (cmp > 0)
                    arr.Swap(cur, r--);
                else
                    cur++;
            }

            return (l, r);
        }

        private async Task ParallelQuickSort(T[] arr, int left, int right)
        {
            if (right <= left)
                return;

            if (right - left + 1 <= InsertionSortBound)
            {
                InsertionSort(arr, left, right);
                return;
            }

            var (leftBound, rightBound) = Partition(arr, left, right);

            var leftPart = Task.Run(() => ParallelQuickSort(arr, left, leftBound - 1));
            var rightPart = Task.Run(() => ParallelQuickSort(arr, rightBound + 1, right));

            await Task.WhenAll(leftPart, rightPart).ConfigureAwait(false);
        }
        public async Task QuickSortAsync(T[] arr)
        {
            await ParallelQuickSort(arr, 0, arr.Length - 1);
        }

        private void QuickSort(T[] arr, int left, int right)
        {
            if (right <= left)
                return;

            if (right - left + 1 <= InsertionSortBound)
            {
                InsertionSort(arr, left, right);
                return;
            }

            var (leftBound, rightBound) = Partition(arr, left, right);

            QuickSort(arr, left, leftBound - 1);
            QuickSort(arr, rightBound + 1, right);
        }
        public void QuickSort(T[] arr)
        {
            QuickSort(arr, 0, arr.Length - 1);
        }
    }
}