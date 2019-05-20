using System.Threading.Tasks;

namespace Infrastructure.Network.Aws.Interfaces
{
    public interface IAwsSnsClient
    {
        Task CreateSnsTopic();
        Task SubscribeQueueToSnsTopic(object q);
    }
}