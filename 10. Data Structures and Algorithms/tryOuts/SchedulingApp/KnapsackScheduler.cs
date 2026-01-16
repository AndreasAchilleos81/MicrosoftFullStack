namespace SchedulingApp
{
    public class KnapsackScheduler
    {
        private readonly Dictionary<string, int> memo = new Dictionary<string, int>();
        private int recursiveCallCount = 0;

        public KnapsackSolution SolveMemoized(ResourceItem[] items, int capacity)
        {
            memo.Clear();
            recursiveCallCount = 0; 
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            int[] values = items.Select(i => i.Value).ToArray();
            int[] weights = items.Select(i => i.Weight).ToArray();

            int optimalValue = KnapsackMemo(values, weights, capacity, items.Length);

            stopwatch.Stop();

            return new KnapsackSolution
            {
                OptimalValue = optimalValue,
                SelectedItems = ReconstructSolutionMemo(items, capacity, values, weights),
                ExecutionTimeMs = stopwatch.ElapsedMilliseconds,
                RecursiveCalls = recursiveCallCount
            };
        }

        private int KnapsackMemo(int[] values, int[] weights, int capacity, int n)
        {
            recursiveCallCount++;

            // Create unique key for this subproblem
            string key = $"{n}-{capacity}";

            // Check if result is already computed
            if (memo.ContainsKey(key))
            {
                return memo[key];
            }

            // Base case: no items left or no capacity
            if (n == 0 || capacity == 0)
            {
                return 0;
            }

            int result;

            // If weight of the nth item is more than capacity, skip it
            if (weights[n - 1] > capacity)
            {
                result = KnapsackMemo(values, weights, capacity, n - 1);
            }
            else
            {
                // Return maximum of two cases:
                // 1. nth item included
                // 2. nth item not included
                result = Math.Max(
                    values[n - 1] + KnapsackMemo(values, weights, capacity - weights[n - 1], n - 1),
                    KnapsackMemo(values, weights, capacity, n - 1)
                );
            }

            // Store result in memo before returning
            memo[key] = result;

            return result;
        }

        private List<ResourceItem> ReconstructSolutionMemo(ResourceItem[] items, int capacity, int[] values, int[] weights)
        {
            var selected = new List<ResourceItem>();
            int n = items.Length;
            int remainingCapacity = capacity;

            for (int i = n - 1; i >= 0 && remainingCapacity > 0; i--)
            {
                string keyWith = $"{i + 1}-{remainingCapacity}";
                string keyWithout = $"{i}-{remainingCapacity}";

                int withItem = memo.ContainsKey(keyWith) ? memo[keyWith] : 0;
                int withoutItem = memo.ContainsKey(keyWithout) ? memo[keyWithout] : 0;

                if (withItem != withoutItem && weights[i] <= remainingCapacity)
                {
                    selected.Add(items[i]);
                    remainingCapacity -= weights[i];
                }
            }

            return selected;
        }
    }
}
