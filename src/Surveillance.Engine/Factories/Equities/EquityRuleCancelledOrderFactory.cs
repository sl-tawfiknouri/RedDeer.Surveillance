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
        private readonly IUniverseMarketCacheFactory _factory;

        private readonly ILogger<CancelledOrderRule> _logger;

        private readonly IUniverseEquityOrderFilterService _orderFilterService;

        private readonly ILogger<TradingHistoryStack> _tradingHistoryLogger;

        public EquityRuleCancelledOrderFactory(
            IUniverseEquityOrderFilterService orderFilterService,
            IUniverseMarketCacheFactory factory,
            ILogger<CancelledOrderRule> logger,
            ILogger<TradingHistoryStack> tradingHistoryLogger)
        {
            this._orderFilterService =
                orderFilterService ?? throw new ArgumentNullException(nameof(orderFilterService));
            this._factory = factory ?? throw new ArgumentNullException(nameof(factory));
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
                this._factory,
                runMode,
                this._logger,
                this._tradingHistoryLogger);
        }
    }
}