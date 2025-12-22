using AsyncQueueApp;

TaskQueue taskQueue = new TaskQueue();
BackgroundWorker backgroundWorker = new BackgroundWorker(taskQueue);

for (int i = 0; i < 10; i++)
{
        int taskId = i;
    await taskQueue.Enqueue(async () =>
    {
        Console.WriteLine($"Task {taskId} Started");
        await Task.Delay(1000);
        Console.WriteLine($"Task {taskId} completed");
    });
}

var workerTask = backgroundWorker.StarProcessing();


await workerTask;   