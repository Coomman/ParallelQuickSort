using ParallelQuickSort.Core;

namespace ParallelQuickSort.Tests;

public class QuickSortTests
{
    [Fact]
    public void SequentialCorrectness()
    {
        var sorter = new Sorter();
        
        for (int i = 1; i <= 1000; i++)
        {
            var arr = Enumerable.Repeat(0, i)
                .Select(_ => Random.Shared.Next())
                .ToArray();

            var initial = new int[arr.Length];
            arr.CopyTo(initial, 0);
            
            sorter.QuickSort(arr);

            arr.Should().BeInAscendingOrder("Initial array is {" + string.Join(", ", initial) + "}");
        }
    }
}