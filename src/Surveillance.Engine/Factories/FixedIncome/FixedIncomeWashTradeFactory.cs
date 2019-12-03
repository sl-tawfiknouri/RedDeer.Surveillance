namespace Surveillance.Engine.Rules.Factories.FixedIncome
{
    using System;

    using Domain.Core.Trading.Factories.Interfaces;

    using Microsoft.Extensions.Logging;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
    using Surveillance.Engine.Rules.Factories.FixedIncome.Interfaces;
    using Surveillance.Engine.Rules.Factories.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.FixedIncome.Interfaces;
    using Surveillance.Engine.Rules.Rules;
    using Surveillance.Engine.Rules.Rules.FixedIncome.WashTrade;
    using Surveillance.Engine.Rules.Rules.FixedIncome.WashTrade.Interfaces;
    using Surveillance.Engine.Rules.Rules.Shared.WashTrade.Interfaces;
    using Surveillance.Engine.Rules.Trades;
    using Surveillance.Engine.Rules.Universe.Filter.Interfaces;

    public class FixedIncomeWashTradeFactory : IFixedIncomeWashTradeFactory
    {
        private readonly IClusteringService _clusteringService;

        private readonly IUniverseFixedIncomeOrderFilterService _filterService;

        private readonly ILogger<FixedIncomeWashTradeRule> _logger;

        private readonly IUniverseEquityMarketCacheFactory _equityMarketCacheFactory;

        private readonly IUniverseFixedIncomeMarketCacheFactory _fixedIncomeMarketCacheFactory;

        private readonly IPortfolioFactory _portfolioFactory;

        private readonly ILogger<TradingHistoryStack> _tradingLogger;

        public FixedIncomeWashTradeFactory(
            IUniverseFixedIncomeOrderFilterService filterService,
            IUniverseEquityMarketCacheFactory equityMarketCacheFactory,
            IUniverseFixedIncomeMarketCacheFactory fixedIncomeMarketCacheFactory,
            IClusteringService clusteringService,
            IPortfolioFactory portfolioFactory,
            ILogger<FixedIncomeWashTradeRule> logger,
            ILogger<TradingHistoryStack> tradingLogger)
        {
            this._filterService = filterService ?? throw new ArgumentNullException(nameof(filterService));
            this._equityMarketCacheFactory =
                equityMarketCacheFactory ?? throw new ArgumentNullException(nameof(equityMarketCacheFactory));
            this._fixedIncomeMarketCacheFactory =
                fixedIncomeMarketCacheFactory ?? throw new ArgumentNullException(nameof(fixedIncomeMarketCacheFactory));
            this._clusteringService = clusteringService ?? throw new ArgumentNullException(nameof(clusteringService));
            this._portfolioFactory = portfolioFactory ?? throw new ArgumentNullException(nameof(portfolioFactory));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._tradingLogger = tradingLogger ?? throw new ArgumentNullException(nameof(tradingLogger));
        }

        public static string Version => Versioner.Version(1, 0);

        public IFixedIncomeWashTradeRule BuildRule(
            IWashTradeRuleFixedIncomeParameters parameters,
            ISystemProcessOperationRunRuleContext ruleCtx,
            IUniverseAlertStream alertStream,
            RuleRunMode runMode)
        {
            return new FixedIncomeWashTradeRule(
                parameters,
                this._filterService,
                ruleCtx,
                this._equityMarketCacheFactory,
                this._fixedIncomeMarketCacheFactory,
                runMode,
                alertStream,
                this._clusteringService,
                this._portfolioFactory,
                this._logger,
                this._tradingLogger);
        }
    }
}