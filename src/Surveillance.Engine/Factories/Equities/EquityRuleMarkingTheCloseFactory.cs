using System;
using Microsoft.Extensions.Logging;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
using Surveillance.Engine.Rules.Factories.Equities.Interfaces;
using Surveillance.Engine.Rules.Factories.Interfaces;
using Surveillance.Engine.Rules.Markets.Interfaces;
using Surveillance.Engine.Rules.Rules;
using Surveillance.Engine.Rules.Rules.Equity.MarkingTheClose;
using Surveillance.Engine.Rules.Rules.Equity.MarkingTheClose.Interfaces;
using Surveillance.Engine.Rules.Trades;
using Surveillance.Engine.Rules.Universe.Filter.Interfaces;

namespace Surveillance.Engine.Rules.Factories.Equities
{
    public class EquityRuleMarkingTheCloseFactory : IEquityRuleMarkingTheCloseFactory
    {
        private readonly IUniverseEquityOrderFilterService _orderFilterService;
        private readonly IUniverseMarketCacheFactory _factory;
        private readonly IMarketTradingHoursService _tradingHoursService;
        private readonly ILogger<MarkingTheCloseRule> _logger;
        private readonly ILogger<TradingHistoryStack> _tradingHistoryLogger;

        public EquityRuleMarkingTheCloseFactory(
            IUniverseEquityOrderFilterService orderFilterService,
            IUniverseMarketCacheFactory factory,
            IMarketTradingHoursService tradingHoursService,
            ILogger<MarkingTheCloseRule> logger,
            ILogger<TradingHistoryStack> tradingHistoryLogger)
        {
            _orderFilterService = orderFilterService ?? throw new ArgumentNullException(nameof(orderFilterService));
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _tradingHoursService = tradingHoursService ?? throw new ArgumentNullException(nameof(tradingHoursService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tradingHistoryLogger = tradingHistoryLogger ?? throw new ArgumentNullException(nameof(tradingHistoryLogger));
        }

        public IMarkingTheCloseRule Build(
            IMarkingTheCloseEquitiesParameters equitiesParameters,
            ISystemProcessOperationRunRuleContext ruleCtx,
            IUniverseAlertStream alertStream,
            RuleRunMode runMode,
            IUniverseDataRequestsSubscriber dataRequestSubscriber)
        {
            return new MarkingTheCloseRule(
                equitiesParameters,
                alertStream,
                ruleCtx,
                _orderFilterService, 
                _factory,
                _tradingHoursService,
                dataRequestSubscriber,
                runMode,
                _logger,
                _tradingHistoryLogger);
        }

        public static string Version => Versioner.Version(1, 0);
    }
}
