namespace Surveillance.Reddeer.ApiClient.Interfaces
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// The Heartbeat interface.
    /// </summary>
    public interface IHeartbeatApi
    {
        /// <summary>
        /// The heart beating async.
        /// </summary>
        /// <param name="token">
        /// The token.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<bool> HeartBeatingAsync(CancellationToken token);
    }
}