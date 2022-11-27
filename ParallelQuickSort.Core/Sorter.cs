namespace ParallelQuickSort.Core;

public class Sorter
{
    private const int MaxParallelDepth = 3;
    private const int PartitionThreshold = 10000;
    
    public static void QuickSort(int[] arr)
    {
        QuickSort(arr, 0, arr.Length - 1);
    }

    public static Task NaiveParallelQuickSort(int[] arr)
    {
        return QuickSort(arr, 0, arr.Length - 1, 1);
    }
    
    private static void QuickSort(int[] arr, int l, int r)
    {
        if (r <= l)
            return;

        var (lBound, rBound) = Partition(arr, l, r);

        QuickSort(arr, l, lBound - 1);
        QuickSort(arr, rBound + 1, r);
    }

    private static async Task QuickSort(int[] arr, int l, int r, int depth)
    {
        if (r <= l)
            return;

        var (lBound, rBound) = r - l < PartitionThreshold
            ? Partition(arr, l, r)
            : await ParallelPartition(arr, l, r);

        var left = depth == MaxParallelDepth
            ? QuickSort(arr, l, lBound - 1, depth)
            : Task.Run(() => QuickSort(arr, l, lBound - 1, depth + 1));
        var right = QuickSort(arr, rBound + 1, r, depth);

        await Task.WhenAll(left, right);
    }
    
    private static (int lBound, int rBound) Partition(int[] arr, int l, int r)
    {
        SetPivot(arr, l, r);
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

    private static async Task<(int lBound, int rBound)> ParallelPartition(int[] arr, int l, int r)
    {
        SetPivot(arr, l, r);
        int pivot = arr[l];

        var less = Task.Run(() => Filter(arr, l, r, x => x < pivot));
        var greater = Task.Run(() => Filter(arr, l, r, x => x > pivot));
        var same = Filter(arr, l, r, x => x == pivot);

        var res = await Task.WhenAll(less, greater);

        var sameStart = l + res[0].Length;
        var sameFinish = sameStart + same.Length;
        
        var lessCopy = Task.Run(() => res[0].CopyTo(arr, l));
        var greaterCopy  = Task.Run(() => res[1].CopyTo(arr, sameFinish));
        same.CopyTo(arr, sameStart);

        await Task.WhenAll(lessCopy, greaterCopy);

        return (sameStart, sameFinish - 1);
    }

    private static void SetPivot(int[] arr, int l, int r)
    {
        int mid = l + (r - l) / 2;

        Swap(arr, mid, l + 1);

        if (arr[l] > arr[r])
            Swap(arr, l, r);
        if (arr[l + 1] > arr[r])
            Swap(arr, l + 1, r);
        if (arr[l] > arr[l + 1])
            Swap(arr, l, l + 1);
    }

    private static void Swap(int[] arr, int first, int second)
    {
        (arr[first], arr[second]) = (arr[second], arr[first]);
    }

    private static int[] Filter(int[] arr, int l, int r, Func<int, bool> pred)
    {
        var list = new List<int>(PartitionThreshold);

        for (int i = l; i <= r; i++)
        {
            if (pred(arr[i]))
            {
                list.Add(arr[i]);
            }
        }

        return list.ToArray();
    }
}