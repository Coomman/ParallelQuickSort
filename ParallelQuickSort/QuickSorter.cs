using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using static System.ConsoleColor;

namespace ParallelQuickSort
{
    public class QuickSorter
    {
        private Stopwatch Timer { get; } = new Stopwatch();

        private const string FilePath = "data.txt";

        private readonly object _lockObject = new object();

        private int[] Data { get; }

        public QuickSorter(bool downloadData = true)
        {
            if (downloadData && File.Exists(FilePath))
                Data = File.ReadAllText(FilePath)
                    .Split(' ')
                    .Select(int.Parse)
                    .ToArray();
        }

        public void GenerateData()
        {
            var data = SortHelper.GenerateArray(50_000_000);
            File.WriteAllText(FilePath, string.Join(' ', data));
        }

        private async Task<TimeSpan> QuickSortRoutine(ParallelQuickSorter<int> qs)
        {
            var arr = SortHelper.GenerateArray(10_000_000);

            var start = DateTime.Now.TimeOfDay;

            //await qs.QuickSortAsync(arr);
            await Task.Run(() => qs.QuickSort(arr));

            var finish = DateTime.Now.TimeOfDay;

            lock (_lockObject)
                PrintResult(finish - start, arr.CheckSort(), "Parallel QuickSort", arr.Length);

            return finish - start;
        }

        public void PLINQSort(bool fromFile = true)
        {
            int[] arr;

            if (fromFile)
                arr = Data.Clone() as int[];
            else
                arr = SortHelper.GenerateArray(10_000_000);

            Timer.Start();

            arr = arr.AsParallel()
                .OrderBy(n => n)
                .ToArray();

            Timer.Stop();

            PrintResult(Timer.Elapsed, arr.CheckSort(), "PLINQ OrderBy", arr.Length);
        }

        public async Task ParallelQuickSort()
        {
            var qs = new ParallelQuickSorter<int>();

            var tasks = new Task<TimeSpan>[5];

            for (int j = 0; j < tasks.Length; j++)
                tasks[j] = Task.Run(() => QuickSortRoutine(qs));

            await Task.WhenAll(tasks);

            Yellow.WriteLine(
                $"Average time is {new TimeSpan(Convert.ToInt64(tasks.Average(task => task.Result.Ticks)))}\n");
        }

        public async Task TestParallelQuickSort()
        {
            var qs = new ParallelQuickSorter<int>();

            for (int i = 70; i <= 100; i += 10)
            {
                Console.WriteLine($"InsertionBound = {i}");

                var tasks = new Task<TimeSpan>[5];

                for (int j = 0; j < 5; j++)
                    tasks[j] = Task.Run(() => QuickSortRoutine(qs));

                await Task.WhenAll(tasks);

                Yellow.WriteLine(
                    $"Average time is {new TimeSpan(Convert.ToInt64(tasks.Average(task => task.Result.Ticks)))}\n");
            }
        }

        private void PrintResult(TimeSpan time, bool isSorted, string sortName, int elemCount)
        {
            Cyan.Write(sortName);
            Console.Write(" on ");
            DarkCyan.Write(elemCount.ToString("N0"));

            Console.Write(" elements finished in ");
            Blue.Write(time.ToString());

            Console.Write(". Array is sorted: ");
            if (isSorted)
                Green.WriteLine("✓");
            else
                Red.WriteLine("×");

            Timer.Reset();
        }
    }
}
