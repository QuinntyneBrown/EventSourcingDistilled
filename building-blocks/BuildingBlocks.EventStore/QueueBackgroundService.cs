using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingBlocks.EventStore
{
    public class QueueBackgroundService : BackgroundService
    {
        private readonly ITaskQueue _taskQueue;

        public QueueBackgroundService(ITaskQueue taskQueue)
        {
            _taskQueue = taskQueue;
        }

        protected async override Task ExecuteAsync(
            CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var workItem = await _taskQueue.DequeueAsync(cancellationToken);

                await workItem(cancellationToken);
            }
        }
    }
}
