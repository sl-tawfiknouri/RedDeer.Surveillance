using Microsoft.Extensions.Logging;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
using Surveillance.Engine.Rules.Factories.Interfaces;
using Surveillance.Engine.Rules.Markets.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Filter;
using Surveillance.Engine.Rules.Rules;
using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
using System;

namespace Surveillance.Engine.Rules.Universe.Filter
{
    using Surveillance.Engine.Rules.Currency.Interfaces;

    public class HighMarketCapFilterFactory : IHighMarketCapFilterFactory
    {
        private readonly ICurrencyConverterService currencyConverterService;
        private readonly IUniverseEquityMarketCacheFactory _universeMarketCacheFactory;
        private readonly IMarketTradingHoursService _tradingHoursService;
        private readonly ILoggerFactory _loggerFactory;

        public HighMarketCapFilterFactory(
            ICurrencyConverterService currencyConverterService,
            IUniverseEquityMarketCacheFactory universeMarketCacheFactory,
            IMarketTradingHoursService tradingHoursService,
            ILoggerFactory loggerFactory)
        {
            this.currencyConverterService = currencyConverterService ?? throw new ArgumentNullException(nameof(currencyConverterService));
            _universeMarketCacheFactory = universeMarketCacheFactory ?? throw new ArgumentNullException(nameof(universeMarketCacheFactory));
            _tradingHoursService = tradingHoursService ?? throw new ArgumentNullException(nameof(tradingHoursService));
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        public IHighMarketCapFilter Build(RuleRunMode ruleRunMode, 
            DecimalRangeRuleFilter marketCap, 
            string ruleName, 
            IUniverseDataRequestsSubscriber universeDataRequestsSubscriber, 
            ISystemProcessOperationRunRuleContext operationRunRuleContext)
        {
            return new HighMarketCapFilter(
                _universeMarketCacheFactory, 
                ruleRunMode,
                marketCap, 
                _tradingHoursService, 
                operationRunRuleContext,
                universeDataRequestsSubscriber,
                this.currencyConverterService,
                ruleName,
                _loggerFactory.CreateLogger<HighMarketCapFilter>());
        }
    }
}
