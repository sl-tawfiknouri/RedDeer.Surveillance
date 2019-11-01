namespace Surveillance.Engine.Rules.Factories.Equities
{
    using System;

    using Microsoft.Extensions.Logging;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
    using Surveillance.Engine.Rules.Factories.Equities.Interfaces;
    using Surveillance.Engine.Rules.Factories.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
    using Surveillance.Engine.Rules.Rules;
    using Surveillance.Engine.Rules.Rules.Equity.CancelledOrders;
    using Surveillance.Engine.Rules.Rules.Equity.CancelledOrders.Interfaces;
    using Surveillance.Engine.Rules.Trades;
    using Surveillance.Engine.Rules.Universe.Filter.Interfaces;

    public class EquityRuleCancelledOrderFactory : IEquityRuleCancelledOrderFactory
    {
        private readonly IUniverseEquityMarketCacheFactory _equityFactory;

        private readonly IUniverseFixedIncomeMarketCacheFactory _fixedIncomeFactory;

        private readonly ILogger<CancelledOrderRule> _logger;

        private readonly IUniverseEquityOrderFilterService _orderFilterService;

        private readonly ILogger<TradingHistoryStack> _tradingHistoryLogger;

        public EquityRuleCancelledOrderFactory(
            IUniverseEquityOrderFilterService orderFilterService,
            IUniverseEquityMarketCacheFactory equityFactory,
            IUniverseFixedIncomeMarketCacheFactory fixedIncomeFactory,
            ILogger<CancelledOrderRule> logger,
            ILogger<TradingHistoryStack> tradingHistoryLogger)
        {
            this._orderFilterService =
                orderFilterService ?? throw new ArgumentNullException(nameof(orderFilterService));
            this._equityFactory = equityFactory ?? throw new ArgumentNullException(nameof(equityFactory));
            this._fixedIncomeFactory = fixedIncomeFactory ?? throw new ArgumentNullException(nameof(fixedIncomeFactory));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._tradingHistoryLogger = tradingHistoryLogger;
        }

        public static string Version => Versioner.Version(2, 0);

        public ICancelledOrderRule Build(
            ICancelledOrderRuleEquitiesParameters parameters,
            ISystemProcessOperationRunRuleContext ruleCtx,
            IUniverseAlertStream alertStream,
            RuleRunMode runMode)
        {
            return new CancelledOrderRule(
                parameters,
                ruleCtx,
                alertStream,
                this._orderFilterService,
                this._equityFactory,
                this._fixedIncomeFactory,
                runMode,
                this._logger,
                this._tradingHistoryLogger);
        }
    }
}