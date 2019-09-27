namespace Surveillance.Engine.Rules.Rules.Equity.WashTrade
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Domain.Core.Financial.Money;
    using Domain.Core.Trading;
    using Domain.Core.Trading.Orders;
    using Domain.Surveillance.Rules;
    using Domain.Surveillance.Rules.Interfaces;
    using Domain.Surveillance.Scheduling;

    using Microsoft.Extensions.Logging;

    using SharedKernel.Contracts.Markets;

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
        /// <summary>
        /// The alert stream.
        /// </summary>
        private readonly IUniverseAlertStream alertStream;

        /// <summary>
        /// The clustering.
        /// </summary>
        private readonly IClusteringService clustering;

        /// <summary>
        /// The currency converter service.
        /// </summary>
        private readonly ICurrencyConverterService currencyConverterService;

        /// <summary>
        /// The equities parameters.
        /// </summary>
        private readonly IWashTradeRuleEquitiesParameters equitiesParameters;

        /// <summary>
        /// The order filter.
        /// </summary>
        private readonly IUniverseOrderFilter orderFilter;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="WashTradeRule"/> class.
        /// </summary>
        /// <param name="equitiesParameters">
        /// The equities parameters.
        /// </param>
        /// <param name="ruleContext">
        /// The rule context.
        /// </param>
        /// <param name="clustering">
        /// The clustering.
        /// </param>
        /// <param name="alertStream">
        /// The alert stream.
        /// </param>
        /// <param name="currencyConverterService">
        /// The currency converter service.
        /// </param>
        /// <param name="orderFilter">
        /// The order filter.
        /// </param>
        /// <param name="marketCacheFactory">
        /// The market cache factory.
        /// </param>
        /// <param name="runMode">
        /// The run mode.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="tradingHistoryLogger">
        /// The trading history logger.
        /// </param>
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
            this.equitiesParameters =
                equitiesParameters ?? throw new ArgumentNullException(nameof(equitiesParameters));
            this.clustering = clustering ?? throw new ArgumentNullException(nameof(clustering));
            this.currencyConverterService = 
                currencyConverterService ?? throw new ArgumentNullException(nameof(currencyConverterService));
            this.orderFilter = orderFilter ?? throw new ArgumentNullException(nameof(orderFilter));
            this.alertStream = alertStream ?? throw new ArgumentNullException(nameof(alertStream));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Gets or sets the organization factor value.
        /// </summary>
        public IFactorValue OrganisationFactorValue { get; set; } = FactorValue.None;

        /// <summary>
        /// The clone.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="IUniverseCloneableRule"/>.
        /// </returns>
        public IUniverseCloneableRule Clone(IFactorValue value)
        {
            var clone = (WashTradeRule)this.Clone();
            clone.OrganisationFactorValue = value;

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
            var clone = (WashTradeRule)this.MemberwiseClone();
            clone.BaseClone();

            return clone;
        }

        /// <summary>
        /// The clustering trades.
        /// </summary>
        /// <param name="activeTrades">
        /// The active trades.
        /// </param>
        /// <returns>
        /// The <see cref="WashTradeClusteringPositionBreach"/>.
        /// </returns>
        public WashTradeRuleBreach.WashTradeClusteringPositionBreach ClusteringTrades(List<Order> activeTrades)
        {
            if (!this.equitiesParameters.PerformClusteringPositionAnalysis)
                return WashTradeRuleBreach.WashTradeClusteringPositionBreach.None();

            if (activeTrades == null || !activeTrades.Any())
                return WashTradeRuleBreach.WashTradeClusteringPositionBreach.None();

            var clusters = this.clustering.Cluster(activeTrades);

            if (clusters == null || !clusters.Any())
                return WashTradeRuleBreach.WashTradeClusteringPositionBreach.None();

            var breachingClusters = new List<PositionClusterCentroid>();
            foreach (var cluster in clusters)
            {
                var counts = cluster.Buys.Get().Count + cluster.Sells.Get().Count;

                if (counts < this.equitiesParameters.ClusteringPositionMinimumNumberOfTrades) continue;

                var buyValue = cluster.Buys.Get().Sum(
                    b => b.OrderAverageFillPrice.GetValueOrDefault().Value * b.OrderFilledVolume.GetValueOrDefault(0));
                var sellValue = cluster.Sells.Get().Sum(
                    s => s.OrderAverageFillPrice.GetValueOrDefault().Value * s.OrderFilledVolume.GetValueOrDefault(0));

                var largerValue = Math.Max(buyValue, sellValue);
                var smallerValue = Math.Min(buyValue, sellValue);

                var offset = largerValue * this.equitiesParameters.ClusteringPercentageValueDifferenceThreshold
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
        /// See if trades net out to near zero i.e. large amount of churn
        /// </summary>
        /// <param name="activeTrades">
        /// The active Trades.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<WashTradeRuleBreach.WashTradeAveragePositionBreach> NettingTrades(List<Order> activeTrades)
        {
            if (!this.equitiesParameters.PerformAveragePositionAnalysis)
                return WashTradeRuleBreach.WashTradeAveragePositionBreach.None();

            if (activeTrades == null || !activeTrades.Any())
                return WashTradeRuleBreach.WashTradeAveragePositionBreach.None();

            if (activeTrades.Count < this.equitiesParameters.AveragePositionMinimumNumberOfTrades)
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

            if (relativeValue > this.equitiesParameters.AveragePositionMaximumPositionValueChange.GetValueOrDefault(0))
                return WashTradeRuleBreach.WashTradeAveragePositionBreach.None();

            if (this.equitiesParameters.AveragePositionMaximumAbsoluteValueChangeAmount == null
                || string.IsNullOrWhiteSpace(
                    this.equitiesParameters.AveragePositionMaximumAbsoluteValueChangeCurrency))
            {
                this.logger.LogInformation(
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
                new Currency(this.equitiesParameters.AveragePositionMaximumAbsoluteValueChangeCurrency);
            var convertedCurrency = await this.currencyConverterService.Convert(
                                        new[] { absMoney },
                                        targetCurrency,
                                        this.UniverseDateTime,
                                        this.RuleCtx);

            if (convertedCurrency == null)
            {
                this.logger.LogError(
                    "was not able to determine currency conversion - preferring to raise alert in lieu of necessary exchange rate information");

                return new WashTradeRuleBreach.WashTradeAveragePositionBreach(
                    true,
                    activeTrades.Count,
                    relativeValue,
                    null);
            }

            if (this.equitiesParameters.AveragePositionMaximumAbsoluteValueChangeAmount
                < convertedCurrency.Value.Value)
            {
                this.logger.LogInformation(
                    $"found an average position breach but the total change in position value exceeded the threshold of {this.equitiesParameters.AveragePositionMaximumAbsoluteValueChangeAmount} ({this.equitiesParameters.AveragePositionMaximumAbsoluteValueChangeCurrency}).");

                return WashTradeRuleBreach.WashTradeAveragePositionBreach.None();
            }

            return new WashTradeRuleBreach.WashTradeAveragePositionBreach(
                true,
                activeTrades.Count,
                relativeValue,
                convertedCurrency);
        }

        /// <summary>
        /// The run order filled event.
        /// </summary>
        /// <param name="history">
        /// The history.
        /// </param>
        public override void RunOrderFilledEvent(ITradingHistoryStack history)
        {
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
            this.logger.LogInformation("Eschaton occured");
            this.RuleCtx?.EndEvent();
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
            return this.orderFilter.Filter(value);
        }

        /// <summary>
        /// The genesis.
        /// </summary>
        protected override void Genesis()
        {
            this.logger.LogInformation("Genesis occured");
        }

        /// <summary>
        /// The market close.
        /// </summary>
        /// <param name="exchange">
        /// The exchange.
        /// </param>
        protected override void MarketClose(MarketOpenClose exchange)
        {
        }

        /// <summary>
        /// The market open.
        /// </summary>
        /// <param name="exchange">
        /// The exchange.
        /// </param>
        protected override void MarketOpen(MarketOpenClose exchange)
        {
        }

        /// <summary>
        /// The run initial submission event.
        /// </summary>
        /// <param name="history">
        /// The history.
        /// </param>
        protected override void RunInitialSubmissionEvent(ITradingHistoryStack history)
        {
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

            this.logger.LogInformation(
                $"incrementing alerts because of security {security?.Name} at {this.UniverseDateTime}");

            // wrong but should be a judgement anyway
            var breach = new WashTradeRuleBreach(
                this.equitiesParameters.Windows.BackwardWindowSize,
                this.OrganisationFactorValue,
                this.RuleCtx.SystemProcessOperationContext(),
                this.RuleCtx.CorrelationId(),
                this.equitiesParameters,
                tradePosition,
                security,
                averagePositionCheck,
                clusteringPositionCheck,
                null,
                null,
                this.UniverseDateTime);

            var universeAlert = new UniverseAlertEvent(Rules.WashTrade, breach, this.RuleCtx);
            this.alertStream.Add(universeAlert);
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
        private List<Order> FilterByClientAccount(Order mostRecentFrame, Stack<Order> frames)
        {
            if (frames == null)
            {
                return new List<Order>();
            }

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