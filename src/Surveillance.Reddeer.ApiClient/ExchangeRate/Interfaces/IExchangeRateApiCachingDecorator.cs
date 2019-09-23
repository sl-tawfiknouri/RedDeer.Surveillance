namespace Surveillance.Reddeer.ApiClient.ExchangeRate.Interfaces
{
    using System;

    public interface IExchangeRateApiCachingDecorator : IExchangeRateApi
    {
        TimeSpan Expiry { get; }
    }
}