# Parallel QuickSort Implementation

Repository contains 3 QuickSort algorithms and benchmarks to test the performance.

## [Sequential version](https://github.com/Coomman/ParallelQuickSort/blob/494ac9a6cbe5582e7277470f9e045b5a2015cc86/ParallelQuickSort.Core/Sorters/Sorter.cs#L8)
Just a basic algorithm with 2 optimizations:
1. **Partition returns two numbers:** indices of the beginning and the end of pivot elements (helps to skip all the repeated pivots).
2. **Pivot choosing optimization:** pick three numbers and get the middle of them.

## [Naive parallel version](https://github.com/Coomman/ParallelQuickSort/blob/494ac9a6cbe5582e7277470f9e045b5a2015cc86/ParallelQuickSort.Core/Sorters/Sorter.cs#L13)
The Sequential version of the algorithm with 2 aspects parallelized:
1. **Parallel partition:** finds elements less than the pivot and greater than the pivot in parallel.  
Then copies them to the array in parallel.
2. ***Fork-join* recursion:** runs QS on elements less than the pivot and greater than the pivot in parallel with threads limitation.

## [SampleSort](https://github.com/Coomman/ParallelQuickSort/blob/494ac9a6cbe5582e7277470f9e045b5a2015cc86/ParallelQuickSort.Core/Sorters/SampleSorter.cs#L7)
The main problem of the Naive parallel version is using a local pivot.  
This algorithm finds the approximate pivots for the whole array, but uses **O(n)** extra memory:
1. Split equal parts of the array between **P** processes.
2. Run Sequential QS on all parts in parallel.
3. In each part get sample elements dividing their parts into equal subparts.
4. Run Sequential QS on samples and take their samples as pivots.
5. Partition all the parts in parallel using this pivots to get **P** groups (ex. all elements < n in each part).
6. Find starting copy point for each group using ***Scan***.
7. Merge (similarly to merge-sort) and copy groups in parallel.

# Benchmarks
**Task:** Sorting 100.000.000 random array using a maximum of 4 threads.  
**Used benchmark:** BenchmarkDotNet 15 runs average.

BenchmarkDotNet=v0.13.2, OS=Windows 10 (10.0.19045.2251)  
Intel Core i9-9900K CPU 5.00GHz (Coffee Lake), 1 CPU, 16 logical and 8 physical cores  
.NET SDK=7.0.100  
[Host]     : .NET 7.0.0 (7.0.22.51805), X64 RyuJIT AVX2  
Job-EKYYXI : .NET 7.0.0 (7.0.22.51805), X64 RyuJIT AVX2

| Method            | Array Size  | Mean     | Error    | StdDev   |
|-------------------|-------------|----------|----------|----------|
| Sequential QS     | 100.000.000 | 16.010 s | 0.0178 s | 0.0149 s |
| Naive Parallel QS | 100.000.000 | 8.810 s  | 0.1748 s | 0.1943 s |
| SampleSort        | 100.000.000 | 3.452 s  | 0.0138 s | 0.0129 s |

The **Naive Parallel** version is **80%** faster and **SampleSort** is approximately **360%** faster than the **Sequential** version.
