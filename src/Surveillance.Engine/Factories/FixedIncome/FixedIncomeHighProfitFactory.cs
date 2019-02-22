using System;
using Microsoft.Extensions.Logging;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
using Surveillance.Engine.Rules.Factories.FixedIncome.Interfaces;
using Surveillance.Engine.Rules.Factories.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.FixedIncome.Interfaces;
using Surveillance.Engine.Rules.Rules;
using Surveillance.Engine.Rules.Rules.FixedIncome.HighProfits;
using Surveillance.Engine.Rules.Rules.FixedIncome.HighProfits.Interfaces;
using Surveillance.Engine.Rules.Trades;
using Surveillance.Engine.Rules.Universe.Filter.Interfaces;

namespace Surveillance.Engine.Rules.Factories.FixedIncome
{
    public class FixedIncomeHighProfitFactory : IFixedIncomeHighProfitFactory
    {
        private readonly IUniverseFixedIncomeOrderFilter _fixedIncomeOrderFilter;
        private readonly IUniverseMarketCacheFactory _marketCacheFactory;
        private readonly ILogger<FixedIncomeHighProfitsRule> _logger;
        private readonly ILogger<TradingHistoryStack> _stackLogger;

        public FixedIncomeHighProfitFactory(
            IUniverseFixedIncomeOrderFilter fixedIncomeOrderFilter,
            IUniverseMarketCacheFactory marketCacheFactory,
            ILogger<FixedIncomeHighProfitsRule> logger, 
            ILogger<TradingHistoryStack> stackLogger)
        {
            _fixedIncomeOrderFilter = fixedIncomeOrderFilter ?? throw new ArgumentNullException(nameof(fixedIncomeOrderFilter));
            _marketCacheFactory = marketCacheFactory ?? throw new ArgumentNullException(nameof(marketCacheFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _stackLogger = stackLogger ?? throw new ArgumentNullException(nameof(stackLogger));
        }

        public IFixedIncomeHighProfitsRule BuildRule(
            IHighProfitsRuleFixedIncomeParameters parameters,
            ISystemProcessOperationRunRuleContext ruleCtx,
            IUniverseAlertStream alertStream,
            RuleRunMode runMode)
        {
            return new FixedIncomeHighProfitsRule(
                parameters,
                _fixedIncomeOrderFilter,
                ruleCtx,
                _marketCacheFactory,
                runMode,
                alertStream,
                _logger,
                _stackLogger);
        }

        public static string Version => Versioner.Version(1, 0);
    }
}
