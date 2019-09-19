namespace Surveillance.Reddeer.ApiClient.RuleParameter.Interfaces
{
    using System.Threading.Tasks;

    using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;

    using Surveillance.Reddeer.ApiClient.Interfaces;

    /// <summary>
    /// The Rule Parameter interface.
    /// </summary>
    public interface IRuleParameterApi : IHeartbeatApi
    {
        /// <summary>
        /// The get async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<RuleParameterDto> GetAsync();

        /// <summary>
        /// The get async.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<RuleParameterDto> GetAsync(string id);
    }
}