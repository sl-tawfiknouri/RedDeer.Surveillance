using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RedDeer.Contracts.SurveillanceService.Api.Markets;
using Surveillance.Reddeer.ApiClient.MarketOpenClose.Interfaces;

namespace Surveillance.Reddeer.ApiClient.MarketOpenClose
{
    public class MarketOpenCloseApiCachingDecorator : IMarketOpenCloseApiCachingDecorator
    {
        private readonly IMarketOpenCloseApi _decorated;
        private readonly ILogger _logger;

        private IReadOnlyCollection<ExchangeDto> _cachedMarketData;

        private readonly TimeSpan _cacheLength;
        private DateTime _cacheExpiry;

        public MarketOpenCloseApiCachingDecorator(
            IMarketOpenCloseApi decorated,
            ILogger<MarketOpenCloseApiCachingDecorator> logger)
        {
            _cacheExpiry = DateTime.UtcNow.AddMilliseconds(-1);
            _cacheLength = TimeSpan.FromMinutes(30);

            _decorated = decorated ?? throw new ArgumentNullException(nameof(decorated));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IReadOnlyCollection<ExchangeDto>> Get()
        {
            if (DateTime.UtcNow < _cacheExpiry)
            {
                return _cachedMarketData;
            }

            _logger.LogInformation("Fetching market open/close data in the cached repository");
            _cachedMarketData = await _decorated.Get();
            _cacheExpiry = DateTime.UtcNow.Add(_cacheLength);

            return _cachedMarketData;
        }

        public async Task<bool> HeartBeating(CancellationToken token)
        {
            if (_decorated == null)
            {
                return false;
            }

            return await _decorated.HeartBeating(token);
        }
    }
}
