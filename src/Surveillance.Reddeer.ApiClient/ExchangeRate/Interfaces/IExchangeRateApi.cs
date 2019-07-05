using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RedDeer.Contracts.SurveillanceService.Api.ExchangeRate;
using Surveillance.Reddeer.ApiClient.Interfaces;

namespace Surveillance.Reddeer.ApiClient.ExchangeRate.Interfaces
{
    public interface IExchangeRateApi : IHeartbeatApi
    {
        Task<IDictionary<DateTime, IReadOnlyCollection<ExchangeRateDto>>> Get(DateTime commencement, DateTime termination);
    }
}