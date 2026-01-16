using System;
using System.Collections.Generic;
namespace Optimized;

public class TaskExecutorDebugged
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