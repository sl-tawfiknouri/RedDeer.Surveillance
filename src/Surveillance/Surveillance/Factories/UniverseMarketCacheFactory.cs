using System;
using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.Aurora.BMLL.Interfaces;
using Surveillance.Factories.Interfaces;
using Surveillance.Markets;
using Surveillance.Markets.Interfaces;

namespace Surveillance.Factories
{
    public class UniverseMarketCacheFactory : IUniverseMarketCacheFactory
    {
        private readonly IBmllDataRequestRepository _dataRequestRepository;
        private readonly ILogger<UniverseMarketCacheFactory> _logger;

        public UniverseMarketCacheFactory(
            IBmllDataRequestRepository dataRequestRepository,
            ILogger<UniverseMarketCacheFactory> logger)
        {
            _dataRequestRepository = dataRequestRepository ?? throw new ArgumentNullException(nameof(dataRequestRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IUniverseMarketCache Build(TimeSpan window)
        {
            return new UniverseMarketCache(window, _dataRequestRepository, _logger);
        }
    }
}
