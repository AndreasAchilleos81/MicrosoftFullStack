// 1. How did the LLM assist in refining the algorithm?
// The LLM identified Bubble Sort as the primary performance bottleneck and recommended replacing it with an in‑place QuickSort implementation. It also introduced parallel sorting and improved space efficiency by avoiding extra arrays.

// 2. Were any LLM-generated suggestions inaccurate or unnecessary?
// No. All suggestions aligned with standard algorithmic best practices and directly improved performance, scalability, and memory usage.

// 3. What were the most impactful improvements you implemented?
// Replacing Bubble Sort with QuickSort (O(n log n))

// Adding parallel sorting for multi-core performance

// Using in‑place partitioning to keep memory usage minimal

// These changes transformed the algorithm from a slow quadratic sorter into a highly efficient, scalable solution.

using System;
using System.Threading.Tasks;

public class Sorting
{
    // ✔ LLM-GENERATED MODIFICATION:
    // Replaced BubbleSort with QuickSort.
    // WHY: Bubble Sort is O(n²) and extremely slow for large datasets.
    //      QuickSort averages O(n log n) and is one of the fastest comparison sorts.
    public static void QuickSort(int[] arr)
    {
        QuickSort(arr, 0, arr.Length - 1);
    }

    // ✔ LLM-GENERATED MODIFICATION:
    // Added recursive QuickSort with parallel execution support.
    // WHY: Parallel.Invoke allows left and right partitions to be sorted concurrently,
    //      improving performance on multi-core CPUs for large datasets.
    private static void QuickSort(int[] arr, int left, int right)
    {
        if (left >= right)
            return;

        int pivotIndex = Partition(arr, left, right);

        // ✔ LLM-GENERATED MODIFICATION:
        // Parallel sorting of partitions.
        // WHY: Reduces total execution time for large arrays by using multiple CPU cores.
        Parallel.Invoke(
            () => QuickSort(arr, left, pivotIndex - 1),
            () => QuickSort(arr, pivotIndex + 1, right)
        );
    }

    // ✔ LLM-GENERATED MODIFICATION:
    // In-place partitioning method.
    // WHY: Avoids extra memory allocations, improving space efficiency.
    private static int Partition(int[] arr, int left, int right)
    {
        int pivot = arr[right];
        int i = left - 1;

        for (int j = left; j < right; j++)
        {
            if (arr[j] <= pivot)
            {
                i++;
                Swap(arr, i, j);
            }
        }

        Swap(arr, i + 1, right);
        return i + 1;
    }

    // ✔ LLM-GENERATED MODIFICATION:
    // Simple in-place swap helper.
    // WHY: Keeps QuickSort memory usage O(1).
    private static void Swap(int[] arr, int a, int b)
    {
        int temp = arr[a];
        arr[a] = arr[b];
        arr[b] = temp;
    }

    public static void PrintArray(int[] arr)
    {
        foreach (var item in arr)
            Console.Write(item + " ");

        Console.WriteLine();
    }

    public static void Main()
    {
        int[] dataset = { 50, 20, 40, 10, 30 };

        Console.WriteLine("Before Sorting:");
        PrintArray(dataset);

        // ✔ LLM-GENERATED MODIFICATION:
        // Replaced BubbleSort(dataset) with QuickSort(dataset).
        // WHY: QuickSort is dramatically faster and scales well.
        QuickSort(dataset);

        Console.WriteLine("After Sorting:");
        PrintArray(dataset);
    }
}
