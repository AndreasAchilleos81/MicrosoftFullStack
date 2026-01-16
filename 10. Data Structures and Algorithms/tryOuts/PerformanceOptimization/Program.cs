using System;
using System.Diagnostics;

class Program
{
    static void Main()
    {
        var stopwatch = Stopwatch.StartNew();
        int result = Fibonacci(40);
        stopwatch.Stop();
        Console.WriteLine($"Fibonacci(40) = {result}");
        Console.WriteLine($"Execution Time: {stopwatch.ElapsedMilliseconds} ms");
    }

    //static int Fibonacci(int n)
    //{
    //    if (n <= 1) return n;
    //    return Fibonacci(n - 1) + Fibonacci(n - 2);
    //}

    // Optimized Fibonacci using memoization.
    // Time Complexity: O(n)
    // Space Complexity: O(n)
    static int Fibonacci(int n, Dictionary<int, int> memo = null)
    {
        memo ??= new Dictionary<int, int>();

        if (n <= 1)
            return n;

        // Return cached value if available
        if (memo.ContainsKey(n))
            return memo[n];

        // Compute and store result
        memo[n] = Fibonacci(n - 1, memo) + Fibonacci(n - 2, memo);
        return memo[n];
    }
}