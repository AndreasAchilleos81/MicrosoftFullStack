using Optimized;
using NonOptimized;
using System.Diagnostics;

NonOptimized.BinaryTree binaryTree = new BinaryTree();

Optimized.AvlTree avlTree = new AvlTree();


foreach (var value in Enumerable.Range(1, 10000))
{
	binaryTree.Insert(value);
	avlTree.Insert(value);
}

Stopwatch stopwatch = new Stopwatch();
stopwatch.Start();
binaryTree.PrintInOrder(binaryTree.Root);
long nonOptimedBinaryTreeTime = stopwatch.ElapsedMilliseconds;

stopwatch.Restart();
avlTree.PrintInOrder(avlTree.Root);

long optimizedAVLTreeTime = stopwatch.ElapsedMilliseconds;


stopwatch.Stop();

NonOptimized.ApiRequestQueue queue = new NonOptimized.ApiRequestQueue();
stopwatch.Restart();	
queue.Enqueue(new NonOptimized.ApiRequest("/auth", 1));
queue.Enqueue(new NonOptimized.ApiRequest("/data", 3));
queue.Enqueue(new NonOptimized.ApiRequest("/healthcheck", 2));
long nonOptimizedQueueTime = stopwatch.ElapsedTicks;

Optimized.ApiRequestQueue optQueue = new Optimized.ApiRequestQueue();
stopwatch.Restart();
optQueue.Enqueue(new Optimized.ApiRequest("/auth", 1));
optQueue.Enqueue(new Optimized.ApiRequest("/data", 3));
optQueue.Enqueue(new Optimized.ApiRequest("/healthcheck", 2));
long optimizedQueueTime = stopwatch.ElapsedTicks;

// Sorting 

int[] datasetForNonOptimized = GenerateRandomDataset();
int[] datasetForOptimized = datasetForNonOptimized.ToArray(); // Clone for fair comparison

Sorting nonOptimized = new Sorting();	

stopwatch.Restart();
nonOptimized.BubbleSort(datasetForNonOptimized);

Console.WriteLine("Before Sorting:");
nonOptimized.PrintArray(datasetForNonOptimized);
nonOptimized.BubbleSort(datasetForNonOptimized);
Console.WriteLine("After Sorting:");
nonOptimized.PrintArray(datasetForNonOptimized);
long nonOptimizedSortingTime = stopwatch.ElapsedTicks;


stopwatch.Restart();
OptimizedSorting optimizedSorting = new OptimizedSorting();
Console.WriteLine("Before Sorting:");
optimizedSorting.PrintArray(datasetForOptimized);
optimizedSorting.QuickSort(datasetForOptimized);
Console.WriteLine("After Sorting:");
optimizedSorting.PrintArray(datasetForOptimized);
long OptimizedSortingTime = stopwatch.ElapsedTicks;

stopwatch.Stop();


Console.WriteLine("\n\n");
Console.WriteLine("-------------------------------------------------------------------------------------");
Console.WriteLine("ACTIVITY 1 Time taken to print both trees:");
Console.WriteLine($"Non optimized tree took to print: {nonOptimedBinaryTreeTime}");
Console.WriteLine($"Optimized tree took to print: {optimizedAVLTreeTime}");
Console.WriteLine("-------------------------------------------------------------------------------------");

Console.WriteLine("ACTIVITY 2 Time taken to print both QUEUES:");
Console.WriteLine($"Non Optimized Api Request Queue Processing Time: {nonOptimizedQueueTime}");
Console.WriteLine($"Optimized Api Request Queue Processing Time: {optimizedQueueTime}");
Console.WriteLine("-------------------------------------------------------------------------------------");

Console.WriteLine("ACTIVITY 3 Time taken to to sort and print before and after sort:");
Console.WriteLine($"Non Optimized Sorting Processing Time: {nonOptimizedSortingTime}");
Console.WriteLine($"Optimized Sorting Processing Time: {OptimizedSortingTime}");
Console.WriteLine("-------------------------------------------------------------------------------------");

TaskExecutorDebugged executor = new TaskExecutorDebugged();

executor.AddTask("Task 1");
executor.AddTask(null); // Now safely logged, not crashing
executor.AddTask("Fail Task"); // Will retry, then log failure
executor.AddTask("Task 2");

executor.ProcessTasks();

static int[] GenerateRandomDataset(int size = 10000)
{
	var rand = new Random(42); // fixed seed for reproducibility
	int[] arr = new int[size];

	for (int i = 0; i < size; i++)
		arr[i] = rand.Next(0, 1000000);

	return arr;
}
