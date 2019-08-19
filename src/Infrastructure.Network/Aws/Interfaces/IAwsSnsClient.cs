namespace Infrastructure.Network.Aws.Interfaces
{
    using System.Threading;
    using System.Threading.Tasks;

    using Amazon.Runtime.SharedInterfaces;

    public interface IAwsSnsClient
    {
        Task<string> CreateSnsTopic(string topicName, CancellationToken ct);

        Task SubscribeQueueToSnsTopic(string topicArn, ICoreAmazonSQS sqsClient, string sqsUrl);
    }
}