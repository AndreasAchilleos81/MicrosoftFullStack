using System;
using System.Collections.Generic;

namespace NonOptimized;
public class TaskExecutor
{
	private Queue<string> taskQueue = new Queue<string>();
	public void AddTask(string task)
	{
		taskQueue.Enqueue(task);
	}
	public void ProcessTasks()
	{
		while (taskQueue.Count > 0)
		{
			string task = taskQueue.Dequeue();
			Console.WriteLine($"Processing task: {task}");
			ExecuteTask(task);
		}
	}
	private void ExecuteTask(string task)
	{
		if (task == null)
		{
			throw new Exception("Task is null");
		}
		if (task.Contains("Fail"))
		{
			throw new Exception("Task execution failed");
		}
		Console.WriteLine($"Task {task} completed successfully.");
	}
}
class Program
{
	static void Main()
	{
		TaskExecutor executor = new TaskExecutor();
		executor.AddTask("Task 1");
		executor.AddTask(null); // This will cause a crash
		executor.AddTask("Fail Task"); // This will also cause a crash
		executor.AddTask("Task 2");
		executor.ProcessTasks();
	}
}
