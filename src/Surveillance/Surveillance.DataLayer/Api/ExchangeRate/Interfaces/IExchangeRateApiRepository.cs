using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RedDeer.Contracts.SurveillanceService.Api.ExchangeRate;

namespace Surveillance.DataLayer.Api.ExchangeRate.Interfaces
{
    public interface IExchangeRateApiRepository
    {
        Task<IReadOnlyCollection<ExchangeRateDto>> Get(DateTime commencement, DateTime termination);
    }
}