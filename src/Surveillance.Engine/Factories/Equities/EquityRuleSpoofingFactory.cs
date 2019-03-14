using System;
using Domain.Core.Trading.Execution.Interfaces;
using Domain.Core.Trading.Factories.Interfaces;
using Microsoft.Extensions.Logging;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
using Surveillance.Engine.Rules.Factories.Equities.Interfaces;
using Surveillance.Engine.Rules.Factories.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
using Surveillance.Engine.Rules.Rules;
using Surveillance.Engine.Rules.Rules.Equity.Spoofing;
using Surveillance.Engine.Rules.Rules.Equity.Spoofing.Interfaces;
using Surveillance.Engine.Rules.Trades;
using Surveillance.Engine.Rules.Universe.Filter.Interfaces;

namespace Surveillance.Engine.Rules.Factories.Equities
{
    public class EquityRuleSpoofingFactory : IEquityRuleSpoofingFactory
    {
        private readonly IUniverseEquityOrderFilterService _orderFilterService;
        private readonly IUniverseMarketCacheFactory _factory;
        private readonly IPortfolioFactory _portfolioFactory;
        private readonly IOrderAnalysisService _orderAnalysisService;
        private readonly ILogger<SpoofingRule> _logger;
        private readonly ILogger<TradingHistoryStack> _tradingHistoryLogger;

        public EquityRuleSpoofingFactory(IUniverseMarketCacheFactory factory,
            IUniverseEquityOrderFilterService orderFilterService,
            IPortfolioFactory portfolioFactory,
            IOrderAnalysisService analysisService,
            ILogger<SpoofingRule> logger,
            ILogger<TradingHistoryStack> tradingHistoryLogger)
        {
            _orderFilterService = orderFilterService ?? throw new ArgumentNullException(nameof(orderFilterService));
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _portfolioFactory = portfolioFactory ?? throw new ArgumentNullException(nameof(portfolioFactory));
            _orderAnalysisService = analysisService ?? throw new ArgumentNullException(nameof(analysisService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tradingHistoryLogger = tradingHistoryLogger ?? throw new ArgumentNullException(nameof(tradingHistoryLogger));
        }

        public ISpoofingRule Build(
            ISpoofingRuleEquitiesParameters spoofingEquitiesParameters,
            ISystemProcessOperationRunRuleContext ruleCtx,
            IUniverseAlertStream alertStream,
            RuleRunMode runMode)
        {
            return new SpoofingRule(
                spoofingEquitiesParameters,
                ruleCtx,
                alertStream,
                _orderFilterService,
                _factory,
                runMode,
                _portfolioFactory,
                _orderAnalysisService,
                _logger,
                _tradingHistoryLogger);
        }

        public static string Version => Versioner.Version(3, 0);
    }
}
