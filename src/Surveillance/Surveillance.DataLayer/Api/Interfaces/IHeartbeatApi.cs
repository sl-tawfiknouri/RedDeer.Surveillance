using System.Threading;
using System.Threading.Tasks;

namespace Surveillance.DataLayer.Api.Interfaces
{
    public interface IHeartbeatApi
    {
        Task<bool> HeartBeating(CancellationToken token);
    }
}
