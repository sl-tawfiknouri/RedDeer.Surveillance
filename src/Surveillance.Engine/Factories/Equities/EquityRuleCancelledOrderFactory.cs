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

namespace Surveillance.Engine.Rules.Factories.Equities
{
    public class EquityRuleCancelledOrderFactory : IEquityRuleCancelledOrderFactory
    {
        private readonly IUniverseEquityOrderFilter _orderFilter;
        private readonly IUniverseMarketCacheFactory _factory;
        private readonly ILogger<CancelledOrderRule> _logger;
        private readonly ILogger<TradingHistoryStack> _tradingHistoryLogger;
        
        public EquityRuleCancelledOrderFactory(
            IUniverseEquityOrderFilter orderFilter,
            IUniverseMarketCacheFactory factory,
            ILogger<CancelledOrderRule> logger,
            ILogger<TradingHistoryStack> tradingHistoryLogger)
        {
            _orderFilter = orderFilter ?? throw new ArgumentNullException(nameof(orderFilter));
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tradingHistoryLogger = tradingHistoryLogger;
        }

        public ICancelledOrderRule Build(
            ICancelledOrderRuleEquitiesParameters parameters,
            ISystemProcessOperationRunRuleContext ruleCtx,
            IUniverseAlertStream alertStream,
            RuleRunMode runMode)
        {
            return new CancelledOrderRule(parameters, ruleCtx, alertStream, _orderFilter, _factory, runMode, _logger, _tradingHistoryLogger);
        }

        public static string Version => Versioner.Version(2, 0);
    }
}