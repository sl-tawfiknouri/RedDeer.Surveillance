using System;
using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.Aurora.BMLL.Interfaces;
using Surveillance.Factories.Interfaces;
using Surveillance.Markets;
using Surveillance.Markets.Interfaces;
using Surveillance.Rules;

namespace Surveillance.Factories
{
    public class UniverseMarketCacheFactory : IUniverseMarketCacheFactory
    {
        private readonly IStubBmllDataRequestRepository _stubDataRequestRepository;
        private readonly IBmllDataRequestRepository _dataRequestRepository;
        private readonly ILogger<UniverseMarketCacheFactory> _logger;

        public UniverseMarketCacheFactory(
            IStubBmllDataRequestRepository stubDataRequestRepository,
            IBmllDataRequestRepository dataRequestRepository,
            ILogger<UniverseMarketCacheFactory> logger)
        {
            _stubDataRequestRepository = stubDataRequestRepository ?? throw new ArgumentNullException(nameof(stubDataRequestRepository));
            _dataRequestRepository = dataRequestRepository ?? throw new ArgumentNullException(nameof(dataRequestRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IUniverseMarketCache Build(TimeSpan window, RuleRunMode runMode)
        {
            var repo = runMode == RuleRunMode.ValidationRun
                ? _dataRequestRepository
                : _stubDataRequestRepository;

            return new UniverseMarketCache(window, repo, _logger);
        }
    }
}
