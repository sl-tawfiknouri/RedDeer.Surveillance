﻿namespace Infrastructure.Network.Aws.Interfaces
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Amazon.Runtime.SharedInterfaces;

    public interface IAwsQueueClient
    {
        ICoreAmazonSQS SqsClient { get; }

        Task<string> CreateQueue(string queueName);

        Task<bool> DeleteQueue(string queueName);

        Task<bool> ExistsQueue(string queueName);

        // ReSharper disable once UnusedMember.Global
        Task<string> GetQueueUrlAsync(string name, CancellationToken cancellationToken, bool retry = true);

        Task PurgeQueue(string queueName, CancellationToken token);

        Task<int> QueueMessageCount(string name, CancellationToken cancellationToken);

        Task SendToQueue(string name, string message, CancellationToken cancellationToken);

        Task SubscribeToQueueAsync(
            string name,
            Func<string, string, Task> action,
            CancellationToken cancellationToken,
            AwsResusableCancellationToken reusableToken);

        Task<string> UrlQueue(string queueName);
    }
}