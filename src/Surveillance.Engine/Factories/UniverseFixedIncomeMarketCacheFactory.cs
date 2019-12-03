using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.Aurora.BMLL.Interfaces;
using Surveillance.Engine.Rules.Factories.Interfaces;
using Surveillance.Engine.Rules.Markets;
using Surveillance.Engine.Rules.Markets.Interfaces;
using Surveillance.Engine.Rules.Rules;
using System;
using System.Collections.Generic;
using System.Text;

namespace Surveillance.Engine.Rules.Factories
{
    public class UniverseFixedIncomeMarketCacheFactory : IUniverseFixedIncomeMarketCacheFactory
    {
        private readonly IRuleRunDataRequestRepository _dataRequestRepository;

        private readonly ILogger<UniverseFixedIncomeMarketCacheFactory> _logger;

        private readonly IStubRuleRunDataRequestRepository _stubDataRequestRepository;

        public UniverseFixedIncomeMarketCacheFactory(
            IStubRuleRunDataRequestRepository stubDataRequestRepository,
            IRuleRunDataRequestRepository dataRequestRepository,
            ILogger<UniverseFixedIncomeMarketCacheFactory> logger)
        {
            this._stubDataRequestRepository = stubDataRequestRepository
                                              ?? throw new ArgumentNullException(nameof(stubDataRequestRepository));
            this._dataRequestRepository =
                dataRequestRepository ?? throw new ArgumentNullException(nameof(dataRequestRepository));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IUniverseFixedIncomeInterDayCache BuildInterday(RuleRunMode runMode)
        {
            var repo = runMode == RuleRunMode.ValidationRun
                           ? this._dataRequestRepository
                           : this._stubDataRequestRepository;

            return new UniverseFixedIncomeInterDayCache(repo, this._logger);
        }

        public IUniverseFixedIncomeIntraDayCache BuildIntraday(TimeSpan window, RuleRunMode runMode)
        {
            var repo = runMode == RuleRunMode.ValidationRun
                           ? this._dataRequestRepository
                           : this._stubDataRequestRepository;

            return new UniverseFixedIncomeIntraDayCache(window, repo, this._logger);
        }
    }
}
