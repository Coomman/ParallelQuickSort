using System.Diagnostics;
using ParallelQuickSort.Core.Sorters;

namespace ParallelQuickSort.Core.Benchmarks;

public static class ManualBenchmark
{
    private const string TestData = "100_000_000.txt";

    public static async Task CompareRun(int runsCount)
    {
        long seq = 0, par = 0;
        for (int i = 0; i < runsCount; i++)
        {
            Console.WriteLine($"Iteration #{i + 1}");
    
            var arr1 = Enumerable.Repeat(0, 100_000_000)
                .Select(_ => Random.Shared.Next())
                .ToArray();

            var arr2 = arr1
                .AsParallel()
                .ToArray();

            var sw = Stopwatch.StartNew();

            Sorter.QuickSort(arr1);

            seq += sw.ElapsedMilliseconds;


            sw = Stopwatch.StartNew();

            await SampleSorter.Sort(arr2);

            par += sw.ElapsedMilliseconds;
    
            sw.Reset();
        }

        double seqAvg = seq / (double) runsCount, parAvg = par / (double) runsCount;

        Console.WriteLine();
        Console.WriteLine("Sequential elapsed: " + TimeSpan.FromMilliseconds(seqAvg));
        Console.WriteLine("Parallel elapsed: " + TimeSpan.FromMilliseconds(parAvg));
        Console.WriteLine($"Parallel is {(seqAvg / parAvg - 1) * 100:F}% faster");
    }

    public static async Task SampleSort(int runsCount)
    {
        var arr = (await File.ReadAllTextAsync(TestData))
            .Split()
            .AsParallel()
            .Select(int.Parse)
            .ToArray();

        long par = 0;
        for (int i = 0; i < runsCount; i++)
        {
            Console.WriteLine($"Iteration #{i + 1}");

            var arr2 = arr
                .AsParallel()
                .ToArray();

            var sw = Stopwatch.StartNew();

            await SampleSorter.Sort(arr2);

            par += sw.ElapsedMilliseconds;
    
            sw.Reset();
        }

        double parAvg = par / (double) runsCount;

        Console.WriteLine("Parallel elapsed: " + TimeSpan.FromMilliseconds(parAvg));
    }
}