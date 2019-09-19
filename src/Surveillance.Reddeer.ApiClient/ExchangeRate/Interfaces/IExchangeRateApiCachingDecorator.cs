namespace Surveillance.Reddeer.ApiClient.ExchangeRate.Interfaces
{
    using System;

    /// <summary>
    /// The ExchangeRateCachingDecorator interface.
    /// </summary>
    public interface IExchangeRateApiCachingDecorator : IExchangeRateApi
    {
        /// <summary>
        /// Gets the expiry.
        /// </summary>
        TimeSpan Expiry { get; }
    }
}