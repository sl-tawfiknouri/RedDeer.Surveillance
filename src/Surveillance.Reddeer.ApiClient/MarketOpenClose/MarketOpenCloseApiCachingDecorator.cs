namespace Surveillance.Reddeer.ApiClient.MarketOpenClose
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using RedDeer.Contracts.SurveillanceService.Api.Markets;

    using Surveillance.Reddeer.ApiClient.MarketOpenClose.Interfaces;

    public class MarketOpenCloseApiCachingDecorator : IMarketOpenCloseApiCachingDecorator
    {
        private readonly TimeSpan _cacheLength;

        private readonly IMarketOpenCloseApi _decorated;

        private readonly ILogger _logger;

        private IReadOnlyCollection<ExchangeDto> _cachedMarketData;

        private DateTime _cacheExpiry;

        public MarketOpenCloseApiCachingDecorator(
            IMarketOpenCloseApi decorated,
            ILogger<MarketOpenCloseApiCachingDecorator> logger)
        {
            this._cacheExpiry = DateTime.UtcNow.AddMilliseconds(-1);
            this._cacheLength = TimeSpan.FromMinutes(30);

            this._decorated = decorated ?? throw new ArgumentNullException(nameof(decorated));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IReadOnlyCollection<ExchangeDto>> Get()
        {
            if (DateTime.UtcNow < this._cacheExpiry) return this._cachedMarketData;

            this._logger.LogInformation("Fetching market open/close data in the cached repository");
            this._cachedMarketData = await this._decorated.Get();
            this._cacheExpiry = DateTime.UtcNow.Add(this._cacheLength);

            return this._cachedMarketData;
        }

        public async Task<bool> HeartBeating(CancellationToken token)
        {
            if (this._decorated == null) return false;

            return await this._decorated.HeartBeating(token);
        }
    }
}