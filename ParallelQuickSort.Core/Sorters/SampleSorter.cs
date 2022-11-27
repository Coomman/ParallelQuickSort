namespace ParallelQuickSort.Core.Sorters;

public static class SampleSorter
{
    private const int Parallelism = 4;

    public static async Task Sort(int[] arr)
    {
        var bounds = GetBounds(0, arr.Length - 1);

        var samples = (await Task.WhenAll(bounds
                .Select(x => Task.Run(() => QuickSortWithSamples(arr, x.l, x.r)))))
            .SelectMany(x => x)
            .ToArray();

        var pivots = QuickSortWithSamples(samples, 0, samples.Length - 1).ToArray();

        var batches = (await Task.WhenAll(bounds
                .Select(x => Task.Run(() => MultiPartition(arr, x.l, x.r, pivots)))))
            .ToArray();

        var groups = Enumerable.Range(0, pivots.Length + 1)
            .Select(i => batches.Select(b => b[i])
                .Where(x => x.Any())
                .ToArray())
            .ToArray();

        var sizes = groups
            .Select(g => g.Sum(x => x.Length))
            .ToArray();
        Scan(sizes);

        await Task.WhenAll(groups.Zip(sizes,
            (g, start) => Task.Run(() => MergeAndCopy(arr, g, start))));
    }
    
    private static (int l, int r)[] GetBounds(int l, int r)
    {
        int batchSize = (int) Math.Ceiling((r - l + 1) / (double) Parallelism);

        return Enumerable.Repeat(0, Parallelism - 1)
            .Select((_, i) => (l: l + i * batchSize, r: l + (i + 1) * batchSize - 1))
            .Append((l: l + batchSize * (Parallelism - 1), r))
            .ToArray();
    }

    private static int[] QuickSortWithSamples(int[] arr, int l, int r)
    {
        SequentialQuickSort(arr, l, r);

        var bounds = GetBounds(l, r);

        return bounds.Skip(1).Select(x => arr[x.l]).ToArray();
    }
    private static void SequentialQuickSort(int[] arr, int l, int r)
    {
        if (r <= l)
            return;

        var (lBound, rBound) = Partition(arr, l, r);

        SequentialQuickSort(arr, l, lBound - 1);
        SequentialQuickSort(arr, rBound + 1, r);
    }
    private static (int lBound, int rBound) Partition(int[] arr, int l, int r)
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

    private static int[][] MultiPartition(int[] arr, int l, int r, int[] pivots)
    {
        var rBounds = pivots
            .Select(x => BinarySearch(arr, l, r, x))
            .Append(r)
            .ToArray();

        return rBounds.Select(rBound =>
        {
            if (l > r)
                return Array.Empty<int>();

            var size = rBound - l + 1;
            if (size == 0)
                return Array.Empty<int>();
            
            var batch = new int[size];

            Array.Copy(arr, l, batch, 0, size);
            l += size;

            return batch;
        }).ToArray();
    }
    private static int BinarySearch(int[] arr, int l, int r, int value)
    {
        value++;
        
        while (l <= r)
        {
            int mid = (l & r) + ((l ^ r) >> 1);

            if (arr[mid] < value)
                l = mid + 1;
            else
                r = mid - 1;
        }
        
        return l - 1;
    }

    private static void MergeAndCopy(int[] arr, int[][] group, int cur)
    {
        var indices = new int[group.Length];
        
        while(true)
        {
            int? min = null;
            ref int minIdx = ref cur;
            
            for (int i = 0; i < indices.Length; i++)
            {
                var idx = indices[i];
                var grp = group[i];

                if (idx < grp.Length && (min is null || min > grp[idx]))
                {
                    min = grp[idx];
                    minIdx = ref indices[i];
                }
            }
            
            if (min is null)
                break;

            arr[cur++] = min.Value;
            minIdx++;
        }
    }
    
    private static void Scan(int[] arr)
    {
        var prev = arr[0];
        arr[0] = 0;
        
        for (int i = 1; i < arr.Length; i++)
        {
            var newPrev = arr[i] + prev;
            arr[i] = prev;
            prev = newPrev;
        }
    }

    private static void Swap(int[] arr, int first, int second)
    {
        (arr[first], arr[second]) = (arr[second], arr[first]);
    }
}