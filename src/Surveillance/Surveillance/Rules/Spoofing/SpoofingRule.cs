using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Surveillance.Rules.Spoofing.Interfaces;
using Surveillance.Trades;
using Surveillance.Trades.Interfaces;
using System.Collections.Generic;
using Domain.Trades.Orders;
using Surveillance.Factories.Interfaces;
using Surveillance.DataLayer.ElasticSearch.Rules.Interfaces;
using Surveillance.Rule_Parameters.Interfaces;
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
        private readonly ISpoofingRuleParameters _parameters;
        private readonly IRuleBreachFactory _ruleBreachFactory;
        private readonly IRuleBreachRepository _ruleBreachRepository;
        private readonly ISpoofingRuleMessageSender _spoofingRuleMessageSender;
        private readonly ILogger _logger;

        public SpoofingRule(
            ISpoofingRuleParameters parameters,
            IRuleBreachFactory ruleBreachFactory,
            IRuleBreachRepository ruleBreachRepository,
            ISpoofingRuleMessageSender spoofingRuleMessageSender,
            ILogger<SpoofingRule> logger)
            : base(
                parameters?.WindowSize ?? TimeSpan.FromMinutes(30),
                Domain.Scheduling.Rules.Spoofing,
                Versioner.Version(1, 0),
                logger)
        {
            _parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
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
                mostRecentTrade.Position == OrderPosition.BuyLong
                ? buyPosition
                : sellPosition;

            var opposingPosition =
                mostRecentTrade.Position == OrderPosition.SellLong
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
                case OrderPosition.BuyLong:
                    buyPosition.Add(nextTrade);
                    break;
                case OrderPosition.SellLong:
                    sellPosition.Add(nextTrade);
                    break;
                case OrderPosition.BuyShort:
                    break;
                case OrderPosition.SellShort:
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

            var volumeInPosition = tradingPosition.VolumeInStatus(OrderStatus.Fulfilled);
            var volumeSpoofed = opposingPosition.VolumeNotInStatus(OrderStatus.Fulfilled);

            var description = $"Traded ({mostRecentTrade.Position.ToString()}) {mostRecentTrade.Security?.Identifiers} with a fulfilled volume of {volumeInPosition} and a cancelled volume of {volumeSpoofed} in other trading direction preceding the most recent fulfilled trade.";

            var spoofingBreach = _ruleBreachFactory.Build(
                ElasticSearchDtos.Rules.RuleBreachCategories.Spoofing,
                mostRecentTrade.StatusChangedOn,
                mostRecentTrade.StatusChangedOn,
                description);

            var ruleBreach =
                new SpoofingRuleBreach(
                    _parameters.WindowSize,
                    tradingPosition,
                    opposingPosition,
                    mostRecentTrade.Security, 
                    mostRecentTrade);

            _ruleBreachRepository.Save(spoofingBreach);
            _spoofingRuleMessageSender.Send(ruleBreach);
        }
    }
}