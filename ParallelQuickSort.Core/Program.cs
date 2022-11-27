using System.Diagnostics;
using ParallelQuickSort.Core;

const double count = 5;

long seq = 0, par = 0;
for (int i = 0; i < count; i++)
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

    await Sorter.ParallelQuickSort(arr2);

    par += sw.ElapsedMilliseconds;
    
    sw.Reset();
}

double seqAvg = seq / count, parAvg = par / count;

Console.WriteLine();
Console.WriteLine("Sequential elapsed: " + TimeSpan.FromMilliseconds(seqAvg));
Console.WriteLine("Parallel elapsed: " + TimeSpan.FromMilliseconds(parAvg));
Console.WriteLine($"Parallel is {(seqAvg / parAvg - 1) * 100:F}% faster");

