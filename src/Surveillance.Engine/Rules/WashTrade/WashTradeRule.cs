using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Financial;
using Domain.Trading;
using Microsoft.Extensions.Logging;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Analytics.Streams;
using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
using Surveillance.Engine.Rules.Currency.Interfaces;
using Surveillance.Engine.Rules.Factories;
using Surveillance.Engine.Rules.Factories.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Interfaces;
using Surveillance.Engine.Rules.Rules.Interfaces;
using Surveillance.Engine.Rules.Rules.WashTrade.Interfaces;
using Surveillance.Engine.Rules.Trades;
using Surveillance.Engine.Rules.Trades.Interfaces;
using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
using Surveillance.Engine.Rules.Universe.Interfaces;
using Surveillance.Engine.Rules.Universe.MarketEvents;

namespace Surveillance.Engine.Rules.Rules.WashTrade
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
        private readonly IWashTradeRuleParameters _parameters;
        private readonly IWashTradePositionPairer _positionPairer;
        private readonly IWashTradeClustering _clustering;
        private readonly IUniverseAlertStream _alertStream;
        private readonly ICurrencyConverter _currencyConverter;
        private readonly IUniverseOrderFilter _orderFilter;

        public WashTradeRule(
            IWashTradeRuleParameters parameters,
            ISystemProcessOperationRunRuleContext ruleCtx,
            IWashTradePositionPairer positionPairer,
            IWashTradeClustering clustering,
            IUniverseAlertStream alertStream,
            ICurrencyConverter currencyConverter,
            IUniverseOrderFilter orderFilter,
            IUniverseMarketCacheFactory factory,
            RuleRunMode runMode,
            ILogger logger,
            ILogger<TradingHistoryStack> tradingHistoryLogger)
            : base(
                parameters?.WindowSize ?? TimeSpan.FromDays(1),
                Domain.Scheduling.Rules.WashTrade,
                WashTradeRuleFactory.Version,
                "Wash Trade Rule",
                ruleCtx,
                factory,
                runMode,
                logger,
                tradingHistoryLogger)
        {
            _parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            _positionPairer = positionPairer ?? throw new ArgumentNullException(nameof(positionPairer));
            _clustering = clustering ?? throw new ArgumentNullException(nameof(clustering));
            _currencyConverter = currencyConverter ?? throw new ArgumentNullException(nameof(currencyConverter));
            _orderFilter = orderFilter ?? throw new ArgumentNullException(nameof(orderFilter));
            _alertStream = alertStream ?? throw new ArgumentNullException(nameof(alertStream));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IFactorValue OrganisationFactorValue { get; set; } = FactorValue.None;

        protected override IUniverseEvent Filter(IUniverseEvent value)
        {
            return _orderFilter.Filter(value);
        }

        protected override void RunRule(ITradingHistoryStack history)
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

            // Pairing trade analysis
            var pairingPositionsCheckTask = PairingTrades(liveTrades);
            pairingPositionsCheckTask.Wait();
            var pairingPositionsCheck = pairingPositionsCheckTask.Result;

            // Clustering trade analysis
            var clusteringPositionCheck = ClusteringTrades(liveTrades);

            if ((averagePositionCheck == null || !averagePositionCheck.AveragePositionRuleBreach)
                && (pairingPositionsCheck == null || !pairingPositionsCheck.PairingPositionRuleBreach)
                && (clusteringPositionCheck == null || !clusteringPositionCheck.ClusteringPositionBreach))
            {
                return;
            }

            var security = liveTrades?.FirstOrDefault()?.Instrument;

            _logger.LogInformation($"incrementing alerts because of security {security?.Name} at {UniverseDateTime}");

            var breach =
                new WashTradeRuleBreach(
                    OrganisationFactorValue,
                    RuleCtx.SystemProcessOperationContext(),
                    RuleCtx.CorrelationId(),
                    _parameters,
                    tradePosition,
                    security,
                    averagePositionCheck,
                    pairingPositionsCheck,
                    clusteringPositionCheck);

            var universeAlert = new UniverseAlertEvent(Domain.Scheduling.Rules.WashTrade, breach, RuleCtx);
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
            if (!_parameters.PerformAveragePositionAnalysis)
            {
                return WashTradeRuleBreach.WashTradeAveragePositionBreach.None();
            }

            if (activeTrades == null 
                || !activeTrades.Any())
            {
                return WashTradeRuleBreach.WashTradeAveragePositionBreach.None();
            }
        
            if (activeTrades.Count < _parameters.AveragePositionMinimumNumberOfTrades)
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

            if (relativeValue > _parameters.AveragePositionMaximumPositionValueChange.GetValueOrDefault(0))
            {
                return WashTradeRuleBreach.WashTradeAveragePositionBreach.None();
            }

            if (_parameters.AveragePositionMaximumAbsoluteValueChangeAmount == null
                || string.IsNullOrWhiteSpace(_parameters.AveragePositionMaximumAbsoluteValueChangeCurrency))
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
            var absCurrencyAmount = new CurrencyAmount(absDifference, currency?.Value ?? string.Empty);

            var targetCurrency = new Domain.Financial.Currency(_parameters.AveragePositionMaximumAbsoluteValueChangeCurrency);
            var convertedCurrency = await _currencyConverter.Convert(new[] {absCurrencyAmount}, targetCurrency, UniverseDateTime, RuleCtx);

            if (convertedCurrency == null)
            {
                _logger.LogError($"was not able to determine currency conversion - preferring to raise alert in lieu of necessary exchange rate information");

                return new WashTradeRuleBreach.WashTradeAveragePositionBreach
                    (true,
                     activeTrades.Count,
                     relativeValue,
                    null);
            }

            if (_parameters.AveragePositionMaximumAbsoluteValueChangeAmount < convertedCurrency.Value.Value)
            {
                _logger.LogInformation($"found an average position breach but the total change in position value exceeded the threshold of {_parameters.AveragePositionMaximumAbsoluteValueChangeAmount} ({_parameters.AveragePositionMaximumAbsoluteValueChangeCurrency}).");

                return WashTradeRuleBreach.WashTradeAveragePositionBreach.None();
            }

            return new WashTradeRuleBreach.WashTradeAveragePositionBreach
                (true,
                activeTrades.Count,
                relativeValue,
                convertedCurrency);
        }

        public async Task<WashTradeRuleBreach.WashTradePairingPositionBreach> PairingTrades(List<Order> activeTrades)
        {
            if (!_parameters.PerformPairingPositionAnalysis)
            {
                return WashTradeRuleBreach.WashTradePairingPositionBreach.None();
            }

            if (activeTrades == null
                || !activeTrades.Any())
            {
                return WashTradeRuleBreach.WashTradePairingPositionBreach.None();
            }

            var tradingWithinThreshold = await CheckAbsoluteCurrencyAmountIsBelowMaximumThreshold(activeTrades);

            if (!tradingWithinThreshold)
            {
                return WashTradeRuleBreach.WashTradePairingPositionBreach.None();
            }
            
            var pairings = _positionPairer.PairUp(activeTrades, _parameters);

            if (_parameters.PairingPositionPercentageVolumeDifferenceThreshold != null)
            {
                pairings = FilteredPairsByVolume(pairings);
            }

            if (!pairings.Any())
            {
                return WashTradeRuleBreach.WashTradePairingPositionBreach.None();
            }

            var buyCount = pairings.SelectMany(a => a.Buys.Get()).Count();
            var sellCount = pairings.SelectMany(a => a.Sells.Get()).Count();
            var totalTradesWithinPairings = buyCount + sellCount;

            if (_parameters.PairingPositionMinimumNumberOfPairedTrades == null)
            {
                return new WashTradeRuleBreach.WashTradePairingPositionBreach(true, pairings.Count, totalTradesWithinPairings);
            }

            if ((totalTradesWithinPairings) >= _parameters.PairingPositionMinimumNumberOfPairedTrades.GetValueOrDefault(0))
            {
                return new WashTradeRuleBreach.WashTradePairingPositionBreach(true, pairings.Count, totalTradesWithinPairings);
            }

            return WashTradeRuleBreach.WashTradePairingPositionBreach.None();
        }

        private IReadOnlyCollection<PositionCluster> FilteredPairsByVolume(IReadOnlyCollection<PositionCluster> pairs)
        {
            if (pairs == null
                || !pairs.Any())
            {
                return new PositionCluster[0];
            }

            var results = new List<PositionCluster>();
            foreach (var pair in pairs)
            {
                var buyVolume = pair.Buys.Get().Sum(b => b.OrderFilledVolume.GetValueOrDefault());
                var sellVolume = pair.Sells.Get().Sum(s => s.OrderFilledVolume.GetValueOrDefault());

                if (buyVolume == sellVolume)
                {
                    results.Add(pair);
                    continue;
                }

                var larger = Math.Max(buyVolume, sellVolume);
                var smaller = Math.Min(buyVolume, sellVolume);
                var offset = (decimal)larger * (_parameters?.PairingPositionPercentageVolumeDifferenceThreshold.GetValueOrDefault(0) ?? 0m);

                if ((smaller >= larger - offset))
                {
                    results.Add(pair);
                    continue;
                }
            }

            return results;
        }

        private async Task<bool> CheckAbsoluteCurrencyAmountIsBelowMaximumThreshold(List<Order> activeTrades)
        {
            if (_parameters.PairingPositionMaximumAbsoluteCurrencyAmount == null
                || string.IsNullOrWhiteSpace(_parameters.PairingPositionMaximumAbsoluteCurrency))
            {
                return true;
            }

            var buyPosition = new List<Order>(activeTrades.Where(at => at.OrderDirection == OrderDirections.BUY || at.OrderDirection == OrderDirections.COVER).ToList());
            var sellPosition = new List<Order>(activeTrades.Where(at => at.OrderDirection == OrderDirections.SELL || at.OrderDirection == OrderDirections.SHORT).ToList());

            var valueOfBuy = buyPosition.Sum(bp => bp.OrderFilledVolume.GetValueOrDefault() * (bp.OrderAverageFillPrice.GetValueOrDefault().Value));
            var valueOfSell = sellPosition.Sum(sp => sp.OrderFilledVolume.GetValueOrDefault() * (sp.OrderAverageFillPrice.GetValueOrDefault().Value));

            if (valueOfBuy == 0)
            {
                return false;
            }

            if (valueOfSell == 0)
            {
                return false;
            }

            var absDifference = Math.Abs(valueOfBuy - valueOfSell);
            var currency = new Domain.Financial.Currency(activeTrades.FirstOrDefault()?.OrderCurrency.Value ?? string.Empty);
            var absCurrencyAmount = new CurrencyAmount(absDifference, currency);

            var targetCurrency = new Domain.Financial.Currency(_parameters.AveragePositionMaximumAbsoluteValueChangeCurrency);
            var convertedCurrency = await _currencyConverter.Convert(new[] { absCurrencyAmount }, targetCurrency, UniverseDateTime, RuleCtx);

            if (convertedCurrency == null)
            {
                _logger.LogError($"was not able to determine currency conversion. Will evaluate pairing trades on its own merits.");

                return true;
            }

            if (convertedCurrency?.Value > _parameters.PairingPositionMaximumAbsoluteCurrencyAmount.GetValueOrDefault(0))
            {
                return false;
            }

            return true;
        }

        public WashTradeRuleBreach.WashTradeClusteringPositionBreach ClusteringTrades(List<Order> activeTrades)
        {
            if (!_parameters.PerformClusteringPositionAnalysis)
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

                if (counts < _parameters.ClusteringPositionMinimumNumberOfTrades)
                {
                    continue;
                }

                var buyValue = cluster.Buys.Get().Sum(b => b.OrderAverageFillPrice.GetValueOrDefault().Value * b.OrderFilledVolume.GetValueOrDefault(0));
                var sellValue = cluster.Sells.Get().Sum(s => s.OrderAverageFillPrice.GetValueOrDefault().Value * s.OrderFilledVolume.GetValueOrDefault(0));
                
                var largerValue = Math.Max(buyValue, sellValue);
                var smallerValue = Math.Min(buyValue, sellValue);

                var offset = largerValue * _parameters.ClusteringPercentageValueDifferenceThreshold.GetValueOrDefault(0);
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
            var clone = (WashTradeRule)this.MemberwiseClone();
            clone.BaseClone();
            clone.OrganisationFactorValue = value;

            return clone;
        }
    }
}
