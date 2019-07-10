using System.Threading;
using System.Threading.Tasks;

namespace Surveillance.Reddeer.ApiClient.Interfaces
{
    public interface IHeartbeatApi
    {
        Task<bool> HeartBeating(CancellationToken token);
    }
}
