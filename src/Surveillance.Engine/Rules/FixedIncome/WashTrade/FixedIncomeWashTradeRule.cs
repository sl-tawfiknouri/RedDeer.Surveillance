namespace Surveillance.Engine.Rules.Rules.FixedIncome.WashTrade
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Domain.Core.Trading;
    using Domain.Core.Trading.Factories.Interfaces;
    using Domain.Core.Trading.Orders;
    using Domain.Surveillance.Rules;
    using Domain.Surveillance.Rules.Interfaces;
    using Domain.Surveillance.Scheduling;

    using Microsoft.Extensions.Logging;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Data.Universe.Interfaces;
    using Surveillance.Data.Universe.MarketEvents;
    using Surveillance.Engine.Rules.Analytics.Streams;
    using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
    using Surveillance.Engine.Rules.Factories.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.FixedIncome.Interfaces;
    using Surveillance.Engine.Rules.Rules.FixedIncome.WashTrade.Interfaces;
    using Surveillance.Engine.Rules.Rules.Interfaces;
    using Surveillance.Engine.Rules.Rules.Shared.WashTrade;
    using Surveillance.Engine.Rules.Rules.Shared.WashTrade.Interfaces;
    using Surveillance.Engine.Rules.Trades;
    using Surveillance.Engine.Rules.Trades.Interfaces;
    using Surveillance.Engine.Rules.Universe.Filter.Interfaces;

    /// <summary>
    /// The fixed income wash trade rule.
    /// </summary>
    public class FixedIncomeWashTradeRule : BaseUniverseRule, IFixedIncomeWashTradeRule
    {
        /// <summary>
        /// The alert stream.
        /// </summary>
        private readonly IUniverseAlertStream alertStream;

        /// <summary>
        /// The clustering service.
        /// </summary>
        private readonly IClusteringService clusteringService;

        /// <summary>
        /// The order filter service.
        /// </summary>
        private readonly IUniverseFixedIncomeOrderFilterService orderFilterService;

        /// <summary>
        /// The parameters.
        /// </summary>
        private readonly IWashTradeRuleFixedIncomeParameters parameters;

        /// <summary>
        /// The portfolio factory.
        /// </summary>
        private readonly IPortfolioFactory portfolioFactory;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<FixedIncomeWashTradeRule> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="FixedIncomeWashTradeRule"/> class.
        /// </summary>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <param name="orderFilterService">
        /// The order filter service.
        /// </param>
        /// <param name="ruleContext">
        /// The rule context.
        /// </param>
        /// <param name="equityFactory">
        /// The factory.
        /// </param>
        /// <param name="fixedIncomeFactory">
        /// The factory.
        /// </param>
        /// <param name="runMode">
        /// The run mode.
        /// </param>
        /// <param name="alertStream">
        /// The alert stream.
        /// </param>
        /// <param name="clusteringService">
        /// The clustering service.
        /// </param>
        /// <param name="portfolioFactory">
        /// The portfolio factory.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="tradingStackLogger">
        /// The trading stack logger.
        /// </param>
        public FixedIncomeWashTradeRule(
            IWashTradeRuleFixedIncomeParameters parameters,
            IUniverseFixedIncomeOrderFilterService orderFilterService,
            ISystemProcessOperationRunRuleContext ruleContext,
            IUniverseEquityMarketCacheFactory equityFactory,
            IUniverseFixedIncomeMarketCacheFactory fixedIncomeFactory,
            RuleRunMode runMode,
            IUniverseAlertStream alertStream,
            IClusteringService clusteringService,
            IPortfolioFactory portfolioFactory,
            ILogger<FixedIncomeWashTradeRule> logger,
            ILogger<TradingHistoryStack> tradingStackLogger)
            : base(
                parameters?.Windows?.BackwardWindowSize ?? TimeSpan.FromDays(1),
                parameters?.Windows?.BackwardWindowSize ?? TimeSpan.FromDays(1),
                parameters?.Windows?.FutureWindowSize ?? TimeSpan.Zero,
                Rules.FixedIncomeWashTrades,
                Versioner.Version(1, 0),
                $"{nameof(FixedIncomeWashTradeRule)}",
                ruleContext,
                equityFactory,
                fixedIncomeFactory,
                runMode,
                logger,
                tradingStackLogger)
        {
            this.parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            this.orderFilterService = orderFilterService ?? throw new ArgumentNullException(nameof(orderFilterService));
            this.alertStream = alertStream ?? throw new ArgumentNullException(nameof(alertStream));
            this.clusteringService = clusteringService ?? throw new ArgumentNullException(nameof(clusteringService));
            this.portfolioFactory = portfolioFactory ?? throw new ArgumentNullException(nameof(portfolioFactory));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Gets or sets the organisation factor value.
        /// </summary>
        public IFactorValue OrganisationFactorValue { get; set; } = FactorValue.None;

        /// <summary>
        /// The clone.
        /// </summary>
        /// <param name="factor">
        /// The factor.
        /// </param>
        /// <returns>
        /// The <see cref="IUniverseCloneableRule"/>.
        /// </returns>
        public IUniverseCloneableRule Clone(IFactorValue factor)
        {
            var clone = (FixedIncomeWashTradeRule)this.Clone();
            clone.OrganisationFactorValue = factor;

            return clone;
        }

        /// <summary>
        /// The clone.
        /// </summary>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public object Clone()
        {
            this.logger.LogInformation($"Clone called at {this.UniverseDateTime}");

            var clone = (FixedIncomeWashTradeRule)this.MemberwiseClone();
            clone.BaseClone();

            this.logger.LogInformation($"Clone completed for {this.UniverseDateTime}");

            return clone;
        }

        /// <summary>
        /// See if trades net out to near zero i.e. large amount of churn
        /// </summary>
        /// <param name="tradingHistory">
        /// The trading History.
        /// </param>
        /// <returns>
        /// The <see cref="WashTradeAveragePositionBreach"/>.
        /// </returns>
        public WashTradeRuleBreach.WashTradeAveragePositionBreach NettingTrades(IReadOnlyCollection<Order> tradingHistory)
        {
            if (!this.parameters.PerformAveragePositionAnalysis) return WashTradeRuleBreach.WashTradeAveragePositionBreach.None();

            if (tradingHistory == null || !tradingHistory.Any()) return WashTradeRuleBreach.WashTradeAveragePositionBreach.None();

            if (tradingHistory.Count < this.parameters.AveragePositionMinimumNumberOfTrades)
                return WashTradeRuleBreach.WashTradeAveragePositionBreach.None();

            var portfolio = this.portfolioFactory.Build();
            portfolio.Add(tradingHistory);

            foreach (var profitAndLoss in portfolio.ProfitAndLossTotal())
            {
                if (profitAndLoss.PercentageProfits() == null) continue;

                if (Math.Abs(profitAndLoss.PercentageProfits().GetValueOrDefault(0))
                    > this.parameters.AveragePositionMaximumPositionValueChange.GetValueOrDefault(0))
                    return WashTradeRuleBreach.WashTradeAveragePositionBreach.None();

                return new WashTradeRuleBreach.WashTradeAveragePositionBreach(
                    true,
                    tradingHistory.Count,
                    profitAndLoss.PercentageProfits(),
                    null);
            }

            return WashTradeRuleBreach.WashTradeAveragePositionBreach.None();
        }

        /// <summary>
        /// The run order filled event.
        /// </summary>
        /// <param name="history">
        /// The history.
        /// </param>
        public override void RunOrderFilledEvent(ITradingHistoryStack history)
        {
            this.logger.LogInformation($"RunOrderFilledEvent called at {this.UniverseDateTime}");

            this.logger.LogInformation($"RunOrderFilledEvent completed for {this.UniverseDateTime}");
        }

        /// <summary>
        /// The run order filled event delayed.
        /// </summary>
        /// <param name="history">
        /// The history.
        /// </param>
        public override void RunOrderFilledEventDelayed(ITradingHistoryStack history)
        {
            // do nothing
        }

        /// <summary>
        /// The data constraints.
        /// </summary>
        /// <returns>
        /// The <see cref="IRuleDataConstraint"/>.
        /// </returns>
        public override IRuleDataConstraint DataConstraints()
        {
            return RuleDataConstraint.Empty().Case;
        }

        /// <summary>
        /// The end of universe.
        /// </summary>
        protected override void EndOfUniverse()
        {
            this.logger.LogInformation($"EndOfUniverse called at {this.UniverseDateTime}");

            this.RuleCtx?.EndEvent();

            this.logger.LogInformation($"EndOfUniverse completed for {this.UniverseDateTime}");
        }

        /// <summary>
        /// The filter.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="IUniverseEvent"/>.
        /// </returns>
        protected override IUniverseEvent Filter(IUniverseEvent value)
        {
            return this.orderFilterService.Filter(value);
        }

        /// <summary>
        /// The genesis.
        /// </summary>
        protected override void Genesis()
        {
            this.logger.LogInformation($"Genesis called at {this.UniverseDateTime}");

            this.logger.LogInformation($"Genesis completed for {this.UniverseDateTime}");
        }

        /// <summary>
        /// The market close.
        /// </summary>
        /// <param name="exchange">
        /// The exchange.
        /// </param>
        protected override void MarketClose(MarketOpenClose exchange)
        {
            this.logger.LogInformation($"MarketClose called at {this.UniverseDateTime} for {exchange.MarketId}");

            this.logger.LogInformation($"MarketClose completed for {this.UniverseDateTime} for {exchange.MarketId}");
        }

        /// <summary>
        /// The market open.
        /// </summary>
        /// <param name="exchange">
        /// The exchange.
        /// </param>
        protected override void MarketOpen(MarketOpenClose exchange)
        {
            this.logger.LogInformation($"MarketOpen called at {this.UniverseDateTime} for {exchange.MarketId}");

            this.logger.LogInformation($"MarketOpen completed for {this.UniverseDateTime} for {exchange.MarketId}");
        }

        /// <summary>
        /// The run initial submission event.
        /// </summary>
        /// <param name="history">
        /// The history.
        /// </param>
        protected override void RunInitialSubmissionEvent(ITradingHistoryStack history)
        {
            this.logger.LogInformation($"RunInitialSubmissionRule called at {this.UniverseDateTime}");

            this.logger.LogInformation($"RunInitialSubmissionRule completed for {this.UniverseDateTime}");
        }

        /// <summary>
        /// The run initial submission event delayed.
        /// </summary>
        /// <param name="history">
        /// The history.
        /// </param>
        protected override void RunInitialSubmissionEventDelayed(ITradingHistoryStack history)
        {
            // do nothing
        }

        /// <summary>
        /// The run post order event.
        /// </summary>
        /// <param name="history">
        /// The history.
        /// </param>
        protected override void RunPostOrderEvent(ITradingHistoryStack history)
        {
            this.logger.LogInformation($"RunPostOrderEvent called at {this.UniverseDateTime}");

            var filteredOrders = this.FilterByClientAccount(
                history.ActiveTradeHistory().Any() ? history.ActiveTradeHistory().Peek() : null,
                history.ActiveTradeHistory().ToList());

            var clusteringAnalysis = this.ClusteringAnalysis(filteredOrders);
            var averageNettingAnalysis = this.NettingTrades(filteredOrders);

            if ((clusteringAnalysis == null || !clusteringAnalysis.ClusteringPositionBreach)
                && (averageNettingAnalysis == null || !averageNettingAnalysis.AveragePositionRuleBreach)) return;

            var security = filteredOrders?.FirstOrDefault()?.Instrument;

            // wrong but should be a judgement anyway
            var breach = new WashTradeRuleBreach(
                this.parameters.Windows.BackwardWindowSize,
                this.OrganisationFactorValue,
                this.RuleCtx.SystemProcessOperationContext(),
                this.RuleCtx.CorrelationId(),
                this.parameters,
                new TradePosition(filteredOrders),
                security,
                averageNettingAnalysis,
                clusteringAnalysis,
                null,
                null,
                this.UniverseDateTime);

            var universeAlert = new UniverseAlertEvent(Rules.FixedIncomeWashTrades, breach, this.RuleCtx);
            this.alertStream.Add(universeAlert);

            this.logger.LogInformation($"RunPostOrderEvent completed for {this.UniverseDateTime}");
        }

        /// <summary>
        /// The run post order event delayed.
        /// </summary>
        /// <param name="history">
        /// The history.
        /// </param>
        protected override void RunPostOrderEventDelayed(ITradingHistoryStack history)
        {
            // do nothing
        }

        /// <summary>
        /// The analyze cluster.
        /// </summary>
        /// <param name="centroid">
        /// The centroid.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool AnalyseCluster(PositionClusterCentroid centroid)
        {
            if (centroid == null) return false;

            var clusterPorfolio = this.portfolioFactory.Build();
            clusterPorfolio.Add(centroid.Buys.Get().ToList());
            clusterPorfolio.Add(centroid.Sells.Get().ToList());

            if (clusterPorfolio.Ledger.FullLedger().Count
                < this.parameters.ClusteringPositionMinimumNumberOfTrades) return false;

            foreach (var profitAndLoss in clusterPorfolio.ProfitAndLossTotal())
            {
                if (profitAndLoss.Costs.Value == 0 || profitAndLoss.Revenue.Value == 0) continue;

                var largerValue = Math.Max(profitAndLoss.Costs.Value, profitAndLoss.Revenue.Value);
                var smallerValue = Math.Min(profitAndLoss.Costs.Value, profitAndLoss.Revenue.Value);

                var offset =
                    largerValue * this.parameters.ClusteringPercentageValueDifferenceThreshold.GetValueOrDefault(0);
                var lowerBoundary = largerValue - offset;
                var upperBoundary = largerValue + offset;

                if (smallerValue >= lowerBoundary && smallerValue <= upperBoundary) return true;
            }

            return false;
        }

        /// <summary>
        /// The clustering analysis.
        /// </summary>
        /// <param name="tradingHistory">
        /// The trading history.
        /// </param>
        /// <returns>
        /// The <see cref="WashTradeClusteringPositionBreach"/>.
        /// </returns>
        private WashTradeRuleBreach.WashTradeClusteringPositionBreach ClusteringAnalysis(IReadOnlyCollection<Order> tradingHistory)
        {
            if (!this.parameters.PerformClusteringPositionAnalysis) return WashTradeRuleBreach.WashTradeClusteringPositionBreach.None();

            if (tradingHistory == null || !tradingHistory.Any()) return WashTradeRuleBreach.WashTradeClusteringPositionBreach.None();

            var portfolio = this.portfolioFactory.Build();
            portfolio.Add(tradingHistory);

            var clusters = this.clusteringService.Cluster(portfolio.Ledger.FullLedger());

            if (clusters == null || !clusters.Any()) return WashTradeRuleBreach.WashTradeClusteringPositionBreach.None();

            var washTradingClusters = clusters.Where(this.AnalyseCluster).Select(i => i.CentroidPrice).ToList();

            if (!washTradingClusters.Any()) return WashTradeRuleBreach.WashTradeClusteringPositionBreach.None();

            return new WashTradeRuleBreach.WashTradeClusteringPositionBreach(true, washTradingClusters.Count, washTradingClusters);
        }

        /// <summary>
        /// The filter by client account.
        /// </summary>
        /// <param name="mostRecentFrame">
        /// The most recent frame.
        /// </param>
        /// <param name="frames">
        /// The frames.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        private List<Order> FilterByClientAccount(Order mostRecentFrame, List<Order> frames)
        {
            if (frames == null) return new List<Order>();

            var liveTrades = frames.Where(at => at.OrderStatus() == OrderStatus.Filled)
                .Where(at => at.OrderAverageFillPrice != null).ToList();

            if (!string.IsNullOrWhiteSpace(mostRecentFrame?.OrderClientAccountAttributionId))
                liveTrades = liveTrades.Where(
                    lt => string.Equals(
                        lt.OrderClientAccountAttributionId,
                        mostRecentFrame.OrderClientAccountAttributionId,
                        StringComparison.InvariantCultureIgnoreCase)).ToList();

            return liveTrades;
        }
    }
}