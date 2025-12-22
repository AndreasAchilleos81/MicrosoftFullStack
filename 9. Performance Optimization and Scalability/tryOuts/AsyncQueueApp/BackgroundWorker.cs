

using System.Net.NetworkInformation;

namespace AsyncQueueApp
{
    public class BackgroundWorker
    {
        private readonly TaskQueue _taskQueue;

        public BackgroundWorker(TaskQueue taskQueue)
        {
            _taskQueue = taskQueue;
        }

        public async Task StarProcessing()
        {
            var reader = _taskQueue.GetReader();

            while (await reader.WaitToReadAsync())
            {
                var task = await _taskQueue.Dequeue();
                await task();

                await Task.Delay(300);
            }
        }
    }
}
