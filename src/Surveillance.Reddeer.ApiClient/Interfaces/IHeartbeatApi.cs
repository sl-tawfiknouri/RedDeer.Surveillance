namespace Surveillance.Reddeer.ApiClient.Interfaces
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IHeartbeatApi
    {
        Task<bool> HeartBeating(CancellationToken token);
    }
}