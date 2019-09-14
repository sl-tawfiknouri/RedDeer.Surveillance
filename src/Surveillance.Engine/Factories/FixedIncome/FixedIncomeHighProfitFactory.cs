namespace Surveillance.Engine.Rules.Factories.FixedIncome
{
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

    public class FixedIncomeHighProfitFactory : IFixedIncomeHighProfitFactory
    {
        private readonly IUniverseFixedIncomeOrderFilterService _fixedIncomeOrderFilterService;

        private readonly ILogger<FixedIncomeHighProfitsRule> _logger;

        private readonly IUniverseMarketCacheFactory _marketCacheFactory;

        private readonly ILogger<TradingHistoryStack> _stackLogger;

        public FixedIncomeHighProfitFactory(
            IUniverseFixedIncomeOrderFilterService fixedIncomeOrderFilterService,
            IUniverseMarketCacheFactory marketCacheFactory,
            ILogger<FixedIncomeHighProfitsRule> logger,
            ILogger<TradingHistoryStack> stackLogger)
        {
            this._fixedIncomeOrderFilterService = fixedIncomeOrderFilterService
                                                  ?? throw new ArgumentNullException(
                                                      nameof(fixedIncomeOrderFilterService));
            this._marketCacheFactory =
                marketCacheFactory ?? throw new ArgumentNullException(nameof(marketCacheFactory));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._stackLogger = stackLogger ?? throw new ArgumentNullException(nameof(stackLogger));
        }

        public static string Version => Versioner.Version(1, 0);

        public IFixedIncomeHighProfitsRule BuildRule(
            IHighProfitsRuleFixedIncomeParameters parameters,
            ISystemProcessOperationRunRuleContext ruleCtx,
            IUniverseAlertStream alertStream,
            RuleRunMode runMode)
        {
            return new FixedIncomeHighProfitsRule(
                parameters,
                this._logger);

            //return new FixedIncomeHighProfitsRule(
            //    parameters,
            //    this._fixedIncomeOrderFilterService,
            //    ruleCtx,
            //    this._marketCacheFactory,
            //    runMode,
            //    alertStream,
            //    this._logger,
            //    this._stackLogger);
        }
    }
}