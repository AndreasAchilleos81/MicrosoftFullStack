using SchedulingApp;

Console.WriteLine("=== Resource Scheduling: Knapsack Problem with Dynamic Programming ===\n");

// Define resource items
var resources = new ResourceItem[]
{
                new ResourceItem("Task A", 60, 10),
                new ResourceItem("Task B", 100, 20),
                new ResourceItem("Task C", 120, 30),
                new ResourceItem("Task D", 80, 15),
                new ResourceItem("Task E", 90, 25),
                new ResourceItem("Task F", 70, 18)
};

int capacity = 50;

Console.WriteLine("Available Resources:");
for (int i = 0; i < resources.Length; i++)
{
    Console.WriteLine($"  {i + 1}. {resources[i]}");
}
Console.WriteLine($"\nCapacity Constraint: {capacity}");
Console.WriteLine(new string('=', 70));

var scheduler = new KnapsackScheduler();

// Test with larger dataset
Console.WriteLine("\n=== STRESS TEST: Larger Dataset ===");
RunStressTest();

static void DisplaySolution(KnapsackSolution solution, string approach)
{
    Console.WriteLine($"Optimal Value: {solution.OptimalValue}");
    Console.WriteLine($"Execution Time: {solution.ExecutionTimeMs}ms");
    Console.WriteLine($"Recursive Calls: {solution.RecursiveCalls:N0}");
    Console.WriteLine($"Selected Items ({solution.SelectedItems.Count}):");

    int totalWeight = 0;
    foreach (var item in solution.SelectedItems)
    {
        Console.WriteLine($"  • {item}");
        totalWeight += item.Weight;
    }
    Console.WriteLine($"Total Weight Used: {totalWeight}");
}

static void DisplayPerformanceComparison(KnapsackSolution recursive, KnapsackSolution memoized)
{
    double speedup = recursive.ExecutionTimeMs > 0
        ? (double)recursive.ExecutionTimeMs / Math.Max(memoized.ExecutionTimeMs, 1)
        : 1;

    double callReduction = recursive.RecursiveCalls > 0
        ? (1 - (double)memoized.RecursiveCalls / recursive.RecursiveCalls) * 100
        : 0;

    Console.WriteLine($"Speedup Factor: {speedup:F2}x faster");
    Console.WriteLine($"Recursive Call Reduction: {callReduction:F1}%");
    Console.WriteLine($"  Recursive: {recursive.RecursiveCalls:N0} calls");
    Console.WriteLine($"  Memoized:  {memoized.RecursiveCalls:N0} calls");
}

static void RunStressTest()
{
    var largeDataset = new ResourceItem[15];
    var random = new Random(42);

    for (int i = 0; i < largeDataset.Length; i++)
    {
        largeDataset[i] = new ResourceItem(
            $"Resource {i + 1}",
            random.Next(50, 200),
            random.Next(5, 30)
        );
    }

    int largeCapacity = 100;
    var scheduler = new KnapsackScheduler();

    Console.WriteLine($"Dataset Size: {largeDataset.Length} items");
    Console.WriteLine($"Capacity: {largeCapacity}\n");

    // Only test memoized version for large datasets
    Console.WriteLine("Testing Memoized Approach...");
    var memoizedSolution = scheduler.SolveMemoized(largeDataset, largeCapacity);

    Console.WriteLine($"✓ Completed in {memoizedSolution.ExecutionTimeMs}ms");
    Console.WriteLine($"  Optimal Value: {memoizedSolution.OptimalValue}");
    Console.WriteLine($"  Recursive Calls: {memoizedSolution.RecursiveCalls:N0}");
    Console.WriteLine($"  Selected {memoizedSolution.SelectedItems.Count} items");


}