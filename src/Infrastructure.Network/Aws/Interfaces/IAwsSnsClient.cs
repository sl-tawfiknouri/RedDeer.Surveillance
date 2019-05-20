using System.Threading;
using System.Threading.Tasks;
using Amazon.Runtime.SharedInterfaces;

namespace Infrastructure.Network.Aws.Interfaces
{
    public interface IAwsSnsClient
    {
        Task<string> CreateSnsTopic(string topicName, CancellationToken ct);
        Task SubscribeQueueToSnsTopic(string topicArn, ICoreAmazonSQS sqsClient, string sqsUrl);
    }
}