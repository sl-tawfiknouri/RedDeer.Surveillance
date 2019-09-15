namespace Surveillance.Engine.Rules.Factories.FixedIncome
{
    using System;

    using Domain.Surveillance.Scheduling;

    using Microsoft.Extensions.Logging;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
    using Surveillance.Engine.Rules.Factories.FixedIncome.Interfaces;
    using Surveillance.Engine.Rules.Factories.Interfaces;
    using Surveillance.Engine.Rules.Judgements.Interfaces;
    using Surveillance.Engine.Rules.Markets.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.FixedIncome.Interfaces;
    using Surveillance.Engine.Rules.Rules;
    using Surveillance.Engine.Rules.Rules.FixedIncome.HighProfits;
    using Surveillance.Engine.Rules.Rules.FixedIncome.HighProfits.Interfaces;
    using Surveillance.Engine.Rules.Rules.Shared.HighProfits.Calculators.Factories.Interfaces;
    using Surveillance.Engine.Rules.Rules.Shared.HighProfits.Calculators.Interfaces;
    using Surveillance.Engine.Rules.Trades;
    using Surveillance.Engine.Rules.Universe.Filter.Interfaces;

    public class FixedIncomeHighProfitFactory : IFixedIncomeHighProfitFactory
    {
        private readonly IUniverseFixedIncomeOrderFilterService fixedIncomeOrderFilterService;

        private readonly ILogger<FixedIncomeHighProfitsRule> logger;

        private readonly IUniverseMarketCacheFactory marketCacheFactory;

        private readonly IMarketDataCacheStrategyFactory marketDataCacheStrategyFactory;

        private readonly ICostCalculatorFactory costCalculatorFactory;

        private readonly IRevenueCalculatorFactory revenueCalculatorFactory;

        private readonly IExchangeRateProfitCalculator exchangeRateProfitCalculator;
        
        private readonly ILogger<TradingHistoryStack> stackLogger;

        public FixedIncomeHighProfitFactory(
            IUniverseFixedIncomeOrderFilterService fixedIncomeOrderFilterService,
            IUniverseMarketCacheFactory marketCacheFactory,
            IMarketDataCacheStrategyFactory marketDataCacheStrategyFactory,
            ICostCalculatorFactory costCalculatorFactory,
            IRevenueCalculatorFactory revenueCalculatorFactory,
            IExchangeRateProfitCalculator exchangeRateProfitCalculator,
            ILogger<FixedIncomeHighProfitsRule> logger,
            ILogger<TradingHistoryStack> stackLogger)
        {
            this.fixedIncomeOrderFilterService = fixedIncomeOrderFilterService
                                                  ?? throw new ArgumentNullException(
                                                      nameof(fixedIncomeOrderFilterService));
            this.marketCacheFactory =
                marketCacheFactory ?? throw new ArgumentNullException(nameof(marketCacheFactory));
            this.marketDataCacheStrategyFactory = marketDataCacheStrategyFactory ?? throw new ArgumentNullException(nameof(marketDataCacheStrategyFactory));
            this.costCalculatorFactory =
                costCalculatorFactory ?? throw new ArgumentNullException(nameof(costCalculatorFactory));
            this.revenueCalculatorFactory = revenueCalculatorFactory ?? throw new ArgumentNullException(nameof(revenueCalculatorFactory));
            this.exchangeRateProfitCalculator = exchangeRateProfitCalculator ?? throw new ArgumentNullException(nameof(exchangeRateProfitCalculator));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.stackLogger = stackLogger ?? throw new ArgumentNullException(nameof(stackLogger));
        }

        public static string Version => Versioner.Version(1, 0);

        public IFixedIncomeHighProfitsRule BuildRule(
            IHighProfitsRuleFixedIncomeParameters parameters,
            ISystemProcessOperationRunRuleContext ruleContext,
            IFixedIncomeHighProfitJudgementService judgementService,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            RuleRunMode runMode,
            ScheduledExecution scheduledExecution)
        {
            var fixedIncomeStreamRule = new FixedIncomeHighProfitsStreamRule(
                parameters,
                ruleContext,
                this.costCalculatorFactory,
                this.revenueCalculatorFactory,
                this.exchangeRateProfitCalculator,
       this.fixedIncomeOrderFilterService,
                this.marketCacheFactory,
                this.marketDataCacheStrategyFactory,
                dataRequestSubscriber,
                judgementService,
                runMode,
                this.logger,
                this.stackLogger);

            return new FixedIncomeHighProfitsRule(
                parameters,
                fixedIncomeStreamRule,
                this.logger);
        }
    }
}