namespace Surveillance.Engine.Rules.Factories.Equities
{
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

    public class EquityRuleSpoofingFactory : IEquityRuleSpoofingFactory
    {
        private readonly IUniverseMarketCacheFactory _factory;

        private readonly ILogger<SpoofingRule> _logger;

        private readonly IOrderAnalysisService _orderAnalysisService;

        private readonly IUniverseEquityOrderFilterService _orderFilterService;

        private readonly IPortfolioFactory _portfolioFactory;

        private readonly ILogger<TradingHistoryStack> _tradingHistoryLogger;

        public EquityRuleSpoofingFactory(
            IUniverseMarketCacheFactory factory,
            IUniverseEquityOrderFilterService orderFilterService,
            IPortfolioFactory portfolioFactory,
            IOrderAnalysisService analysisService,
            ILogger<SpoofingRule> logger,
            ILogger<TradingHistoryStack> tradingHistoryLogger)
        {
            this._orderFilterService =
                orderFilterService ?? throw new ArgumentNullException(nameof(orderFilterService));
            this._factory = factory ?? throw new ArgumentNullException(nameof(factory));
            this._portfolioFactory = portfolioFactory ?? throw new ArgumentNullException(nameof(portfolioFactory));
            this._orderAnalysisService = analysisService ?? throw new ArgumentNullException(nameof(analysisService));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._tradingHistoryLogger =
                tradingHistoryLogger ?? throw new ArgumentNullException(nameof(tradingHistoryLogger));
        }

        public static string Version => Versioner.Version(3, 0);

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
                this._orderFilterService,
                this._factory,
                runMode,
                this._portfolioFactory,
                this._orderAnalysisService,
                this._logger,
                this._tradingHistoryLogger);
        }
    }
}