namespace Surveillance.Reddeer.ApiClient.MarketOpenClose.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using RedDeer.Contracts.SurveillanceService.Api.Markets;

    using Surveillance.Reddeer.ApiClient.Interfaces;

    /// <summary>
    /// The MarketOpenClose interface.
    /// </summary>
    public interface IMarketOpenCloseApi : IHeartbeatApi
    {
        /// <summary>
        /// The get async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<IReadOnlyCollection<ExchangeDto>> GetAsync();
    }
}