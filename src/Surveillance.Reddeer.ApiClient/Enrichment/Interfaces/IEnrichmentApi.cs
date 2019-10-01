namespace Surveillance.Reddeer.ApiClient.Enrichment.Interfaces
{
    using System.Threading;
    using System.Threading.Tasks;

    using RedDeer.Contracts.SurveillanceService.Api.SecurityEnrichment;

    /// <summary>
    /// The Enrichment interface.
    /// </summary>
    public interface IEnrichmentApi
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

        /// <summary>
        /// The post async.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<SecurityEnrichmentMessage> PostAsync(SecurityEnrichmentMessage message);
    }
}