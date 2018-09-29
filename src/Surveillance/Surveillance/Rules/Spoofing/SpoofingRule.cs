using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Surveillance.Rules.Spoofing.Interfaces;
using Surveillance.Trades;
using Surveillance.Trades.Interfaces;
using System.Collections.Generic;
using Domain.Trades.Orders;
using Surveillance.Rule_Parameters.Interfaces;
using OrderStatus = Domain.Trades.Orders.OrderStatus;
using Surveillance.DataLayer.Stub;

namespace Surveillance.Rules.Spoofing
{
    public class SpoofingRule : BaseUniverseRule, ISpoofingRule
    {
        private readonly ISpoofingRuleParameters _parameters;
        private readonly ISpoofingRuleMessageSender _spoofingRuleMessageSender;
        private readonly ILogger _logger;

        public SpoofingRule(
            ISpoofingRuleParameters parameters,
            ISpoofingRuleMessageSender spoofingRuleMessageSender,
            ILogger logger)
            : base(
                  parameters?.WindowSize ?? TimeSpan.FromMinutes(30),
                  Domain.Scheduling.Rules.Spoofing,
                  Versioner.Version(1,0),
                  "Spoofing Rule",
                  logger)
        {
            _parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            _spoofingRuleMessageSender = spoofingRuleMessageSender ?? throw new ArgumentNullException(nameof(spoofingRuleMessageSender));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override void RunRule(ITradingHistoryStack history)
        {
            var tradeWindow = history?.ActiveTradeHistory();

            if (tradeWindow == null
                || !tradeWindow.Any())
            {
                return;
            }

            if (tradeWindow.All(trades => trades.Position == tradeWindow.First().Position))
            {
                return;
            }

            var mostRecentTrade = tradeWindow.Pop();

            if (mostRecentTrade.OrderStatus != OrderStatus.Fulfilled)
            {
                // we need to start from a fulfilled order
                return;
            }

            var buyPosition =
                new TradePositionCancellations(
                    new List<TradeOrderFrame>(),
                    _parameters.CancellationThreshold,
                    _parameters.CancellationThreshold,
                    _logger);

            var sellPosition =
                new TradePositionCancellations(
                    new List<TradeOrderFrame>(),
                    _parameters.CancellationThreshold,
                    _parameters.CancellationThreshold,
                    _logger);

            AddToPositions(buyPosition, sellPosition, mostRecentTrade);

            var tradingPosition =
                mostRecentTrade.Position == OrderPosition.Buy
                ? buyPosition
                : sellPosition;

            var opposingPosition =
                mostRecentTrade.Position == OrderPosition.Sell
                ? buyPosition
                : sellPosition;

            var hasBreachedSpoofingRule = CheckPositionForSpoofs(tradeWindow, buyPosition, sellPosition, tradingPosition, opposingPosition);

            if (hasBreachedSpoofingRule)
            {
                RecordRuleBreach(mostRecentTrade, tradingPosition, opposingPosition);
            }
        }

        private bool CheckPositionForSpoofs(
            Stack<TradeOrderFrame> tradeWindow,
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
                    (tradingPosition.VolumeInStatus(OrderStatus.Fulfilled)
                     * _parameters.RelativeSizeMultipleForSpoofExceedingReal);

                var opposedOrders = opposingPosition.VolumeInStatus(OrderStatus.Cancelled);
                hasBreachedSpoofingRule = hasBreachedSpoofingRule || adjustedFulfilledOrders <= opposedOrders;
            }

            return hasBreachedSpoofingRule;
        }

        private void AddToPositions(ITradePositionCancellations buyPosition, ITradePositionCancellations sellPosition, TradeOrderFrame nextTrade)
        {
            switch (nextTrade.Position)
            {
                case OrderPosition.Buy:
                    buyPosition.Add(nextTrade);
                    break;
                case OrderPosition.Sell:
                    sellPosition.Add(nextTrade);
                    break;
                default:
                    _logger.LogError("Spoofing rule not considering an out of range order direction");
                    throw new ArgumentOutOfRangeException(nameof(nextTrade));
            }
        }

        private void RecordRuleBreach(
            TradeOrderFrame mostRecentTrade,
            ITradePosition tradingPosition,
            ITradePosition opposingPosition)
        {
            _logger.LogInformation($"Spoofing rule breach detected for {mostRecentTrade.Security?.Identifiers}");

            var ruleBreach =
                new SpoofingRuleBreach(
                    _parameters.WindowSize,
                    tradingPosition,
                    opposingPosition,
                    mostRecentTrade.Security, 
                    mostRecentTrade);

            _spoofingRuleMessageSender.Send(ruleBreach);
        }

        protected override void Genesis()
        {
            _logger.LogDebug("Genesis occurred in the Spoofing Rule");
        }

        protected override void MarketOpen(MarketOpenClose exchange)
        {
            _logger.LogDebug($"Market Open ({exchange?.MarketId}) occurred in Spoofing Rule at {exchange?.MarketOpen}");
        }

        protected override void MarketClose(MarketOpenClose exchange)
        {
            _logger.LogDebug($"Market Close ({exchange?.MarketId}) occurred in Spoofing Rule at {exchange?.MarketClose}");
        }

        protected override void EndOfUniverse()
        {
            _logger.LogDebug("Eschaton occured in Spoofing Rule");
        }
    }
}