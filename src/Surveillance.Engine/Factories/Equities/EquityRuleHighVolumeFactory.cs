namespace Surveillance.Engine.Rules.Factories.Equities
{
    using System;

    using Microsoft.Extensions.Logging;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
    using Surveillance.Engine.Rules.Currency.Interfaces;
    using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
    using Surveillance.Engine.Rules.Factories.Equities.Interfaces;
    using Surveillance.Engine.Rules.Factories.Interfaces;
    using Surveillance.Engine.Rules.Markets.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
    using Surveillance.Engine.Rules.Rules;
    using Surveillance.Engine.Rules.Rules.Equity.HighVolume;
    using Surveillance.Engine.Rules.Rules.Equity.HighVolume.Interfaces;
    using Surveillance.Engine.Rules.Trades;
    using Surveillance.Engine.Rules.Universe.Filter.Interfaces;

    public class EquityRuleHighVolumeFactory : IEquityRuleHighVolumeFactory
    {
        private readonly IUniverseEquityOrderFilterService orderFilterService;
        private readonly IUniverseMarketCacheFactory factory;
        private readonly IMarketTradingHoursService tradingHoursService;
        private readonly ICurrencyConverterService currencyConverterService;
        private readonly ILogger<IHighVolumeRule> logger;
        private readonly ILogger<TradingHistoryStack> tradingHistoryLogger;

        public EquityRuleHighVolumeFactory(
            IUniverseEquityOrderFilterService orderFilterService,
            IUniverseMarketCacheFactory factory,
            IMarketTradingHoursService tradingHoursService,
            ICurrencyConverterService currencyConverterService,
            ILogger<IHighVolumeRule> logger,
            ILogger<TradingHistoryStack> tradingHistoryLogger)
        {
            this.orderFilterService = orderFilterService ?? throw new ArgumentNullException(nameof(orderFilterService));
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
            this.tradingHoursService = tradingHoursService ?? throw new ArgumentNullException(nameof(tradingHoursService));
            this.currencyConverterService = currencyConverterService ?? throw new ArgumentNullException(nameof(currencyConverterService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.tradingHistoryLogger = tradingHistoryLogger ?? throw new ArgumentNullException(nameof(tradingHistoryLogger));
        }

        public static string Version => Versioner.Version(1, 0);

        public IHighVolumeRule Build(
            IHighVolumeRuleEquitiesParameters equitiesParameters,
            ISystemProcessOperationRunRuleContext operationContext,
            IUniverseAlertStream alertStream,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            RuleRunMode runMode)
        {

            return new HighVolumeRule(
                equitiesParameters,
                operationContext,
                alertStream,
                this.orderFilterService,
                this.factory,
                this.tradingHoursService,
                dataRequestSubscriber,
                this.currencyConverterService,
                runMode,
                this.logger,
                this.tradingHistoryLogger);
        }
    }
}
