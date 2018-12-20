using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Surveillance.Rules.Spoofing.Interfaces;
using Surveillance.Trades;
using Surveillance.Trades.Interfaces;
using System.Collections.Generic;
using DomainV2.Financial;
using DomainV2.Trading;
using Surveillance.Analytics.Streams;
using Surveillance.Factories;
using Surveillance.Analytics.Streams.Interfaces;
using Surveillance.Factories.Interfaces;
using Surveillance.RuleParameters.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.Universe.Filter.Interfaces;
using Surveillance.Universe.Interfaces;
using Surveillance.Universe.MarketEvents;

namespace Surveillance.Rules.Spoofing
{
    public class SpoofingRule : BaseUniverseRule, ISpoofingRule
    {
        private readonly ISpoofingRuleParameters _parameters;
        private readonly ISystemProcessOperationRunRuleContext _ruleCtx;
        private readonly IUniverseAlertStream _alertStream;
        private readonly IUniverseOrderFilter _orderFilter;
        private readonly ILogger _logger;

        public SpoofingRule(
            ISpoofingRuleParameters parameters,
            ISystemProcessOperationRunRuleContext ruleCtx,
            IUniverseAlertStream alertStream,
            IUniverseOrderFilter orderFilter,
            IUniverseMarketCacheFactory factory,
            ILogger logger)
            : base(
                  parameters?.WindowSize ?? TimeSpan.FromMinutes(30),
                  DomainV2.Scheduling.Rules.Spoofing,
                  SpoofingRuleFactory.Version,
                  "Spoofing Rule",
                  ruleCtx,
                  factory,
                  logger)
        {
            _parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _alertStream = alertStream ?? throw new ArgumentNullException(nameof(alertStream));
            _orderFilter = orderFilter ?? throw new ArgumentNullException(nameof(orderFilter));
            _ruleCtx = ruleCtx ?? throw new ArgumentNullException(nameof(ruleCtx));
        }

        protected override void RunInitialSubmissionRule(ITradingHistoryStack history)
        {
            var tradeWindow = history?.ActiveTradeHistory();

            if (tradeWindow == null
                || !tradeWindow.Any())
            {
                return;
            }

            if (tradeWindow.All(trades => trades.OrderPosition == tradeWindow.First().OrderPosition))
            {
                return;
            }

            var mostRecentTrade = tradeWindow.Pop();

            if (mostRecentTrade.OrderStatus() != OrderStatus.Filled)
            {
                // we need to start from a filled order
                return;
            }

            var buyPosition =
                new TradePositionCancellations(
                    new List<Order>(),
                    _parameters.CancellationThreshold,
                    _parameters.CancellationThreshold,
                    _logger);

            var sellPosition =
                new TradePositionCancellations(
                    new List<Order>(),
                    _parameters.CancellationThreshold,
                    _parameters.CancellationThreshold,
                    _logger);

            AddToPositions(buyPosition, sellPosition, mostRecentTrade);

            var tradingPosition =
                mostRecentTrade.OrderPosition == OrderPositions.BUY
                    ? buyPosition
                    : sellPosition;

            var opposingPosition =
                mostRecentTrade.OrderPosition == OrderPositions.SELL
                    ? buyPosition
                    : sellPosition;

            var hasBreachedSpoofingRule = CheckPositionForSpoofs(tradeWindow, buyPosition, sellPosition, tradingPosition, opposingPosition);

            if (hasBreachedSpoofingRule)
            {
                RecordRuleBreach(mostRecentTrade, tradingPosition, opposingPosition);
            }
        }
        
        private bool CheckPositionForSpoofs(
            Stack<Order> tradeWindow,
            ITradePositionCancellations buyPosition,
            ITradePositionCancellations sellPosition,
            ITradePositionCancellations tradingPosition,
            ITradePositionCancellations opposingPosition)
        {
            var hasBreachedSpoofingRule = false;
            var hasTradesInWindow = tradeWindow.Any();

            // ReSharper disable once LoopVariableIsNeverChangedInsideLoop
            while (hasTradesInWindow)
            {
                if (!tradeWindow.Any())
                {
                    // ReSharper disable once RedundantAssignment
                    hasTradesInWindow = false;
                    break;
                }

                var nextTrade = tradeWindow.Pop();

                AddToPositions(buyPosition, sellPosition, nextTrade);

                if (!opposingPosition.HighCancellationRatioByPositionSize() &&
                    !opposingPosition.HighCancellationRatioByTradeCount())
                {
                    continue;
                }

                var adjustedFulfilledOrders =
                    (tradingPosition.VolumeInStatus(OrderStatus.Filled)
                     * _parameters.RelativeSizeMultipleForSpoofExceedingReal);

                var opposedOrders = opposingPosition.VolumeInStatus(OrderStatus.Cancelled);
                hasBreachedSpoofingRule = hasBreachedSpoofingRule || adjustedFulfilledOrders <= opposedOrders;
            }

            return hasBreachedSpoofingRule;
        }

        private void AddToPositions(ITradePositionCancellations buyPosition, ITradePositionCancellations sellPosition, Order nextTrade)
        {
            switch (nextTrade.OrderPosition)
            {
                case OrderPositions.BUY:
                    buyPosition.Add(nextTrade);
                    break;
                case OrderPositions.SELL:
                    sellPosition.Add(nextTrade);
                    break;
                default:
                    _logger.LogError("Spoofing rule not considering an out of range order direction");
                    _ruleCtx.EventException("Spoofing rule not considering an out of range order direction");
                    throw new ArgumentOutOfRangeException(nameof(nextTrade));
            }
        }

        private void RecordRuleBreach(
            Order mostRecentTrade,
            ITradePosition tradingPosition,
            ITradePosition opposingPosition)
        {
            _logger.LogInformation($"Spoofing rule breach detected for {mostRecentTrade.Instrument?.Identifiers}");

            var ruleBreach =
                new SpoofingRuleBreach(
                    _parameters.WindowSize,
                    tradingPosition,
                    opposingPosition,
                    mostRecentTrade.Instrument, 
                    mostRecentTrade);

            var alert = new UniverseAlertEvent(DomainV2.Scheduling.Rules.Spoofing, ruleBreach, _ruleCtx);
            _alertStream.Add(alert);
        }

        protected override IUniverseEvent Filter(IUniverseEvent value)
        {
            return _orderFilter.Filter(value);
        }

        protected override void RunRule(ITradingHistoryStack history)
        {
            // spoofing rule does not monitor by last status changed
        }

        protected override void Genesis()
        {
            _logger.LogInformation("Genesis occurred in the Spoofing Rule");
        }

        protected override void MarketOpen(MarketOpenClose exchange)
        {
            _logger.LogInformation($"Market Open ({exchange?.MarketId}) occurred in Spoofing Rule at {exchange?.MarketOpen}");
        }

        protected override void MarketClose(MarketOpenClose exchange)
        {
            _logger.LogInformation($"Market Close ({exchange?.MarketId}) occurred in Spoofing Rule at {exchange?.MarketClose}");
        }

        protected override void EndOfUniverse()
        {
            _logger.LogInformation("Eschaton occured in Spoofing Rule");
            var alert = new UniverseAlertEvent(DomainV2.Scheduling.Rules.Spoofing, null, _ruleCtx, true);
            _alertStream.Add(alert);
            _ruleCtx?.EndEvent();
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}