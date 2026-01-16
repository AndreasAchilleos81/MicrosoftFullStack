// 1. How did the LLM assist in debugging and optimizing the code?
// The LLM identified crash‑prone areas such as unhandled exceptions, missing null checks, and lack of logging. It proposed structured error handling, retry logic, and centralized logging, transforming the code into a more resilient and maintainable system.

// 2. Were any LLM-generated suggestions inaccurate or unnecessary?
// No. All suggestions aligned with standard defensive programming practices and directly improved reliability, readability, and runtime stability.

// 3. What were the most impactful improvements you implemented?
// Adding null checks to prevent invalid input from crashing the program

// Replacing exception‑driven crashes with structured logging

// Introducing retry logic to handle transient failures gracefully

// Wrapping execution in safe error‑handling blocks

using System;
using System.Collections.Generic;

public class TaskExecutor
{
    private readonly Queue<string> taskQueue = new Queue<string>();

    public void AddTask(string task)
    {
        // ✔ LLM-GENERATED MODIFICATION:
        // Added null check before enqueueing.
        // WHY: Prevents invalid tasks from entering the queue and causing failures later.
        if (task == null)
        {
            LogError("Attempted to add a null task. Skipping.");
            return;
        }

        taskQueue.Enqueue(task);
    }

    public void ProcessTasks()
    {
        while (taskQueue.Count > 0)
        {
            string task = taskQueue.Dequeue();
            Console.WriteLine($"Processing task: {task}");

            // ✔ LLM-GENERATED MODIFICATION:
            // Wrap task execution in retry logic.
            // WHY: Prevents the entire system from crashing due to a single failing task.
            bool success = ExecuteWithRetry(task, maxRetries: 2);

            if (!success)
            {
                LogError($"Task '{task}' failed after retries. Moving on.");
            }
        }
    }

    // ✔ LLM-GENERATED MODIFICATION:
    // Added retry wrapper around ExecuteTask.
    // WHY: Allows transient failures to recover without requiring complex concurrency.
    private bool ExecuteWithRetry(string task, int maxRetries)
    {
        int attempts = 0;

        while (attempts <= maxRetries)
        {
            try
            {
                ExecuteTask(task);
                return true; // Success
            }
            catch (Exception ex)
            {
                attempts++;

                // ✔ LLM-GENERATED MODIFICATION:
                // Replaced crash-causing exceptions with logging.
                // WHY: Improves stability and provides diagnostic information.
                LogError($"Error executing task '{task}' (Attempt {attempts}): {ex.Message}");

                if (attempts > maxRetries)
                    return false;

                Console.WriteLine("Retrying...");
            }
        }

        return false;
    }

    private void ExecuteTask(string task)
    {
        // ✔ LLM-GENERATED MODIFICATION:
        // Defensive null check inside execution.
        // WHY: Ensures unexpected nulls do not crash the system.
        if (task == null)
            throw new ArgumentNullException(nameof(task), "Task cannot be null.");

        // Simulated failure condition
        if (task.Contains("Fail"))
            throw new InvalidOperationException("Simulated task failure.");

        Console.WriteLine($"Task '{task}' completed successfully.");
    }

    // ✔ LLM-GENERATED MODIFICATION:
    // Centralized logging method.
    // WHY: Makes it easy to replace Console.WriteLine with a real logging framework later.
    private void LogError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[ERROR] {message}");
        Console.ResetColor();
    }
}

class Program
{
    static void Main()
    {
        TaskExecutor executor = new TaskExecutor();

        executor.AddTask("Task 1");
        executor.AddTask(null); // Now safely logged, not crashing
        executor.AddTask("Fail Task"); // Will retry, then log failure
        executor.AddTask("Task 2");

        executor.ProcessTasks();
    }
}
