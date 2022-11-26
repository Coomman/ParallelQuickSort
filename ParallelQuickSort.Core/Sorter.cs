namespace ParallelQuickSort.Core;

public class Sorter
{
    public void QuickSort(int[] arr)
    {
        QuickSortCore(arr, 0, arr.Length - 1);
    }
    
    private void QuickSortCore(int[] arr, int l, int r)
    {
        if (r <= l)
            return;

        var (lBound, rBound) = Partition(arr, l, r);

        QuickSortCore(arr, l, lBound - 1);
        QuickSortCore(arr, rBound + 1, r);
    }
    
    private (int lBound, int rBound) Partition(int[] arr, int l, int r)
    {
        int pivot = arr[l];
        int cur = l + 1;

        while (cur <= r)
        {
            int cmp = arr[cur].CompareTo(pivot);

            if (cmp < 0)
                Swap(arr, l++, cur++);
            else if (cmp > 0)
                Swap(arr, cur, r--);
            else
                cur++;
        }

        return (l, r);
    }

    private static void Swap(int[] arr, int first, int second)
    {
        (arr[first], arr[second]) = (arr[second], arr[first]);
    }
}