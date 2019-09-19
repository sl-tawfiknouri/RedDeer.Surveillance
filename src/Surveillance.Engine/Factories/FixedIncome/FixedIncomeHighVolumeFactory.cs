namespace Surveillance.Engine.Rules.Factories.FixedIncome
{
    using System;

    using Microsoft.Extensions.Logging;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
    using Surveillance.Engine.Rules.Factories.FixedIncome.Interfaces;
    using Surveillance.Engine.Rules.Factories.Interfaces;
    using Surveillance.Engine.Rules.Judgements.Interfaces;
    using Surveillance.Engine.Rules.Markets.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.FixedIncome.Interfaces;
    using Surveillance.Engine.Rules.Rules;
    using Surveillance.Engine.Rules.Rules.FixedIncome.HighVolume;
    using Surveillance.Engine.Rules.Rules.FixedIncome.HighVolume.Interfaces;
    using Surveillance.Engine.Rules.Trades;
    using Surveillance.Engine.Rules.Universe.Filter.Interfaces;

    /// <summary>
    /// The fixed income high volume factory.
    /// </summary>
    public class FixedIncomeHighVolumeFactory : IFixedIncomeHighVolumeFactory
    {
        /// <summary>
        /// The filter service.
        /// </summary>
        private readonly IUniverseFixedIncomeOrderFilterService filterService;

        /// <summary>
        /// The market cache factory.
        /// </summary>
        private readonly IUniverseMarketCacheFactory marketCacheFactory;

        /// <summary>
        /// The market trading hours service.
        /// </summary>
        private readonly IMarketTradingHoursService marketTradingHoursService;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<FixedIncomeHighVolumeRule> logger;

        /// <summary>
        /// The trading logger required for the base class.
        /// </summary>
        private readonly ILogger<TradingHistoryStack> tradingLogger;

        /// <summary>
        /// Initializes a new instance of the <see cref="FixedIncomeHighVolumeFactory"/> class.
        /// </summary>
        /// <param name="filterService">
        /// The filter service.
        /// </param>
        /// <param name="marketCacheFactory">
        /// The market cache factory.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="tradingLogger">
        /// The trading logger.
        /// </param>
        /// <param name="marketTradingHoursService">
        /// The trading hours service.
        /// </param>
        public FixedIncomeHighVolumeFactory(
            IUniverseFixedIncomeOrderFilterService filterService,
            IUniverseMarketCacheFactory marketCacheFactory,
            ILogger<FixedIncomeHighVolumeRule> logger,
            ILogger<TradingHistoryStack> tradingLogger,
            IMarketTradingHoursService marketTradingHoursService)
        {
            this.filterService = filterService ?? throw new ArgumentNullException(nameof(filterService));
            this.marketCacheFactory =
                marketCacheFactory ?? throw new ArgumentNullException(nameof(this.marketCacheFactory));
            this.marketTradingHoursService = marketTradingHoursService ?? throw new ArgumentNullException(nameof(marketTradingHoursService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.tradingLogger = tradingLogger ?? throw new ArgumentNullException(nameof(tradingLogger));
        }

        /// <summary>
        /// The version.
        /// </summary>
        public static string Version => Versioner.Version(1, 0);

        /// <summary>
        /// The build rule method to return a new high volume issuance rule with each call.
        /// </summary>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <param name="operationContext">
        /// The operation context.
        /// </param>
        /// <param name="judgementService">
        /// The judgement service/
        /// </param>
        /// <param name="dataRequestSubscriber">
        /// The data request subscriber.
        /// </param>
        /// <param name="runMode">
        /// The run mode.
        /// </param>
        /// <returns>
        /// The <see cref="IFixedIncomeHighVolumeRule"/>.
        /// </returns>
        public IFixedIncomeHighVolumeRule BuildRule(
            IHighVolumeIssuanceRuleFixedIncomeParameters parameters,
            ISystemProcessOperationRunRuleContext operationContext,
            IFixedIncomeHighVolumeJudgementService judgementService,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            RuleRunMode runMode)
        {
            return new FixedIncomeHighVolumeRule(
                parameters,
                this.filterService,
                operationContext,
                this.marketCacheFactory,
                judgementService,
                dataRequestSubscriber,
                this.marketTradingHoursService,
                runMode,
                this.logger,
                this.tradingLogger);
        }
    }
}