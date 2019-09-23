using static Surveillance.Engine.Rules.Rules.Shared.WashTrade.WashTradeRuleBreach;

namespace Surveillance.Engine.Rules.Rules.FixedIncome.WashTrade
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Domain.Core.Trading.Factories.Interfaces;
    using Domain.Core.Trading.Orders;
    using Domain.Surveillance.Scheduling;

    using Microsoft.Extensions.Logging;

    using Surveillance.Auditing.Context.Interfaces;
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
    using Surveillance.Engine.Rules.Universe.Interfaces;
    using Surveillance.Engine.Rules.Universe.MarketEvents;

    public class FixedIncomeWashTradeRule : BaseUniverseRule, IFixedIncomeWashTradeRule
    {
        private readonly IUniverseAlertStream _alertStream;

        private readonly IClusteringService _clusteringService;

        private readonly ILogger<FixedIncomeWashTradeRule> _logger;

        private readonly IUniverseFixedIncomeOrderFilterService _orderFilterService;

        private readonly IWashTradeRuleFixedIncomeParameters _parameters;

        private readonly IPortfolioFactory _portfolioFactory;

        public FixedIncomeWashTradeRule(
            IWashTradeRuleFixedIncomeParameters parameters,
            IUniverseFixedIncomeOrderFilterService orderFilterService,
            ISystemProcessOperationRunRuleContext ruleCtx,
            IUniverseMarketCacheFactory factory,
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
                ruleCtx,
                factory,
                runMode,
                logger,
                tradingStackLogger)
        {
            this._parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            this._orderFilterService =
                orderFilterService ?? throw new ArgumentNullException(nameof(orderFilterService));
            this._alertStream = alertStream ?? throw new ArgumentNullException(nameof(alertStream));
            this._clusteringService = clusteringService ?? throw new ArgumentNullException(nameof(clusteringService));
            this._portfolioFactory = portfolioFactory ?? throw new ArgumentNullException(nameof(portfolioFactory));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IFactorValue OrganisationFactorValue { get; set; } = FactorValue.None;

        public IUniverseCloneableRule Clone(IFactorValue factor)
        {
            var clone = (FixedIncomeWashTradeRule)this.Clone();
            clone.OrganisationFactorValue = factor;

            return clone;
        }

        public object Clone()
        {
            this._logger.LogInformation($"Clone called at {this.UniverseDateTime}");

            var clone = (FixedIncomeWashTradeRule)this.MemberwiseClone();
            clone.BaseClone();

            this._logger.LogInformation($"Clone completed for {this.UniverseDateTime}");

            return clone;
        }

        /// <summary>
        ///     See if trades net out to near zero i.e. large amount of churn
        /// </summary>
        public WashTradeAveragePositionBreach NettingTrades(IReadOnlyCollection<Order> tradingHistory)
        {
            if (!this._parameters.PerformAveragePositionAnalysis) return WashTradeAveragePositionBreach.None();

            if (tradingHistory == null || !tradingHistory.Any()) return WashTradeAveragePositionBreach.None();

            if (tradingHistory.Count < this._parameters.AveragePositionMinimumNumberOfTrades)
                return WashTradeAveragePositionBreach.None();

            var portfolio = this._portfolioFactory.Build();
            portfolio.Add(tradingHistory);

            foreach (var profitAndLoss in portfolio.ProfitAndLossTotal())
            {
                if (profitAndLoss.PercentageProfits() == null) continue;

                if (Math.Abs(profitAndLoss.PercentageProfits().GetValueOrDefault(0))
                    > this._parameters.AveragePositionMaximumPositionValueChange.GetValueOrDefault(0))
                    return WashTradeAveragePositionBreach.None();

                return new WashTradeAveragePositionBreach(
                    true,
                    tradingHistory.Count,
                    profitAndLoss.PercentageProfits(),
                    null);
            }

            return WashTradeAveragePositionBreach.None();
        }

        public override void RunOrderFilledEvent(ITradingHistoryStack history)
        {
            this._logger.LogInformation($"RunOrderFilledEvent called at {this.UniverseDateTime}");

            this._logger.LogInformation($"RunOrderFilledEvent completed for {this.UniverseDateTime}");
        }

        public override void RunOrderFilledEventDelayed(ITradingHistoryStack history)
        {
            // do nothing
        }

        protected override void EndOfUniverse()
        {
            this._logger.LogInformation($"EndOfUniverse called at {this.UniverseDateTime}");

            this.RuleCtx?.EndEvent();

            this._logger.LogInformation($"EndOfUniverse completed for {this.UniverseDateTime}");
        }

        protected override IUniverseEvent Filter(IUniverseEvent value)
        {
            return this._orderFilterService.Filter(value);
        }

        protected override void Genesis()
        {
            this._logger.LogInformation($"Genesis called at {this.UniverseDateTime}");

            this._logger.LogInformation($"Genesis completed for {this.UniverseDateTime}");
        }

        protected override void MarketClose(MarketOpenClose exchange)
        {
            this._logger.LogInformation($"MarketClose called at {this.UniverseDateTime} for {exchange.MarketId}");

            this._logger.LogInformation($"MarketClose completed for {this.UniverseDateTime} for {exchange.MarketId}");
        }

        protected override void MarketOpen(MarketOpenClose exchange)
        {
            this._logger.LogInformation($"MarketOpen called at {this.UniverseDateTime} for {exchange.MarketId}");

            this._logger.LogInformation($"MarketOpen completed for {this.UniverseDateTime} for {exchange.MarketId}");
        }

        protected override void RunInitialSubmissionEvent(ITradingHistoryStack history)
        {
            this._logger.LogInformation($"RunInitialSubmissionRule called at {this.UniverseDateTime}");

            this._logger.LogInformation($"RunInitialSubmissionRule completed for {this.UniverseDateTime}");
        }

        protected override void RunInitialSubmissionEventDelayed(ITradingHistoryStack history)
        {
            // do nothing
        }

        protected override void RunPostOrderEvent(ITradingHistoryStack history)
        {
            this._logger.LogInformation($"RunPostOrderEvent called at {this.UniverseDateTime}");

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
                this._parameters.Windows.BackwardWindowSize,
                this.OrganisationFactorValue,
                this.RuleCtx.SystemProcessOperationContext(),
                this.RuleCtx.CorrelationId(),
                this._parameters,
                new TradePosition(filteredOrders),
                security,
                averageNettingAnalysis,
                clusteringAnalysis,
                null,
                null,
                this.UniverseDateTime);

            var universeAlert = new UniverseAlertEvent(Rules.FixedIncomeWashTrades, breach, this.RuleCtx);
            this._alertStream.Add(universeAlert);

            this._logger.LogInformation($"RunPostOrderEvent completed for {this.UniverseDateTime}");
        }

        protected override void RunPostOrderEventDelayed(ITradingHistoryStack history)
        {
            // do nothing
        }

        private bool AnalyseCluster(PositionClusterCentroid centroid)
        {
            if (centroid == null) return false;

            var clusterPorfolio = this._portfolioFactory.Build();
            clusterPorfolio.Add(centroid.Buys.Get().ToList());
            clusterPorfolio.Add(centroid.Sells.Get().ToList());

            if (clusterPorfolio.Ledger.FullLedger().Count
                < this._parameters.ClusteringPositionMinimumNumberOfTrades) return false;

            foreach (var profitAndLoss in clusterPorfolio.ProfitAndLossTotal())
            {
                if (profitAndLoss.Costs.Value == 0 || profitAndLoss.Revenue.Value == 0) continue;

                var largerValue = Math.Max(profitAndLoss.Costs.Value, profitAndLoss.Revenue.Value);
                var smallerValue = Math.Min(profitAndLoss.Costs.Value, profitAndLoss.Revenue.Value);

                var offset =
                    largerValue * this._parameters.ClusteringPercentageValueDifferenceThreshold.GetValueOrDefault(0);
                var lowerBoundary = largerValue - offset;
                var upperBoundary = largerValue + offset;

                if (smallerValue >= lowerBoundary && smallerValue <= upperBoundary) return true;
            }

            return false;
        }

        private WashTradeClusteringPositionBreach ClusteringAnalysis(IReadOnlyCollection<Order> tradingHistory)
        {
            if (!this._parameters.PerformClusteringPositionAnalysis) return WashTradeClusteringPositionBreach.None();

            if (tradingHistory == null || !tradingHistory.Any()) return WashTradeClusteringPositionBreach.None();

            var portfolio = this._portfolioFactory.Build();
            portfolio.Add(tradingHistory);

            var clusters = this._clusteringService.Cluster(portfolio.Ledger.FullLedger());

            if (clusters == null || !clusters.Any()) return WashTradeClusteringPositionBreach.None();

            var washTradingClusters = clusters.Where(this.AnalyseCluster).Select(i => i.CentroidPrice).ToList();

            if (!washTradingClusters.Any()) return WashTradeClusteringPositionBreach.None();

            return new WashTradeClusteringPositionBreach(true, washTradingClusters.Count, washTradingClusters);
        }

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