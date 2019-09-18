namespace Surveillance.Engine.Rules.Factories.FixedIncome
{
    using System;

    using Microsoft.Extensions.Logging;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.Rules.Factories.FixedIncome.Interfaces;
    using Surveillance.Engine.Rules.Factories.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.FixedIncome.Interfaces;
    using Surveillance.Engine.Rules.Rules;
    using Surveillance.Engine.Rules.Rules.FixedIncome.HighVolumeIssuance;
    using Surveillance.Engine.Rules.Rules.FixedIncome.HighVolumeIssuance.Interfaces;
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
        /// The logger.
        /// </summary>
        private readonly ILogger<FixedIncomeHighVolumeIssuanceRule> logger;

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
        public FixedIncomeHighVolumeFactory(
            IUniverseFixedIncomeOrderFilterService filterService,
            IUniverseMarketCacheFactory marketCacheFactory,
            ILogger<FixedIncomeHighVolumeIssuanceRule> logger,
            ILogger<TradingHistoryStack> tradingLogger)
        {
            this.filterService = filterService ?? throw new ArgumentNullException(nameof(filterService));
            this.marketCacheFactory =
                marketCacheFactory ?? throw new ArgumentNullException(nameof(this.marketCacheFactory));
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
        /// <param name="runMode">
        /// The run mode.
        /// </param>
        /// <returns>
        /// The <see cref="IFixedIncomeHighVolumeRule"/>.
        /// </returns>
        public IFixedIncomeHighVolumeRule BuildRule(
            IHighVolumeIssuanceRuleFixedIncomeParameters parameters,
            ISystemProcessOperationRunRuleContext operationContext,
            RuleRunMode runMode)
        {
            return new FixedIncomeHighVolumeIssuanceRule(
                parameters,
                this.filterService,
                operationContext,
                this.marketCacheFactory,
                runMode,
                this.logger,
                this.tradingLogger);
        }
    }
}