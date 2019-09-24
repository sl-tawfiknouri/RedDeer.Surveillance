namespace Surveillance.Engine.Rules.Rules.Equity.WashTrade
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Domain.Core.Financial.Money;
    using Domain.Core.Trading;
    using Domain.Core.Trading.Orders;
    using Domain.Surveillance.Scheduling;

    using Microsoft.Extensions.Logging;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Data.Universe.Interfaces;
    using Surveillance.Data.Universe.MarketEvents;
    using Surveillance.Engine.Rules.Analytics.Streams;
    using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
    using Surveillance.Engine.Rules.Currency.Interfaces;
    using Surveillance.Engine.Rules.Factories.Equities;
    using Surveillance.Engine.Rules.Factories.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.WashTrade.Interfaces;
    using Surveillance.Engine.Rules.Rules.Interfaces;
    using Surveillance.Engine.Rules.Rules.Shared.WashTrade;
    using Surveillance.Engine.Rules.Rules.Shared.WashTrade.Interfaces;
    using Surveillance.Engine.Rules.Trades;
    using Surveillance.Engine.Rules.Trades.Interfaces;
    using Surveillance.Engine.Rules.Universe.Filter.Interfaces;

    /// <summary>
    ///     This trade rule is geared towards catching alpha wash trade breaches
    ///     These are attempts to manipulate the market at large through publicly observable information
    ///     By making a large amount of trades in an otherwise unremarkable stock the objective is to increase
    ///     interest in trading the stock which will open up opportunities to profit by the wash traders
    ///     Does not use market data so doesn't leverage the run mode setting
    /// </summary>
    public class WashTradeRule : BaseUniverseRule, IWashTradeRule
    {
        private readonly IUniverseAlertStream _alertStream;

        private readonly IClusteringService _clustering;

        private readonly ICurrencyConverterService _currencyConverterService;

        private readonly IWashTradeRuleEquitiesParameters _equitiesParameters;

        private readonly ILogger _logger;

        private readonly IUniverseOrderFilter _orderFilter;

        public WashTradeRule(
            IWashTradeRuleEquitiesParameters equitiesParameters,
            ISystemProcessOperationRunRuleContext ruleContext,
            IClusteringService clustering,
            IUniverseAlertStream alertStream,
            ICurrencyConverterService currencyConverterService,
            IUniverseOrderFilter orderFilter,
            IUniverseMarketCacheFactory marketCacheFactory,
            RuleRunMode runMode,
            ILogger logger,
            ILogger<TradingHistoryStack> tradingHistoryLogger)
            : base(
                equitiesParameters?.Windows?.BackwardWindowSize ?? TimeSpan.FromDays(1),
                equitiesParameters?.Windows?.BackwardWindowSize ?? TimeSpan.FromDays(1),
                equitiesParameters?.Windows?.FutureWindowSize ?? TimeSpan.Zero,
                Rules.WashTrade,
                EquityRuleWashTradeFactory.Version,
                "Wash Trade Rule",
                ruleContext,
                marketCacheFactory,
                runMode,
                logger,
                tradingHistoryLogger)
        {
            this._equitiesParameters =
                equitiesParameters ?? throw new ArgumentNullException(nameof(equitiesParameters));
            this._clustering = clustering ?? throw new ArgumentNullException(nameof(clustering));
            this._currencyConverterService = currencyConverterService
                                             ?? throw new ArgumentNullException(nameof(currencyConverterService));
            this._orderFilter = orderFilter ?? throw new ArgumentNullException(nameof(orderFilter));
            this._alertStream = alertStream ?? throw new ArgumentNullException(nameof(alertStream));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IFactorValue OrganisationFactorValue { get; set; } = FactorValue.None;

        public IUniverseCloneableRule Clone(IFactorValue value)
        {
            var clone = (WashTradeRule)this.Clone();
            clone.OrganisationFactorValue = value;

            return clone;
        }

        public object Clone()
        {
            var clone = (WashTradeRule)this.MemberwiseClone();
            clone.BaseClone();

            return clone;
        }

        public WashTradeRuleBreach.WashTradeClusteringPositionBreach ClusteringTrades(List<Order> activeTrades)
        {
            if (!this._equitiesParameters.PerformClusteringPositionAnalysis)
                return WashTradeRuleBreach.WashTradeClusteringPositionBreach.None();

            if (activeTrades == null || !activeTrades.Any())
                return WashTradeRuleBreach.WashTradeClusteringPositionBreach.None();

            var clusters = this._clustering.Cluster(activeTrades);

            if (clusters == null || !clusters.Any())
                return WashTradeRuleBreach.WashTradeClusteringPositionBreach.None();

            var breachingClusters = new List<PositionClusterCentroid>();
            foreach (var cluster in clusters)
            {
                var counts = cluster.Buys.Get().Count + cluster.Sells.Get().Count;

                if (counts < this._equitiesParameters.ClusteringPositionMinimumNumberOfTrades) continue;

                var buyValue = cluster.Buys.Get().Sum(
                    b => b.OrderAverageFillPrice.GetValueOrDefault().Value * b.OrderFilledVolume.GetValueOrDefault(0));
                var sellValue = cluster.Sells.Get().Sum(
                    s => s.OrderAverageFillPrice.GetValueOrDefault().Value * s.OrderFilledVolume.GetValueOrDefault(0));

                var largerValue = Math.Max(buyValue, sellValue);
                var smallerValue = Math.Min(buyValue, sellValue);

                var offset = largerValue * this._equitiesParameters.ClusteringPercentageValueDifferenceThreshold
                                 .GetValueOrDefault(0);
                var lowerBoundary = largerValue - offset;
                var upperBoundary = largerValue + offset;

                if (smallerValue >= lowerBoundary && smallerValue <= upperBoundary) breachingClusters.Add(cluster);
            }

            if (!breachingClusters.Any()) return WashTradeRuleBreach.WashTradeClusteringPositionBreach.None();

            var centroids = breachingClusters.Select(bc => bc.CentroidPrice).ToList();

            return new WashTradeRuleBreach.WashTradeClusteringPositionBreach(true, breachingClusters.Count, centroids);
        }

        /// <summary>
        ///     See if trades net out to near zero i.e. large amount of churn
        /// </summary>
        public async Task<WashTradeRuleBreach.WashTradeAveragePositionBreach> NettingTrades(List<Order> activeTrades)
        {
            if (!this._equitiesParameters.PerformAveragePositionAnalysis)
                return WashTradeRuleBreach.WashTradeAveragePositionBreach.None();

            if (activeTrades == null || !activeTrades.Any())
                return WashTradeRuleBreach.WashTradeAveragePositionBreach.None();

            if (activeTrades.Count < this._equitiesParameters.AveragePositionMinimumNumberOfTrades)
                return WashTradeRuleBreach.WashTradeAveragePositionBreach.None();

            var buyPosition = new List<Order>(
                activeTrades.Where(
                        at => at.OrderDirection == OrderDirections.BUY || at.OrderDirection == OrderDirections.COVER)
                    .ToList());
            var sellPosition = new List<Order>(
                activeTrades.Where(
                        at => at.OrderDirection == OrderDirections.SELL || at.OrderDirection == OrderDirections.SHORT)
                    .ToList());

            var valueOfBuy = buyPosition.Sum(
                bp => bp.OrderFilledVolume.GetValueOrDefault(0) * bp.OrderAverageFillPrice.GetValueOrDefault().Value);
            var valueOfSell = sellPosition.Sum(
                sp => sp.OrderFilledVolume.GetValueOrDefault(0) * sp.OrderAverageFillPrice.GetValueOrDefault().Value);

            if (valueOfBuy == 0) return WashTradeRuleBreach.WashTradeAveragePositionBreach.None();

            if (valueOfSell == 0) return WashTradeRuleBreach.WashTradeAveragePositionBreach.None();

            var relativeValue = Math.Abs(valueOfBuy / valueOfSell - 1);

            if (relativeValue > this._equitiesParameters.AveragePositionMaximumPositionValueChange.GetValueOrDefault(0))
                return WashTradeRuleBreach.WashTradeAveragePositionBreach.None();

            if (this._equitiesParameters.AveragePositionMaximumAbsoluteValueChangeAmount == null
                || string.IsNullOrWhiteSpace(
                    this._equitiesParameters.AveragePositionMaximumAbsoluteValueChangeCurrency))
            {
                this._logger.LogInformation(
                    "found an average position breach and does not have an absolute limit set. Returning with average position breach");

                return new WashTradeRuleBreach.WashTradeAveragePositionBreach(
                    true,
                    activeTrades.Count,
                    relativeValue,
                    null);
            }

            var absDifference = Math.Abs(valueOfBuy - valueOfSell);
            var currency = activeTrades.FirstOrDefault()?.OrderCurrency;
            var absMoney = new Money(absDifference, currency?.Code ?? string.Empty);

            var targetCurrency =
                new Currency(this._equitiesParameters.AveragePositionMaximumAbsoluteValueChangeCurrency);
            var convertedCurrency = await this._currencyConverterService.Convert(
                                        new[] { absMoney },
                                        targetCurrency,
                                        this.UniverseDateTime,
                                        this.RuleCtx);

            if (convertedCurrency == null)
            {
                this._logger.LogError(
                    "was not able to determine currency conversion - preferring to raise alert in lieu of necessary exchange rate information");

                return new WashTradeRuleBreach.WashTradeAveragePositionBreach(
                    true,
                    activeTrades.Count,
                    relativeValue,
                    null);
            }

            if (this._equitiesParameters.AveragePositionMaximumAbsoluteValueChangeAmount
                < convertedCurrency.Value.Value)
            {
                this._logger.LogInformation(
                    $"found an average position breach but the total change in position value exceeded the threshold of {this._equitiesParameters.AveragePositionMaximumAbsoluteValueChangeAmount} ({this._equitiesParameters.AveragePositionMaximumAbsoluteValueChangeCurrency}).");

                return WashTradeRuleBreach.WashTradeAveragePositionBreach.None();
            }

            return new WashTradeRuleBreach.WashTradeAveragePositionBreach(
                true,
                activeTrades.Count,
                relativeValue,
                convertedCurrency);
        }

        public override void RunOrderFilledEvent(ITradingHistoryStack history)
        {
        }

        public override void RunOrderFilledEventDelayed(ITradingHistoryStack history)
        {
            // do nothing
        }

        protected override void EndOfUniverse()
        {
            this._logger.LogInformation("Eschaton occured");
            this.RuleCtx?.EndEvent();
        }

        protected override IUniverseEvent Filter(IUniverseEvent value)
        {
            return this._orderFilter.Filter(value);
        }

        protected override void Genesis()
        {
            this._logger.LogInformation("Genesis occured");
        }

        protected override void MarketClose(MarketOpenClose exchange)
        {
        }

        protected override void MarketOpen(MarketOpenClose exchange)
        {
        }

        protected override void RunInitialSubmissionEvent(ITradingHistoryStack history)
        {
        }

        protected override void RunInitialSubmissionEventDelayed(ITradingHistoryStack history)
        {
            // do nothing
        }

        protected override void RunPostOrderEvent(ITradingHistoryStack history)
        {
            var activeTrades = history.ActiveTradeHistory();

            if (!activeTrades.Any()) return;

            var liveTrades = this.FilterByClientAccount(
                history.ActiveTradeHistory().Pop(),
                history.ActiveTradeHistory());

            if (!liveTrades?.Any() ?? true) return;

            var tradePosition = new TradePosition(
                this.FilterByClientAccount(history.ActiveTradeHistory().Pop(), history.ActiveTradeHistory()));

            // Net change analysis
            var averagePositionCheckTask = this.NettingTrades(liveTrades);
            averagePositionCheckTask.Wait();
            var averagePositionCheck = averagePositionCheckTask.Result;

            // Clustering trade analysis
            var clusteringPositionCheck = this.ClusteringTrades(liveTrades);

            if ((averagePositionCheck == null || !averagePositionCheck.AveragePositionRuleBreach)
                && (clusteringPositionCheck == null || !clusteringPositionCheck.ClusteringPositionBreach)) return;

            var security = liveTrades?.FirstOrDefault()?.Instrument;

            this._logger.LogInformation(
                $"incrementing alerts because of security {security?.Name} at {this.UniverseDateTime}");

            // wrong but should be a judgement anyway
            var breach = new WashTradeRuleBreach(
                this._equitiesParameters.Windows.BackwardWindowSize,
                this.OrganisationFactorValue,
                this.RuleCtx.SystemProcessOperationContext(),
                this.RuleCtx.CorrelationId(),
                this._equitiesParameters,
                tradePosition,
                security,
                averagePositionCheck,
                clusteringPositionCheck,
                null,
                null,
                this.UniverseDateTime);

            var universeAlert = new UniverseAlertEvent(Rules.WashTrade, breach, this.RuleCtx);
            this._alertStream.Add(universeAlert);
        }

        protected override void RunPostOrderEventDelayed(ITradingHistoryStack history)
        {
            // do nothing
        }

        private List<Order> FilterByClientAccount(Order mostRecentFrame, Stack<Order> frames)
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