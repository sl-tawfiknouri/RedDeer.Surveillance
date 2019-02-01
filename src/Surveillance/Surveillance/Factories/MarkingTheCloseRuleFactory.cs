using System;
using Microsoft.Extensions.Logging;
using Surveillance.Analytics.Streams.Interfaces;
using Surveillance.Factories.Interfaces;
using Surveillance.Markets.Interfaces;
using Surveillance.Rules;
using Surveillance.Rules.MarkingTheClose;
using Surveillance.Rules.MarkingTheClose.Interfaces;
using Surveillance.Systems.Auditing.Context.Interfaces;
using Surveillance.Trades;
using Surveillance.Universe.Filter.Interfaces;

namespace Surveillance.Factories
{
    public class MarkingTheCloseRuleFactory : IMarkingTheCloseRuleFactory
    {
        private readonly IUniverseOrderFilter _orderFilter;
        private readonly IUniverseMarketCacheFactory _factory;
        private readonly IMarketTradingHoursManager _tradingHoursManager;
        private readonly ILogger<MarkingTheCloseRule> _logger;
        private readonly ILogger<TradingHistoryStack> _tradingHistoryLogger;

        public MarkingTheCloseRuleFactory(IUniverseOrderFilter orderFilter,
            IUniverseMarketCacheFactory factory,
            IMarketTradingHoursManager tradingHoursManager,
            ILogger<MarkingTheCloseRule> logger,
            ILogger<TradingHistoryStack> tradingHistoryLogger)
        {
            _orderFilter = orderFilter ?? throw new ArgumentNullException(nameof(orderFilter));
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _tradingHoursManager = tradingHoursManager ?? throw new ArgumentNullException(nameof(tradingHoursManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tradingHistoryLogger = tradingHistoryLogger ?? throw new ArgumentNullException(nameof(tradingHistoryLogger));
        }

        public IMarkingTheCloseRule Build(
            IMarkingTheCloseParameters parameters,
            ISystemProcessOperationRunRuleContext ruleCtx,
            IUniverseAlertStream alertStream,
            RuleRunMode runMode)
        {
            return new MarkingTheCloseRule(
                parameters,
                alertStream,
                ruleCtx,
                _orderFilter, 
                _factory,
                _tradingHoursManager,
                runMode,
                _logger,
                _tradingHistoryLogger);
        }

        public static string Version => Versioner.Version(1, 0);
    }
}
