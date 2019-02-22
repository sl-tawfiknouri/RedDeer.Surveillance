using System;
using Microsoft.Extensions.Logging;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
using Surveillance.Engine.Rules.Factories.FixedIncome.Interfaces;
using Surveillance.Engine.Rules.Factories.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.FixedIncome.Interfaces;
using Surveillance.Engine.Rules.Rules;
using Surveillance.Engine.Rules.Rules.FixedIncome.WashTrade;
using Surveillance.Engine.Rules.Rules.FixedIncome.WashTrade.Interfaces;
using Surveillance.Engine.Rules.Trades;
using Surveillance.Engine.Rules.Universe.Filter.Interfaces;

namespace Surveillance.Engine.Rules.Factories.FixedIncome
{
    public class FixedIncomeWashTradeFactory : IFixedIncomeWashTradeFactory
    {
        private readonly IUniverseFixedIncomeOrderFilter _filter;
        private readonly IUniverseMarketCacheFactory _marketCacheFactory;

        private readonly ILogger<FixedIncomeWashTradeRule> _logger;
        private readonly ILogger<TradingHistoryStack> _tradingLogger;

        public FixedIncomeWashTradeFactory(
            IUniverseFixedIncomeOrderFilter filter,
            IUniverseMarketCacheFactory marketCacheFactory,
            ILogger<FixedIncomeWashTradeRule> logger,
            ILogger<TradingHistoryStack> tradingLogger)
        {
            _filter = filter ?? throw new ArgumentNullException(nameof(filter));
            _marketCacheFactory = marketCacheFactory ?? throw new ArgumentNullException(nameof(marketCacheFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tradingLogger = tradingLogger ?? throw new ArgumentNullException(nameof(tradingLogger));
        }
        
        public IFixedIncomeWashTradeRule BuildRule(
            IWashTradeRuleFixedIncomeParameters parameters,
            ISystemProcessOperationRunRuleContext ruleCtx,
            IUniverseAlertStream alertStream,
            RuleRunMode runMode)
        {
            return new FixedIncomeWashTradeRule(
                parameters,
                _filter,
                ruleCtx,
                _marketCacheFactory,
                runMode,
                alertStream,
                _logger,
                _tradingLogger);
        }

        public static string Version => Versioner.Version(1, 0);
    }
}
