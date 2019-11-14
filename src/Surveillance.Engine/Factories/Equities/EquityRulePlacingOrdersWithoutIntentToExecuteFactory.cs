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
    using Surveillance.Engine.Rules.Rules.Equity.PlacingOrderNoIntentToExecute;
    using Surveillance.Engine.Rules.Rules.Equity.PlacingOrderNoIntentToExecute.Interfaces;
    using Surveillance.Engine.Rules.Trades;
    using Surveillance.Engine.Rules.Universe.Filter.Interfaces;

    public class
        EquityRulePlacingOrdersWithoutIntentToExecuteFactory : IEquityRulePlacingOrdersWithoutIntentToExecuteFactory
    {
        private readonly IUniverseEquityMarketCacheFactory _equityFactory;

        private readonly IUniverseFixedIncomeMarketCacheFactory _fixedIncomeFactory;

        private readonly ILogger<PlacingOrdersWithNoIntentToExecuteRule> _logger;

        private readonly IUniverseEquityOrderFilterService _orderFilterService;

        private readonly ILogger<TradingHistoryStack> _tradingHistoryLogger;

        private readonly IMarketTradingHoursService _tradingHoursService;

        public EquityRulePlacingOrdersWithoutIntentToExecuteFactory(
            IUniverseEquityOrderFilterService orderFilterService,
            IUniverseEquityMarketCacheFactory equityFactory,
            IUniverseFixedIncomeMarketCacheFactory fixedIncomeFactory,
            IMarketTradingHoursService tradingHoursService,
            ILogger<PlacingOrdersWithNoIntentToExecuteRule> logger,
            ILogger<TradingHistoryStack> tradingHistoryLogger)
        {
            this._orderFilterService =
                orderFilterService ?? throw new ArgumentNullException(nameof(orderFilterService));
            this._equityFactory = equityFactory ?? throw new ArgumentNullException(nameof(equityFactory));
            this._fixedIncomeFactory = fixedIncomeFactory ?? throw new ArgumentNullException(nameof(fixedIncomeFactory));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._tradingHistoryLogger =
                tradingHistoryLogger ?? throw new ArgumentNullException(nameof(tradingHistoryLogger));
            this._tradingHoursService =
                tradingHoursService ?? throw new ArgumentNullException(nameof(tradingHoursService));
        }

        public static string Version => Versioner.Version(1, 0);

        public IPlacingOrdersWithNoIntentToExecuteRule Build(
            IPlacingOrderWithNoIntentToExecuteRuleEquitiesParameters parameters,
            IUniverseAlertStream alertStream,
            ISystemProcessOperationRunRuleContext ruleCtx,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            RuleRunMode runMode)
        {
            return new PlacingOrdersWithNoIntentToExecuteRule(
                parameters,
                this._orderFilterService,
                ruleCtx,
                this._equityFactory,
                this._fixedIncomeFactory,
                alertStream,
                dataRequestSubscriber,
                this._tradingHoursService,
                runMode,
                this._logger,
                this._tradingHistoryLogger);
        }
    }
}