using System;
using System.Threading;
using System.Threading.Tasks;

namespace Surveillance.DataLayer.AwsQueue.Interfaces
{
    public interface IAwsQueueClient
    {
        Task<string> GetQueueUrlAsync(string name, CancellationToken cancellationToken);
        Task SendToQueue(string name, string message, CancellationToken cancellationToken);
        Task SubscribeToQueueAsync(string name, Func<string, string, Task> action, CancellationToken cancellationToken);
    }
}