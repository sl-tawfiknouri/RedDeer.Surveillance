﻿using System;
using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.Aurora.BMLL.Interfaces;
using Surveillance.Engine.Rules.Factories.Interfaces;
using Surveillance.Engine.Rules.Markets;
using Surveillance.Engine.Rules.Markets.Interfaces;
using Surveillance.Engine.Rules.Rules;

namespace Surveillance.Engine.Rules.Factories
{
    public class UniverseMarketCacheFactory : IUniverseMarketCacheFactory
    {
        private readonly IStubRuleRunDataRequestRepository _stubDataRequestRepository;
        private readonly IRuleRunDataRequestRepository _dataRequestRepository;
        private readonly ILogger<UniverseMarketCacheFactory> _logger;

        public UniverseMarketCacheFactory(
            IStubRuleRunDataRequestRepository stubDataRequestRepository,
            IRuleRunDataRequestRepository dataRequestRepository,
            ILogger<UniverseMarketCacheFactory> logger)
        {
            _stubDataRequestRepository = stubDataRequestRepository ?? throw new ArgumentNullException(nameof(stubDataRequestRepository));
            _dataRequestRepository = dataRequestRepository ?? throw new ArgumentNullException(nameof(dataRequestRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IUniverseEquityIntradayCache BuildIntraday(TimeSpan window, RuleRunMode runMode)
        {
            var repo = runMode == RuleRunMode.ValidationRun
                ? _dataRequestRepository
                : _stubDataRequestRepository;

            return new UniverseEquityIntradayCache(window, repo, _logger);
        }

        public IUniverseEquityInterDayCache BuildInterday(RuleRunMode runMode)
        {
            var repo = runMode == RuleRunMode.ValidationRun
                ? _dataRequestRepository
                : _stubDataRequestRepository;

            return new UniverseEquityInterDayCache(repo, _logger);
        }
    }
}