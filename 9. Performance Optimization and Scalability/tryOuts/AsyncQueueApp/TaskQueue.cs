using System.Threading.Channels;

namespace AsyncQueueApp
{
    public class TaskQueue
    {
        private readonly Channel<Func<Task>> _queue;
        public TaskQueue()
        {
            _queue = Channel.CreateUnbounded<Func<Task>>();
        }

        public async Task Enqueue(Func<Task> task)
        {
            await _queue.Writer.WriteAsync(task);
        }

        public async Task<Func<Task>> Dequeue()
        {
           return await _queue.Reader.ReadAsync();
        }

        public ChannelReader<Func<Task>> GetReader() {
            return _queue.Reader;
        }
    }
}
