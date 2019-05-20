using System;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Network.Aws.Interfaces
{
    public interface IAwsQueueClient
    {
        // ReSharper disable once UnusedMember.Global
        Task<string> GetQueueUrlAsync(string name, CancellationToken cancellationToken, bool retry = true);
        Task SendToQueue(string name, string message, CancellationToken cancellationToken);
        Task SubscribeToQueueAsync(string name, Func<string, string, Task> action, CancellationToken cancellationToken, AwsResusableCancellationToken reusableToken);
        Task PurgeQueue(string queueName, CancellationToken token);
        Task<int> QueueMessageCount(string name, CancellationToken cancellationToken);
        Task<bool> DeleteQueue(string queueName);
        Task CreateQueue(string queueName);
        Task<bool> ExistsQueue(string queueName);
    }
}