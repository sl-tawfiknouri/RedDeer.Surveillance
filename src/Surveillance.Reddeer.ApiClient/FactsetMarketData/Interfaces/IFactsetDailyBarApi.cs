namespace Surveillance.Reddeer.ApiClient.FactsetMarketData.Interfaces
{
    using System.Threading;
    using System.Threading.Tasks;

    using RedDeer.Contracts.SurveillanceService.Api.FactsetSecurityDaily;

    /// <summary>
    /// The DailyBar interface.
    /// </summary>
    public interface IFactsetDailyBarApi
    {
        /// <summary>
        /// The get async.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<FactsetSecurityResponseDto> GetAsync(FactsetSecurityDailyRequest request);

        /// <summary>
        /// The get with transient fault handling async.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<FactsetSecurityResponseDto> GetWithTransientFaultHandlingAsync(FactsetSecurityDailyRequest request);

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