﻿using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Surveillance.Rules.Spoofing.Interfaces;
using Surveillance.Trades;
using Surveillance.Trades.Interfaces;
using System.Collections.Generic;
using Domain.Trades.Orders;
using Surveillance.Factories.Interfaces;
using Surveillance.DataLayer.ElasticSearch.Rules.Interfaces;
using OrderStatus = Domain.Trades.Orders.OrderStatus;

namespace Surveillance.Rules.Spoofing
{
    /// <summary>
    /// This is a very basic spoofing implementation and is to serve as a guide to the real spoofing rule implementation
    /// I think it will have problems with (placed) -> (cancelled) state transition
    /// And also the order that orders arrive in may cause issues...we're assuming our orders are arriving in last status changed order and that that order does not change
    /// </summary>
    public class SpoofingRule : BaseTradeRule, ISpoofingRule
    {
        private readonly IRuleBreachFactory _ruleBreachFactory;
        private readonly IRuleBreachRepository _ruleBreachRepository;
        private readonly ISpoofingRuleMessageSender _spoofingRuleMessageSender;
        private readonly ILogger _logger;

        // (0-1) % of cancellation req
        private const decimal CancellationThreshold = 0.8m;
        // volume difference between spoof and real trade
        private const decimal RelativeSizeMultipleForSpoofExceedingReal = 2.5m;

        public SpoofingRule(
            IRuleBreachFactory ruleBreachFactory,
            IRuleBreachRepository ruleBreachRepository,
            ISpoofingRuleMessageSender spoofingRuleMessageSender,
            ILogger<SpoofingRule> logger)
            : base(
                TimeSpan.FromMinutes(30),
                Domain.Scheduling.Rules.Spoofing,
                "V1.0",
                logger)
        {
            _ruleBreachFactory = ruleBreachFactory ?? throw new ArgumentNullException(nameof(ruleBreachFactory));
            _ruleBreachRepository = ruleBreachRepository ?? throw new ArgumentNullException(nameof(ruleBreachRepository));
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

            var buyPosition = new TradePosition(new List<TradeOrderFrame>(), CancellationThreshold, _logger);
            var sellPosition = new TradePosition(new List<TradeOrderFrame>(), CancellationThreshold, _logger);
            AddToPositions(buyPosition, sellPosition, mostRecentTrade);

            var tradingPosition =
                mostRecentTrade.Position == OrderPosition.BuyLong
                ? buyPosition
                : sellPosition;

            var opposingPosition =
                mostRecentTrade.Position == OrderPosition.BuyLong
                ? sellPosition
                : buyPosition;

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

                if (!opposingPosition.HighCancellationRatioByTradeSize() &&
                    !opposingPosition.HighCancellationRatioByTradeQuantity())
                {
                    continue;
                }

                var adjustedFulfilledOrders =
                    (tradingPosition.VolumeInStatus(OrderStatus.Fulfilled)
                     * RelativeSizeMultipleForSpoofExceedingReal);

                var opposedOrders = opposingPosition.VolumeInStatus(OrderStatus.Cancelled);
                hasBreachedSpoofingRule = hasBreachedSpoofingRule || adjustedFulfilledOrders <= opposedOrders;
            }

            if (hasBreachedSpoofingRule)
            {
                RecordRuleBreach(mostRecentTrade, tradingPosition, opposingPosition);
            }
        }

        private void AddToPositions(TradePosition buyPosition, TradePosition sellPosition, TradeOrderFrame nextTrade)
        {
            switch (nextTrade.Position)
            {
                case OrderPosition.BuyLong:
                    buyPosition.Add(nextTrade);
                    break;
                case OrderPosition.SellLong:
                    sellPosition.Add(nextTrade);
                    break;
                default:
                    _logger.LogError("Spoofing rule not considering an out of range order direction");
                    throw new ArgumentOutOfRangeException(nameof(nextTrade));
            }
        }

        private void RecordRuleBreach(
            TradeOrderFrame mostRecentTrade,
            TradePosition tradingPosition,
            TradePosition opposingPosition)
        {
            _logger.LogInformation($"Spoofing rule breach detected for {mostRecentTrade.Security?.Identifiers}");

            var volumeInPosition = tradingPosition.VolumeInStatus(OrderStatus.Fulfilled);
            var volumeSpoofed = opposingPosition.VolumeNotInStatus(OrderStatus.Fulfilled);

            var description = $"Traded ({mostRecentTrade.Position.ToString()}) {mostRecentTrade.Security?.Identifiers} with a fulfilled volume of {volumeInPosition} and a cancelled volume of {volumeSpoofed} in other trading direction preceding the most recent fulfilled trade.";

            var spoofingBreach = _ruleBreachFactory.Build(
                ElasticSearchDtos.Rules.RuleBreachCategories.Spoofing,
                mostRecentTrade.StatusChangedOn,
                mostRecentTrade.StatusChangedOn,
                description);

            _ruleBreachRepository.Save(spoofingBreach);
            _spoofingRuleMessageSender.Send(mostRecentTrade, tradingPosition, opposingPosition);
        }
    }
}