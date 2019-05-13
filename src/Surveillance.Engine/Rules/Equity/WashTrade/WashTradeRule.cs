using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Analytics.Streams;
using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
using Surveillance.Engine.Rules.Currency.Interfaces;
using Surveillance.Engine.Rules.Factories.Equities;
using Surveillance.Engine.Rules.Factories.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
using Surveillance.Engine.Rules.Rules.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.WashTrade.Interfaces;
using Surveillance.Engine.Rules.Trades;
using Surveillance.Engine.Rules.Trades.Interfaces;
using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
using Surveillance.Engine.Rules.Universe.Interfaces;
using Surveillance.Engine.Rules.Universe.MarketEvents;
using Surveillance.Engine.Rules.Rules.Shared.WashTrade;
using Domain.Core.Financial.Money;
using Domain.Core.Trading.Orders;
using Surveillance.Engine.Rules.Rules.Shared.WashTrade.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Equity.WashTrade
{
    /// <summary>
    /// This trade rule is geared towards catching alpha wash trade breaches
    /// These are attempts to manipulate the market at large through publicly observable information
    /// By making a large amount of trades in an otherwise unremarkable stock the objective is to increase
    /// interest in trading the stock which will open up opportunities to profit by the wash traders
    ///
    /// Does not use market data so doesn't leverage the run mode setting
    /// </summary>
    public class WashTradeRule : BaseUniverseRule, IWashTradeRule
    {
        private readonly ILogger _logger;
        private readonly IWashTradeRuleEquitiesParameters _equitiesParameters;
        private readonly IClusteringService _clustering;
        private readonly IUniverseAlertStream _alertStream;
        private readonly ICurrencyConverterService _currencyConverterService;
        private readonly IUniverseOrderFilter _orderFilter;

        public WashTradeRule(
            IWashTradeRuleEquitiesParameters equitiesParameters,
            ISystemProcessOperationRunRuleContext ruleCtx,
            IClusteringService clustering,
            IUniverseAlertStream alertStream,
            ICurrencyConverterService currencyConverterService,
            IUniverseOrderFilter orderFilter,
            IUniverseMarketCacheFactory marketCacheFactory,
            RuleRunMode runMode,
            ILogger logger,
            ILogger<TradingHistoryStack> tradingHistoryLogger)
            : base(
                equitiesParameters?.WindowSize ?? TimeSpan.FromDays(1),
                Domain.Surveillance.Scheduling.Rules.WashTrade,
                EquityRuleWashTradeFactory.Version,
                "Wash Trade Rule",
                ruleCtx,
                marketCacheFactory,
                runMode,
                logger,
                tradingHistoryLogger)
        {
            _equitiesParameters = equitiesParameters ?? throw new ArgumentNullException(nameof(equitiesParameters));
            _clustering = clustering ?? throw new ArgumentNullException(nameof(clustering));
            _currencyConverterService = currencyConverterService ?? throw new ArgumentNullException(nameof(currencyConverterService));
            _orderFilter = orderFilter ?? throw new ArgumentNullException(nameof(orderFilter));
            _alertStream = alertStream ?? throw new ArgumentNullException(nameof(alertStream));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IFactorValue OrganisationFactorValue { get; set; } = FactorValue.None;

        protected override IUniverseEvent Filter(IUniverseEvent value)
        {
            return _orderFilter.Filter(value);
        }

        protected override void RunPostOrderEvent(ITradingHistoryStack history)
        {
            var activeTrades = history.ActiveTradeHistory();

            if (!activeTrades.Any())
            {
                return;
            }
            
            var liveTrades = FilterByClientAccount(history.ActiveTradeHistory().Pop(), history.ActiveTradeHistory());

            if (!liveTrades?.Any() ?? true)
            {
                return;
            }

            var tradePosition = new TradePosition(
                FilterByClientAccount(
                    history.ActiveTradeHistory().Pop(),
                    history.ActiveTradeHistory()));

            // Net change analysis
            var averagePositionCheckTask = NettingTrades(liveTrades);
            averagePositionCheckTask.Wait();
            var averagePositionCheck = averagePositionCheckTask.Result;

            // Clustering trade analysis
            var clusteringPositionCheck = ClusteringTrades(liveTrades);

            if ((averagePositionCheck == null || !averagePositionCheck.AveragePositionRuleBreach)
                && (clusteringPositionCheck == null || !clusteringPositionCheck.ClusteringPositionBreach))
            {
                return;
            }

            var security = liveTrades?.FirstOrDefault()?.Instrument;

            _logger.LogInformation($"incrementing alerts because of security {security?.Name} at {UniverseDateTime}");

            var breach =
                new WashTradeRuleBreach(
                    _equitiesParameters.WindowSize,
                    OrganisationFactorValue,
                    RuleCtx.SystemProcessOperationContext(),
                    RuleCtx.CorrelationId(),
                    _equitiesParameters,
                    tradePosition,
                    security,
                    averagePositionCheck,
                    clusteringPositionCheck);

            var universeAlert = new UniverseAlertEvent(Domain.Surveillance.Scheduling.Rules.WashTrade, breach, RuleCtx);
            _alertStream.Add(universeAlert);
        }

        private List<Order> FilterByClientAccount(Order mostRecentFrame, Stack<Order> frames)
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

        /// <summary>
        /// See if trades net out to near zero i.e. large amount of churn
        /// </summary>
        public async Task<WashTradeRuleBreach.WashTradeAveragePositionBreach> NettingTrades(List<Order> activeTrades)
        {
            if (!_equitiesParameters.PerformAveragePositionAnalysis)
            {
                return WashTradeRuleBreach.WashTradeAveragePositionBreach.None();
            }

            if (activeTrades == null 
                || !activeTrades.Any())
            {
                return WashTradeRuleBreach.WashTradeAveragePositionBreach.None();
            }
        
            if (activeTrades.Count < _equitiesParameters.AveragePositionMinimumNumberOfTrades)
            {
                return WashTradeRuleBreach.WashTradeAveragePositionBreach.None();
            }

            var buyPosition = new List<Order>(activeTrades.Where(at => at.OrderDirection == OrderDirections.BUY || at.OrderDirection == OrderDirections.COVER).ToList());
            var sellPosition = new List<Order>(activeTrades.Where(at => at.OrderDirection == OrderDirections.SELL || at.OrderDirection == OrderDirections.SHORT).ToList());

            var valueOfBuy = buyPosition.Sum(bp => bp.OrderFilledVolume.GetValueOrDefault(0) * (bp.OrderAverageFillPrice.GetValueOrDefault().Value));
            var valueOfSell = sellPosition.Sum(sp => sp.OrderFilledVolume.GetValueOrDefault(0) * (sp.OrderAverageFillPrice.GetValueOrDefault().Value));

            if (valueOfBuy == 0)
            {
                return WashTradeRuleBreach.WashTradeAveragePositionBreach.None();
            }

            if (valueOfSell == 0)
            {
                return WashTradeRuleBreach.WashTradeAveragePositionBreach.None();
            }

            var relativeValue = Math.Abs((valueOfBuy / valueOfSell) - 1);

            if (relativeValue > _equitiesParameters.AveragePositionMaximumPositionValueChange.GetValueOrDefault(0))
            {
                return WashTradeRuleBreach.WashTradeAveragePositionBreach.None();
            }

            if (_equitiesParameters.AveragePositionMaximumAbsoluteValueChangeAmount == null
                || string.IsNullOrWhiteSpace(_equitiesParameters.AveragePositionMaximumAbsoluteValueChangeCurrency))
            {
                _logger.LogInformation("found an average position breach and does not have an absolute limit set. Returning with average position breach");

                return new WashTradeRuleBreach.WashTradeAveragePositionBreach
                    (true,
                    activeTrades.Count,
                    relativeValue,
                    null);
            }

            var absDifference = Math.Abs(valueOfBuy - valueOfSell);
            var currency = activeTrades.FirstOrDefault()?.OrderCurrency;
            var absMoney = new Money(absDifference, currency?.Code ?? string.Empty);

            var targetCurrency = new Domain.Core.Financial.Money.Currency(_equitiesParameters.AveragePositionMaximumAbsoluteValueChangeCurrency);
            var convertedCurrency = await _currencyConverterService.Convert(new[] {absMoney}, targetCurrency, UniverseDateTime, RuleCtx);

            if (convertedCurrency == null)
            {
                _logger.LogError($"was not able to determine currency conversion - preferring to raise alert in lieu of necessary exchange rate information");

                return new WashTradeRuleBreach.WashTradeAveragePositionBreach
                    (true,
                     activeTrades.Count,
                     relativeValue,
                    null);
            }

            if (_equitiesParameters.AveragePositionMaximumAbsoluteValueChangeAmount < convertedCurrency.Value.Value)
            {
                _logger.LogInformation($"found an average position breach but the total change in position value exceeded the threshold of {_equitiesParameters.AveragePositionMaximumAbsoluteValueChangeAmount} ({_equitiesParameters.AveragePositionMaximumAbsoluteValueChangeCurrency}).");

                return WashTradeRuleBreach.WashTradeAveragePositionBreach.None();
            }

            return new WashTradeRuleBreach.WashTradeAveragePositionBreach
                (true,
                activeTrades.Count,
                relativeValue,
                convertedCurrency);
        }

        public WashTradeRuleBreach.WashTradeClusteringPositionBreach ClusteringTrades(List<Order> activeTrades)
        {
            if (!_equitiesParameters.PerformClusteringPositionAnalysis)
            {
                return WashTradeRuleBreach.WashTradeClusteringPositionBreach.None();
            }

            if (activeTrades == null
                || !activeTrades.Any())
            {
                return WashTradeRuleBreach.WashTradeClusteringPositionBreach.None();
            }

            var clusters = _clustering.Cluster(activeTrades);

            if (clusters == null
                || !clusters.Any())
            {
                return WashTradeRuleBreach.WashTradeClusteringPositionBreach.None();
            }

            var breachingClusters = new List<PositionClusterCentroid>();
            foreach (var cluster in clusters)
            {
                var counts = cluster.Buys.Get().Count + cluster.Sells.Get().Count;

                if (counts < _equitiesParameters.ClusteringPositionMinimumNumberOfTrades)
                {
                    continue;
                }

                var buyValue = cluster.Buys.Get().Sum(b => b.OrderAverageFillPrice.GetValueOrDefault().Value * b.OrderFilledVolume.GetValueOrDefault(0));
                var sellValue = cluster.Sells.Get().Sum(s => s.OrderAverageFillPrice.GetValueOrDefault().Value * s.OrderFilledVolume.GetValueOrDefault(0));
                
                var largerValue = Math.Max(buyValue, sellValue);
                var smallerValue = Math.Min(buyValue, sellValue);

                var offset = largerValue * _equitiesParameters.ClusteringPercentageValueDifferenceThreshold.GetValueOrDefault(0);
                var lowerBoundary = largerValue - offset;
                var upperBoundary = largerValue + offset;

                if (smallerValue >= lowerBoundary
                    && smallerValue <= upperBoundary)
                {
                    breachingClusters.Add(cluster);
                }
            }

            if (!breachingClusters.Any())
            {
                return WashTradeRuleBreach.WashTradeClusteringPositionBreach.None();
            }

            var centroids = breachingClusters.Select(bc => bc.CentroidPrice).ToList();

            return new WashTradeRuleBreach.WashTradeClusteringPositionBreach(true, breachingClusters.Count, centroids);
        }

        protected override void RunInitialSubmissionRule(ITradingHistoryStack history)
        {
        }

        protected override void Genesis()
        {
            _logger.LogInformation("Genesis occured");
        }

        protected override void MarketOpen(MarketOpenClose exchange)
        {
        }

        protected override void MarketClose(MarketOpenClose exchange)
        {
        }

        protected override void EndOfUniverse()
        {
            _logger.LogInformation($"Eschaton occured");
            RuleCtx?.EndEvent();
        }

        public IUniverseCloneableRule Clone(IFactorValue value)
        {
            var clone = (WashTradeRule)Clone();
            clone.OrganisationFactorValue = value;

            return clone;
        }

        public object Clone()
        {
            var clone = (WashTradeRule)this.MemberwiseClone();
            clone.BaseClone();

            return clone;
        }
    }
}
