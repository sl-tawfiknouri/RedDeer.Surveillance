namespace Surveillance.Engine.Rules.Factories
{
    using System;

    using Microsoft.Extensions.Logging;

    using Surveillance.DataLayer.Aurora.BMLL.Interfaces;
    using Surveillance.Engine.Rules.Factories.Interfaces;
    using Surveillance.Engine.Rules.Markets;
    using Surveillance.Engine.Rules.Markets.Interfaces;
    using Surveillance.Engine.Rules.Rules;

    public class UniverseEquityMarketCacheFactory : IUniverseEquityMarketCacheFactory
    {
        private readonly IRuleRunDataRequestRepository _dataRequestRepository;

        private readonly ILogger<UniverseEquityMarketCacheFactory> _logger;

        private readonly IStubRuleRunDataRequestRepository _stubDataRequestRepository;

        public UniverseEquityMarketCacheFactory(
            IStubRuleRunDataRequestRepository stubDataRequestRepository,
            IRuleRunDataRequestRepository dataRequestRepository,
            ILogger<UniverseEquityMarketCacheFactory> logger)
        {
            this._stubDataRequestRepository = stubDataRequestRepository
                                              ?? throw new ArgumentNullException(nameof(stubDataRequestRepository));
            this._dataRequestRepository =
                dataRequestRepository ?? throw new ArgumentNullException(nameof(dataRequestRepository));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IUniverseEquityInterDayCache BuildInterday(RuleRunMode runMode)
        {
            var repo = runMode == RuleRunMode.ValidationRun
                           ? this._dataRequestRepository
                           : this._stubDataRequestRepository;

            return new UniverseEquityInterDayCache(repo, this._logger);
        }

        public IUniverseEquityIntraDayCache BuildIntraday(TimeSpan window, RuleRunMode runMode)
        {
            var repo = runMode == RuleRunMode.ValidationRun
                           ? this._dataRequestRepository
                           : this._stubDataRequestRepository;

            return new UniverseEquityIntraDayCache(window, repo, this._logger);
        }
    }
}