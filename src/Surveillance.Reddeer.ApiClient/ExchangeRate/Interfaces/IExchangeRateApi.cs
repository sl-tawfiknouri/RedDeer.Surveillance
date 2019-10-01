namespace Surveillance.Reddeer.ApiClient.ExchangeRate.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using RedDeer.Contracts.SurveillanceService.Api.ExchangeRate;

    using Surveillance.Reddeer.ApiClient.Interfaces;

    /// <summary>
    /// The ExchangeRate interface.
    /// </summary>
    public interface IExchangeRateApi : IHeartbeatApi
    {
        /// <summary>
        /// The get async.
        /// </summary>
        /// <param name="commencement">
        /// The commencement.
        /// </param>
        /// <param name="termination">
        /// The termination.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<IDictionary<DateTime, IReadOnlyCollection<ExchangeRateDto>>> GetAsync(DateTime commencement, DateTime termination);
    }
}