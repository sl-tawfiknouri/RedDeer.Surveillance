using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RedDeer.Contracts.SurveillanceService.Api.ExchangeRate;
using Surveillance.DataLayer.Api.Interfaces;

namespace Surveillance.DataLayer.Api.ExchangeRate.Interfaces
{
    public interface IExchangeRateApiRepository : IHeartbeatApi
    {
        Task<IDictionary<DateTime, IReadOnlyCollection<ExchangeRateDto>>> Get(DateTime commencement, DateTime termination);
    }
}