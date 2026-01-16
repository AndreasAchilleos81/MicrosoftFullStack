using DynamicScheduling;

var resources = new ResourceItem[]
		{
			new ResourceItem("Task A", 60, 10),
			new ResourceItem("Task B", 100, 20),
			new ResourceItem("Task C", 120, 30),
			new ResourceItem("Task D", 80, 15),
			new ResourceItem("Task E", 90, 25),
			new ResourceItem("Task F", 70, 18)
		};


Knapsack knapsack = new Knapsack();

int capacity = 50;

var result = knapsack.SolveMemoized(resources, capacity); ;

DisplaySolution(result, "Memoized");

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