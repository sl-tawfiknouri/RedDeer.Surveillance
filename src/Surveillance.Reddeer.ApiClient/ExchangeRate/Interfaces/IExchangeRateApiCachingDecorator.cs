using System;

namespace Surveillance.Reddeer.ApiClient.ExchangeRate.Interfaces
{
    public interface IExchangeRateApiCachingDecorator : IExchangeRateApi
    {
        TimeSpan Expiry { get; }
    }
}
