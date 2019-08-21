namespace Surveillance.Engine.Rules.Factories.Equities
{
    using System;

    using Microsoft.Extensions.Logging;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
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
        private readonly IUniverseMarketCacheFactory _factory;

        private readonly ILogger<IHighVolumeRule> _logger;

        private readonly IUniverseEquityOrderFilterService _orderFilterService;

        private readonly ILogger<TradingHistoryStack> _tradingHistoryLogger;

        private readonly IMarketTradingHoursService _tradingHoursService;

        public EquityRuleHighVolumeFactory(
            IUniverseEquityOrderFilterService orderFilterService,
            IUniverseMarketCacheFactory factory,
            IMarketTradingHoursService tradingHoursService,
            ILogger<IHighVolumeRule> logger,
            ILogger<TradingHistoryStack> tradingHistoryLogger)
        {
            this._orderFilterService =
                orderFilterService ?? throw new ArgumentNullException(nameof(orderFilterService));
            this._factory = factory ?? throw new ArgumentNullException(nameof(factory));
            this._tradingHoursService =
                tradingHoursService ?? throw new ArgumentNullException(nameof(tradingHoursService));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._tradingHistoryLogger =
                tradingHistoryLogger ?? throw new ArgumentNullException(nameof(tradingHistoryLogger));
        }

        public static string Version => Versioner.Version(1, 0);

        public IHighVolumeRule Build(
            IHighVolumeRuleEquitiesParameters equitiesParameters,
            ISystemProcessOperationRunRuleContext opCtx,
            IUniverseAlertStream alertStream,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            RuleRunMode runMode)
        {
            return new HighVolumeRule(
                equitiesParameters,
                opCtx,
                alertStream,
                this._orderFilterService,
                this._factory,
                this._tradingHoursService,
                dataRequestSubscriber,
                runMode,
                this._logger,
                this._tradingHistoryLogger);
        }
    }
}