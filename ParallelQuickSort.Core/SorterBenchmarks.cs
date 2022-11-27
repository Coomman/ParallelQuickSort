using BenchmarkDotNet.Attributes;

namespace ParallelQuickSort.Core;

public class SorterBenchmarks
{
    private readonly Sorter _sorter = new();
    
    private int[] _arr = null!;

    [Params(100_000, 100_000_000)] 
    public int Length { get; set; }
    
    [GlobalSetup]
    public void Setup()
    {
        _arr = Enumerable.Repeat(0, Length).Select(_ => Random.Shared.Next()).ToArray();
    }

    [Benchmark]
    public void QuickSort()
    {
        Sorter.QuickSort(_arr);
    }
}