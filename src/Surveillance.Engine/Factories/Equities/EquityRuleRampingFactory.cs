using System;
using Microsoft.Extensions.Logging;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
using Surveillance.Engine.Rules.Factories.Equities.Interfaces;
using Surveillance.Engine.Rules.Factories.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
using Surveillance.Engine.Rules.Rules;
using Surveillance.Engine.Rules.Rules.Equity.Ramping;
using Surveillance.Engine.Rules.Rules.Equity.Ramping.Analysis.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.Ramping.Interfaces;
using Surveillance.Engine.Rules.Trades;
using Surveillance.Engine.Rules.Universe.Filter.Interfaces;

namespace Surveillance.Engine.Rules.Factories.Equities
{
    public class EquityRuleRampingFactory : IEquityRuleRampingFactory
    {
        private readonly IRampingAnalyser _rampingAnalyser;
        private readonly IUniverseEquityOrderFilterService _orderFilterService;
        private readonly IUniverseMarketCacheFactory _factory;
        private readonly ILogger<IRampingRule> _logger;
        private readonly ILogger<TradingHistoryStack> _tradingHistoryLogger;

        public EquityRuleRampingFactory(
            IRampingAnalyser rampingAnalyser,
            IUniverseEquityOrderFilterService orderFilterService,
            IUniverseMarketCacheFactory factory,
            ILogger<IRampingRule> logger,
            ILogger<TradingHistoryStack> tradingHistoryLogger)
        {
            _rampingAnalyser = rampingAnalyser ?? throw new ArgumentNullException(nameof(rampingAnalyser));
            _orderFilterService = orderFilterService ?? throw new ArgumentNullException(nameof(orderFilterService));
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tradingHistoryLogger = tradingHistoryLogger ?? throw new ArgumentNullException(nameof(tradingHistoryLogger));
        }

        public IRampingRule Build(
            IRampingRuleEquitiesParameters equitiesParameters,
            ISystemProcessOperationRunRuleContext ruleCtx,
            IUniverseAlertStream alertStream,
            RuleRunMode runMode,
            IUniverseDataRequestsSubscriber dataRequestSubscriber)
        {
            return new RampingRule(
                equitiesParameters,
                alertStream,
                ruleCtx,
                _factory,
                _orderFilterService,
                runMode,
                _rampingAnalyser,
                _logger,
                _tradingHistoryLogger);
        }

        public static string Version => Versioner.Version(1, 0);
    }
}
