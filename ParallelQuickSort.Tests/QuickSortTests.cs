using ParallelQuickSort.Core;

namespace ParallelQuickSort.Tests;

public class QuickSortTests
{
    [Fact]
    public async Task Testik()
    {
        var arr = new[] { 14, 89, 77, 18, 9, 1 };
        
        await Sorter.ParallelQuickSort(arr);

        arr.Should().BeInAscendingOrder();
    }
    
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

            arr.Should().BeInAscendingOrder("Initial array is {" + string.Join(", ", initial) + "}");
        }
    }
    
    [Fact]
    public async Task ParallelCorrectness()
    {
        for (int i = 10000; i <= 20000; i++)
        {
            var arr = Enumerable.Repeat(0, i)
                .Select(_ => Random.Shared.Next())
                .ToArray();

            var initial = new int[arr.Length];
            arr.CopyTo(initial, 0);
            
            await Sorter.ParallelQuickSort(arr);

            arr.Should().BeInAscendingOrder("Initial array sizeof(" + arr.Length + ") is {" + string.Join(", ", initial) + "}");
        }
    }

    [Fact]
    public async Task ParallelBigArray()
    {
        var arr = Enumerable.Repeat(0, 100_000_000)
            .Select(_ => Random.Shared.Next())
            .ToArray();
        
        await Sorter.ParallelQuickSort(arr);

        arr.Should().BeInAscendingOrder();
    }
}