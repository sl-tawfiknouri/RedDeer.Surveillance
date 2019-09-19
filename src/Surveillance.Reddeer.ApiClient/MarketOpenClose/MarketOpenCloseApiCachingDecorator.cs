namespace Surveillance.Reddeer.ApiClient.MarketOpenClose
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using RedDeer.Contracts.SurveillanceService.Api.Markets;

    using Surveillance.Reddeer.ApiClient.MarketOpenClose.Interfaces;

    /// <summary>
    /// The market open close caching decorator.
    /// </summary>
    public class MarketOpenCloseApiCachingDecorator : IMarketOpenCloseApiCachingDecorator
    {
        /// <summary>
        /// The cache length.
        /// </summary>
        private readonly TimeSpan cacheLength;

        /// <summary>
        /// The decorated.
        /// </summary>
        private readonly IMarketOpenCloseApi decorated;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// The cached market data.
        /// </summary>
        private IReadOnlyCollection<ExchangeDto> cachedMarketData;

        /// <summary>
        /// The cache expiry.
        /// </summary>
        private DateTime cacheExpiry;

        /// <summary>
        /// Initializes a new instance of the <see cref="MarketOpenCloseApiCachingDecorator"/> class.
        /// </summary>
        /// <param name="decorated">
        /// The decorated.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public MarketOpenCloseApiCachingDecorator(
            IMarketOpenCloseApi decorated,
            ILogger<MarketOpenCloseApiCachingDecorator> logger)
        {
            this.cacheExpiry = DateTime.UtcNow.AddMilliseconds(-1);
            this.cacheLength = TimeSpan.FromMinutes(30);

            this.decorated = decorated ?? throw new ArgumentNullException(nameof(decorated));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// The get async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<IReadOnlyCollection<ExchangeDto>> GetAsync()
        {
            if (DateTime.UtcNow < this.cacheExpiry)
            {
                return this.cachedMarketData;
            }

            this.logger.LogInformation("Fetching market open/close data in the cached repository");
            this.cachedMarketData = await this.decorated.GetAsync().ConfigureAwait(false);
            this.cacheExpiry = DateTime.UtcNow.Add(this.cacheLength);

            return this.cachedMarketData;
        }

        /// <summary>
        /// The heart beating async.
        /// </summary>
        /// <param name="token">
        /// The token.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<bool> HeartBeatingAsync(CancellationToken token)
        {
            if (this.decorated == null)
            {
                return false;
            }

            return await this.decorated.HeartBeatingAsync(token).ConfigureAwait(false);
        }
    }
}