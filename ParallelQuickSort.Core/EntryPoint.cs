using BenchmarkDotNet.Running;
using ParallelQuickSort.Core.Benchmarks;

//await ManualBenchmark.CompareRun(5);

BenchmarkRunner.Run<SorterBenchmarks>();