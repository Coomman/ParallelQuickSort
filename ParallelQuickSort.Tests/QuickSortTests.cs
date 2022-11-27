using ParallelQuickSort.Core.Sorters;

namespace ParallelQuickSort.Tests;

public class QuickSortTests
{
    [Fact]
    public void SequentialCorrectness()
    {
        for (int i = 1; i <= 1000; i++)
        {
            var arr = Enumerable.Repeat(0, i)
                .Select(_ => Random.Shared.Next())
                .ToArray();

            var initial = new int[arr.Length];
            arr.CopyTo(initial, 0);
            
            Sorter.QuickSort(arr);

            arr.Should().BeInAscendingOrder(GetErrorDescription(initial));
        }
    }
    
    [Fact]
    public async Task NaiveParallelCorrectness()
    {
        for (int i = 2; i <= 1000; i++)
        {
            var arr = Enumerable.Repeat(0, i)
                .Select(_ => Random.Shared.Next())
                .ToArray();

            var initial = new int[arr.Length];
            arr.CopyTo(initial, 0);
            
            await Sorter.NaiveParallelQuickSort(arr);

            arr.Should().BeInAscendingOrder(GetErrorDescription(initial));
        }
    }

    [Fact]
    public async Task SampleSortOrderCorrectness()
    {
        for (int i = 100; i < 10_000; i++)
        {
            var arr = Enumerable.Repeat(0, i)
                .Select(_ => Random.Shared.Next())
                .ToArray();

            var initial = new int[arr.Length];
            arr.CopyTo(initial, 0);
            
            await SampleSorter.Sort(arr);

            arr.Should().BeInAscendingOrder(GetErrorDescription(initial));
        }
    }
    
    [Fact]
    public async Task SampleSortContentCorrectness()
    {
        for (int i = 100; i < 1000; i++)
        {
            var arr = Enumerable.Repeat(0, i)
                .Select(_ => Random.Shared.Next())
                .ToArray();

            var initial = arr.Order().ToArray();
            await SampleSorter.Sort(arr);

            arr.Should().BeEquivalentTo(initial);
        }
    }
    
    [Fact]
    public async Task SampleSortBigArrayCorrectness()
    {
        var arr = Enumerable.Repeat(0, 100_000_000)
            .Select(_ => Random.Shared.Next())
            .ToArray();

        var initial = arr.AsParallel().Order();
        await SampleSorter.Sort(arr);

        arr.AsParallel().SequenceEqual(initial.AsParallel()).Should().BeTrue();
    }

    private static string GetErrorDescription(int[] initial)
    {
        return "Initial array sizeof(" + initial.Length + ") is {" + string.Join(", ", initial) + "}";
    }
}