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

    /// <summary>
    /// The fixed income high profit factory.
    /// </summary>
    public class FixedIncomeHighProfitFactory : IFixedIncomeHighProfitFactory
    {
        /// <summary>
        /// The fixed income order filter service.
        /// </summary>
        private readonly IUniverseFixedIncomeOrderFilterService fixedIncomeOrderFilterService;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<FixedIncomeHighProfitsRule> logger;

        /// <summary>
        /// The market cache factory.
        /// </summary>
        private readonly IUniverseMarketCacheFactory marketCacheFactory;

        /// <summary>
        /// The market data cache strategy factory.
        /// </summary>
        private readonly IMarketDataCacheStrategyFactory marketDataCacheStrategyFactory;

        /// <summary>
        /// The cost calculator factory.
        /// </summary>
        private readonly ICostCalculatorFactory costCalculatorFactory;

        /// <summary>
        /// The revenue calculator factory.
        /// </summary>
        private readonly IRevenueCalculatorFactory revenueCalculatorFactory;

        /// <summary>
        /// The exchange rate profit calculator.
        /// </summary>
        private readonly IExchangeRateProfitCalculator exchangeRateProfitCalculator;

        /// <summary>
        /// The stack logger.
        /// </summary>
        private readonly ILogger<TradingHistoryStack> stackLogger;

        /// <summary>
        /// Initializes a new instance of the <see cref="FixedIncomeHighProfitFactory"/> class.
        /// </summary>
        /// <param name="fixedIncomeOrderFilterService">
        /// The fixed income order filter service.
        /// </param>
        /// <param name="marketCacheFactory">
        /// The market cache factory.
        /// </param>
        /// <param name="marketDataCacheStrategyFactory">
        /// The market data cache strategy factory.
        /// </param>
        /// <param name="costCalculatorFactory">
        /// The cost calculator factory.
        /// </param>
        /// <param name="revenueCalculatorFactory">
        /// The revenue calculator factory.
        /// </param>
        /// <param name="exchangeRateProfitCalculator">
        /// The exchange rate profit calculator.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="stackLogger">
        /// The stack logger.
        /// </param>
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
            this.fixedIncomeOrderFilterService =
                fixedIncomeOrderFilterService ?? throw new ArgumentNullException(nameof(fixedIncomeOrderFilterService));
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

        /// <summary>
        /// The version of the rule.
        /// </summary>
        public static string Version => Versioner.Version(1, 0);

        /// <summary>
        /// The build rule.
        /// </summary>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <param name="ruleContext">
        /// The rule context.
        /// </param>
        /// <param name="judgementService">
        /// The judgement service.
        /// </param>
        /// <param name="dataRequestSubscriber">
        /// The data request subscriber.
        /// </param>
        /// <param name="runMode">
        /// The run mode.
        /// </param>
        /// <param name="scheduledExecution">
        /// The scheduled execution.
        /// </param>
        /// <returns>
        /// The <see cref="IFixedIncomeHighProfitsRule"/>.
        /// </returns>
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