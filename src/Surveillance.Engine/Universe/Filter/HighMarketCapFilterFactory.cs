namespace Surveillance.Engine.Rules.Universe.Filter
{
    using System;

    using Microsoft.Extensions.Logging;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
    using Surveillance.Engine.Rules.Factories.Interfaces;
    using Surveillance.Engine.Rules.Markets.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.Filter;
    using Surveillance.Engine.Rules.Rules;
    using Surveillance.Engine.Rules.Universe.Filter.Interfaces;

    public class HighMarketCapFilterFactory : IHighMarketCapFilterFactory
    {
        private readonly ILoggerFactory _loggerFactory;

        private readonly IMarketTradingHoursService _tradingHoursService;

        private readonly IUniverseMarketCacheFactory _universeMarketCacheFactory;

        public HighMarketCapFilterFactory(
            IUniverseMarketCacheFactory universeMarketCacheFactory,
            IMarketTradingHoursService tradingHoursService,
            ILoggerFactory loggerFactory)
        {
            this._universeMarketCacheFactory = universeMarketCacheFactory
                                               ?? throw new ArgumentNullException(nameof(universeMarketCacheFactory));
            this._tradingHoursService =
                tradingHoursService ?? throw new ArgumentNullException(nameof(tradingHoursService));
            this._loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        public IHighMarketCapFilter Build(
            RuleRunMode ruleRunMode,
            DecimalRangeRuleFilter marketCap,
            string ruleName,
            IUniverseDataRequestsSubscriber universeDataRequestsSubscriber,
            ISystemProcessOperationRunRuleContext operationRunRuleContext)
        {
            return new HighMarketCapFilter(
                this._universeMarketCacheFactory,
                ruleRunMode,
                marketCap,
                this._tradingHoursService,
                operationRunRuleContext,
                universeDataRequestsSubscriber,
                ruleName,
                this._loggerFactory.CreateLogger<HighMarketCapFilter>());
        }
    }
}