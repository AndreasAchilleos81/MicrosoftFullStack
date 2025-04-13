using System;
					
public class Program
{
		static string pending = "Pending";
		static string completed = "Completed";
		
        static string task1 = string.Empty;
        static string task2 = string.Empty;
        static string task3 = string.Empty;

        static bool taskOneCompleted = false;
        static bool taskTwoCompleted = false;
        static bool taskThreeCompleted = false;
	
    public static void Main()
    {
        do
        {
			Console.WriteLine("Enter (1) to enter a task, (2) to complete a tasK, (3) Display tasks, (4) Quit");
			string choice = Console.ReadLine();
			if (choice == "1"){
				AddTask();
				continue;
			}
			
			if (choice == "2"){
				CompleteATask();    
				continue;
			}
			
			if (choice == "3"){
				DisplayTasks();
				continue;
			}
			
			if (choice == "4"){
				break;
			}
			
        } while (true);
	
		Console.WriteLine("Exited - displaying tasks");
		DisplayTasks();
		
    }
	
	public static void DisplayTasks(){
		
		if(!string.IsNullOrEmpty(task1)){
			Console.WriteLine($"Task: {task1} has status: {(taskOneCompleted ? completed : pending)}");
		}
		
		if(!string.IsNullOrEmpty(task2)){
			Console.WriteLine($"Task: {task1} has status: {(taskTwoCompleted ? completed : pending)}");
		}
		
		if(!string.IsNullOrEmpty(task3)){
			Console.WriteLine($"Task: {task1} has status: {(taskThreeCompleted ? completed : pending)}");
		}	
	}

	public static void AddTask()
	{
		if(AreAllTaskFilled()){
			Console.WriteLine("All task slots have been filled");
			return;
		}
		
		Console.WriteLine("Add a task: ");
		string task = Console.ReadLine();
		
        if (task1 == string.Empty)
		{
           task1 = task;
        }
        else if (task2 == string.Empty)
        {
            task2 = task;
        }
        else if (task3 == string.Empty)
        {
            task3 = task;
        }
	}

	public static void CompleteATask(){
		Console.Write("Which task would you like for me to process (1 , 2, 3)");
			int taskChoice = int.Parse(Console.ReadLine());
			
			if (taskChoice == 3 && !string.IsNullOrEmpty(task3)){
				taskThreeCompleted = true;
				Console.WriteLine("Task 3 completed");
			}
			else if (taskChoice == 2 && !string.IsNullOrEmpty(task2)){
				taskTwoCompleted = true;
				Console.WriteLine("Task 2 completed");
			}
			else if (taskChoice == 1 && !string.IsNullOrEmpty(task1)){
				taskOneCompleted = true;
				Console.WriteLine("Task 1 completed");
			}
			else {
				Console.WriteLine("Incorrect task choice or task does not exist or task has already been completed");
			}
	}
	
	public static bool AreAllTasksCompleted(){
		return taskThreeCompleted && taskTwoCompleted && taskOneCompleted;
	}
	
    public static bool AreAllTaskFilled()
    {
        return task1 != string.Empty &&
                task2 != string.Empty &&
                task3 != string.Empty;
    }
}