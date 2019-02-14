using System;
using Microsoft.Extensions.Logging;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
using Surveillance.Engine.Rules.Factories.Interfaces;
using Surveillance.Engine.Rules.Markets.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Interfaces;
using Surveillance.Engine.Rules.Rules;
using Surveillance.Engine.Rules.Rules.Layering;
using Surveillance.Engine.Rules.Rules.Layering.Interfaces;
using Surveillance.Engine.Rules.Trades;
using Surveillance.Engine.Rules.Universe.Filter.Interfaces;

namespace Surveillance.Engine.Rules.Factories
{
    public class LayeringRuleFactory : ILayeringRuleFactory
    {
        private readonly IUniverseOrderFilter _orderFilter;
        private readonly IMarketTradingHoursManager _tradingHoursManager;
        private readonly IUniverseMarketCacheFactory _factory;
        private readonly ILogger<LayeringRuleFactory> _logger;
        private readonly ILogger<TradingHistoryStack> _tradingHistoryLogger;

        public LayeringRuleFactory(
            IUniverseOrderFilter orderFilter,
            IMarketTradingHoursManager tradingHoursManager,
            IUniverseMarketCacheFactory factory,
            ILogger<LayeringRuleFactory> logger,
            ILogger<TradingHistoryStack> tradingHistoryLogger)
        {
            _orderFilter = orderFilter ?? throw new ArgumentNullException(nameof(orderFilter));
            _tradingHoursManager = tradingHoursManager ?? throw new ArgumentNullException(nameof(tradingHoursManager));
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tradingHistoryLogger = tradingHistoryLogger ?? throw new ArgumentNullException(nameof(tradingHistoryLogger));
        }

        public ILayeringRule Build(
            ILayeringRuleParameters parameters,
            ISystemProcessOperationRunRuleContext ruleCtx,
            IUniverseAlertStream alertStream,
            RuleRunMode runMode)
        {
            return new LayeringRule(
                parameters,
                alertStream,
                _orderFilter,
                _logger,
                _factory,
                _tradingHoursManager,
                ruleCtx,
                runMode,
                _tradingHistoryLogger);
        }

        public static string Version => Versioner.Version(1, 0);
    }
}
