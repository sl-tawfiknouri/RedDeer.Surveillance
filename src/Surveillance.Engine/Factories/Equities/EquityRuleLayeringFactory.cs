using System;
using Microsoft.Extensions.Logging;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
using Surveillance.Engine.Rules.Factories.Equities.Interfaces;
using Surveillance.Engine.Rules.Factories.Interfaces;
using Surveillance.Engine.Rules.Markets.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
using Surveillance.Engine.Rules.Rules;
using Surveillance.Engine.Rules.Rules.Equity.Layering;
using Surveillance.Engine.Rules.Rules.Equity.Layering.Interfaces;
using Surveillance.Engine.Rules.Trades;
using Surveillance.Engine.Rules.Universe.Filter.Interfaces;

namespace Surveillance.Engine.Rules.Factories.Equities
{
    public class EquityRuleLayeringFactory : IEquityRuleLayeringFactory
    {
        private readonly IUniverseEquityOrderFilterService _orderFilterService;
        private readonly IMarketTradingHoursService _tradingHoursService;
        private readonly IUniverseMarketCacheFactory _factory;
        private readonly ILogger<EquityRuleLayeringFactory> _logger;
        private readonly ILogger<TradingHistoryStack> _tradingHistoryLogger;

        public EquityRuleLayeringFactory(
            IUniverseEquityOrderFilterService orderFilterService,
            IMarketTradingHoursService tradingHoursService,
            IUniverseMarketCacheFactory factory,
            ILogger<EquityRuleLayeringFactory> logger,
            ILogger<TradingHistoryStack> tradingHistoryLogger)
        {
            _orderFilterService = orderFilterService ?? throw new ArgumentNullException(nameof(orderFilterService));
            _tradingHoursService = tradingHoursService ?? throw new ArgumentNullException(nameof(tradingHoursService));
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tradingHistoryLogger = tradingHistoryLogger ?? throw new ArgumentNullException(nameof(tradingHistoryLogger));
        }

        public ILayeringRule Build(
            ILayeringRuleEquitiesParameters equitiesParameters,
            ISystemProcessOperationRunRuleContext ruleCtx,
            IUniverseAlertStream alertStream,
            RuleRunMode runMode)
        {
            return new LayeringRule(
                equitiesParameters,
                alertStream,
                _orderFilterService,
                _logger,
                _factory,
                _tradingHoursService,
                ruleCtx,
                runMode,
                _tradingHistoryLogger);
        }

        public static string Version => Versioner.Version(1, 0);
    }
}
