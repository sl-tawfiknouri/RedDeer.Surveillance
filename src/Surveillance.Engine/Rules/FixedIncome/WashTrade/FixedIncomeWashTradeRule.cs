using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Core.Trading.Factories.Interfaces;
using Domain.Core.Trading.Orders;
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
using static Surveillance.Engine.Rules.Rules.Shared.WashTrade.WashTradeRuleBreach;

namespace Surveillance.Engine.Rules.Rules.FixedIncome.WashTrade
{
    public class FixedIncomeWashTradeRule : BaseUniverseRule, IFixedIncomeWashTradeRule
    {
        private readonly IWashTradeRuleFixedIncomeParameters _parameters;
        private readonly IUniverseFixedIncomeOrderFilterService _orderFilterService;
        private readonly IUniverseAlertStream _alertStream;
        private readonly IClusteringService _clusteringService;
        private readonly IPortfolioFactory _portfolioFactory;
        private readonly ILogger<FixedIncomeWashTradeRule> _logger;

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
                parameters?.Windows?.FutureWindowSize ?? TimeSpan.Zero,
                Domain.Surveillance.Scheduling.Rules.FixedIncomeWashTrades,
                Versioner.Version(1, 0),
                $"{nameof(FixedIncomeWashTradeRule)}",
                ruleCtx,
                factory,
                runMode,
                logger,
                tradingStackLogger)
        {
            _parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            _orderFilterService = orderFilterService ?? throw new ArgumentNullException(nameof(orderFilterService));
            _alertStream = alertStream ?? throw new ArgumentNullException(nameof(alertStream));
            _clusteringService = clusteringService ?? throw new ArgumentNullException(nameof(clusteringService));
            _portfolioFactory = portfolioFactory ?? throw new ArgumentNullException(nameof(portfolioFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IFactorValue OrganisationFactorValue { get; set; } = FactorValue.None;

        protected override IUniverseEvent Filter(IUniverseEvent value)
        {
            return _orderFilterService.Filter(value);
        }

        protected override void RunPostOrderEvent(ITradingHistoryStack history)
        {
            _logger.LogInformation($"RunPostOrderEvent called at {UniverseDateTime}");

            var filteredOrders =
                FilterByClientAccount(
                    history.ActiveTradeHistory().Any() ? history.ActiveTradeHistory().Peek() : null,
                    history.ActiveTradeHistory().ToList());

            var clusteringAnalysis = ClusteringAnalysis(filteredOrders);
            var averageNettingAnalysis = NettingTrades(filteredOrders);

            if ((clusteringAnalysis == null || !clusteringAnalysis.ClusteringPositionBreach)
                &&
                (averageNettingAnalysis == null || !averageNettingAnalysis.AveragePositionRuleBreach))
            {
                return;
            }

            var security = filteredOrders?.FirstOrDefault()?.Instrument;

            var breach =
                new WashTradeRuleBreach(
                    _parameters.Windows.BackwardWindowSize,
                    OrganisationFactorValue,
                    RuleCtx.SystemProcessOperationContext(),
                    RuleCtx.CorrelationId(),
                    _parameters,
                    new TradePosition(filteredOrders),
                    security,
                    averageNettingAnalysis,
                    clusteringAnalysis,
                    UniverseDateTime);

            var universeAlert = new UniverseAlertEvent(Domain.Surveillance.Scheduling.Rules.FixedIncomeWashTrades, breach, RuleCtx);
            _alertStream.Add(universeAlert);

            _logger.LogInformation($"RunPostOrderEvent completed for {UniverseDateTime}");
        }

        /// <summary>
        /// See if trades net out to near zero i.e. large amount of churn
        /// </summary>
        public WashTradeAveragePositionBreach NettingTrades(IReadOnlyCollection<Order> tradingHistory)
        {
            if (!_parameters.PerformAveragePositionAnalysis)
            {
                return WashTradeAveragePositionBreach.None();
            }

            if (tradingHistory == null
                || !tradingHistory.Any())
            {
                return WashTradeAveragePositionBreach.None();
            }

            if (tradingHistory.Count < _parameters.AveragePositionMinimumNumberOfTrades)
            {
                return WashTradeAveragePositionBreach.None();
            }

            var portfolio = _portfolioFactory.Build();
            portfolio.Add(tradingHistory);

            foreach (var profitAndLoss in portfolio.ProfitAndLossTotal())
            {
                if (profitAndLoss.PercentageProfits() == null)
                {
                    continue;
                }

                if (Math.Abs(profitAndLoss.PercentageProfits().GetValueOrDefault(0)) > _parameters.AveragePositionMaximumPositionValueChange.GetValueOrDefault(0))
                {
                    return WashTradeAveragePositionBreach.None();
                }

                return new WashTradeAveragePositionBreach
                    (true,
                     tradingHistory.Count,
                     profitAndLoss.PercentageProfits(),
                    null);
            }

            return WashTradeAveragePositionBreach.None();
        }

        private WashTradeClusteringPositionBreach ClusteringAnalysis(IReadOnlyCollection<Order> tradingHistory)
        {
            if (!_parameters.PerformClusteringPositionAnalysis)
            {
                return WashTradeClusteringPositionBreach.None();
            }

            if (tradingHistory == null
                || !tradingHistory.Any())
            {
                return WashTradeClusteringPositionBreach.None();
            }

            var portfolio = _portfolioFactory.Build();
            portfolio.Add(tradingHistory);

            var clusters = _clusteringService.Cluster(portfolio.Ledger.FullLedger());

            if (clusters == null
                || !clusters.Any())
            {
                return WashTradeClusteringPositionBreach.None();
            }

            var washTradingClusters = clusters.Where(AnalyseCluster).Select(i => i.CentroidPrice).ToList();

            if (!washTradingClusters.Any())
            {
                return WashTradeClusteringPositionBreach.None();
            }

            return new WashTradeClusteringPositionBreach(true, washTradingClusters.Count, washTradingClusters);
        }

        private bool AnalyseCluster(PositionClusterCentroid centroid)
        {
            if (centroid == null)
            {
                return false;
            }

            var clusterPorfolio = _portfolioFactory.Build();
            clusterPorfolio.Add(centroid.Buys.Get().ToList());
            clusterPorfolio.Add(centroid.Sells.Get().ToList());

            if (clusterPorfolio.Ledger.FullLedger().Count < _parameters.ClusteringPositionMinimumNumberOfTrades)
            {
                return false;
            }

            foreach (var profitAndLoss in clusterPorfolio.ProfitAndLossTotal())
            {
                if (profitAndLoss.Costs.Value == 0
                    || profitAndLoss.Revenue.Value == 0)
                {
                    continue;
                }

                var largerValue = Math.Max(profitAndLoss.Costs.Value, profitAndLoss.Revenue.Value);
                var smallerValue = Math.Min(profitAndLoss.Costs.Value, profitAndLoss.Revenue.Value);

                var offset = largerValue * _parameters.ClusteringPercentageValueDifferenceThreshold.GetValueOrDefault(0);
                var lowerBoundary = largerValue - offset;
                var upperBoundary = largerValue + offset;

                if (smallerValue >= lowerBoundary
                    && smallerValue <= upperBoundary)
                {
                    return true;
                }
            }

            return false;
        }

        private List<Order> FilterByClientAccount(Order mostRecentFrame, List<Order> frames)
        {
            if (frames == null)
            {
                return new List<Order>();
            }

            var liveTrades =
                frames
                    .Where(at => at.OrderStatus() == OrderStatus.Filled)
                    .Where(at => at.OrderAverageFillPrice != null)
                    .ToList();

            if (!string.IsNullOrWhiteSpace(mostRecentFrame?.OrderClientAccountAttributionId))
            {
                liveTrades = liveTrades
                    .Where(lt =>
                        string.Equals(
                            lt.OrderClientAccountAttributionId,
                            mostRecentFrame.OrderClientAccountAttributionId,
                            StringComparison.InvariantCultureIgnoreCase))
                    .ToList();
            }

            return liveTrades;
        }

        protected override void RunInitialSubmissionEvent(ITradingHistoryStack history)
        {
            _logger.LogInformation($"RunInitialSubmissionRule called at {UniverseDateTime}");


            _logger.LogInformation($"RunInitialSubmissionRule completed for {UniverseDateTime}");
        }

        public override void RunOrderFilledEvent(ITradingHistoryStack history)
        {
            _logger.LogInformation($"RunOrderFilledEvent called at {UniverseDateTime}");



            _logger.LogInformation($"RunOrderFilledEvent completed for {UniverseDateTime}");
        }

        protected override void RunPostOrderEventDelayed(ITradingHistoryStack history)
        {
            // do nothing
        }

        protected override void RunInitialSubmissionEventDelayed(ITradingHistoryStack history)
        {
            // do nothing
        }

        public override void RunOrderFilledEventDelayed(ITradingHistoryStack history)
        {
            // do nothing
        }

        protected override void Genesis()
        {
            _logger.LogInformation($"Genesis called at {UniverseDateTime}");


            _logger.LogInformation($"Genesis completed for {UniverseDateTime}");
        }

        protected override void MarketOpen(MarketOpenClose exchange)
        {
            _logger.LogInformation($"MarketOpen called at {UniverseDateTime} for {exchange.MarketId}");


            _logger.LogInformation($"MarketOpen completed for {UniverseDateTime} for {exchange.MarketId}");
        }

        protected override void MarketClose(MarketOpenClose exchange)
        {
            _logger.LogInformation($"MarketClose called at {UniverseDateTime} for {exchange.MarketId}");


            _logger.LogInformation($"MarketClose completed for {UniverseDateTime} for {exchange.MarketId}");
        }

        protected override void EndOfUniverse()
        {
            _logger.LogInformation($"EndOfUniverse called at {UniverseDateTime}");

            RuleCtx?.EndEvent();

            _logger.LogInformation($"EndOfUniverse completed for {UniverseDateTime}");
        }

        public IUniverseCloneableRule Clone(IFactorValue factor)
        {
            var clone = (FixedIncomeWashTradeRule)Clone();
            clone.OrganisationFactorValue = factor;

            return clone;
        }

        public object Clone()
        {
            _logger.LogInformation($"Clone called at {UniverseDateTime}");

            var clone = (FixedIncomeWashTradeRule)this.MemberwiseClone();
            clone.BaseClone();

            _logger.LogInformation($"Clone completed for {UniverseDateTime}");

            return clone;
        }
    }
}
