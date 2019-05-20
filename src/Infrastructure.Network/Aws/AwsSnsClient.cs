using System.Threading.Tasks;
using Infrastructure.Network.Aws.Interfaces;

namespace Infrastructure.Network.Aws
{
    public class AwsSnsClient : IAwsSnsClient
    {
        public async Task CreateSnsTopic()
        {

        }

        public async Task SubscribeQueueToSnsTopic(object q)
        {

        }
    }
}
