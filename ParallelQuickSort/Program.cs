using System;
using System.Threading.Tasks;

namespace ParallelQuickSort
{
    class Program
    {
        static async Task Main()
        {
            var qs = new QuickSorter(false);

            await qs.ParallelQuickSort();

            //qs.PLINQSort(false);

            //await qs.TestParallelQuickSort();

            Console.ReadLine();
        }
    }
}
