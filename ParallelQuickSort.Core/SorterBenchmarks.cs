using BenchmarkDotNet.Attributes;

namespace ParallelQuickSort.Core;

public class SorterBenchmarks
{
    private int[] _arr = null!;
    private int[] _needToSort = null!;

    [Params(100_000_000)] 
    public int Length { get; set; }
    
    [GlobalSetup]
    public void GlobalSetup()
    {
        _arr = Enumerable.Repeat(0, Length).Select(_ => Random.Shared.Next()).ToArray();
    }

    [IterationSetup]
    public void IterationSetup()
    {
        _needToSort = new int[_arr.Length];
        _arr.CopyTo(_needToSort, 0);
    }

    [Benchmark]
    public void QuickSort()
    {
        Sorter.QuickSort(_needToSort);
    }
    
    [Benchmark]
    public async Task NaiveParallelQuickSort()
    {
        await Sorter.NaiveParallelQuickSort(_needToSort);
    }
}
