using System;

namespace Surveillance.DataLayer.Api.ExchangeRate.Interfaces
{
    public interface IExchangeRateApiCachingDecoratorRepository : IExchangeRateApiRepository
    {
        TimeSpan Expiry { get; }
    }
}
