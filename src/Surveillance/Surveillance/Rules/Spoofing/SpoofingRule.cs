using System;
using System.Linq;
using Domain.Equity.Trading.Orders;
using Microsoft.Extensions.Logging;
using Surveillance.Rules.Spoofing.Interfaces;
using static Domain.Equity.Security;
using Surveillance.Trades;
using System.Collections.Concurrent;
using Surveillance.Trades.Interfaces;
using System.Collections.Generic;
using Surveillance.Factories.Interfaces;
using Surveillance.DataLayer.ElasticSearch;

namespace Surveillance.Rules.Spoofing
{
    /// <summary>
    /// This is a very basic spoofing implementation and is to serve as a guide to the real spoofing rule implementation
    /// I think it will have problems with (placed) -> (cancelled) state transition
    /// And also the order that orders arrive in may cause issues...we're assuming our orders are arriving in last status changed order and that that order does not change
    /// </summary>
    public class SpoofingRule : ISpoofingRule
    {
        private TimeSpan _spoofingWindowSize;
        private ConcurrentDictionary<SecurityId, ITradingHistoryStack> _tradingHistory;

        private IRuleBreachFactory _ruleBreachFactory;
        private IRuleBreachRepository _ruleBreachRepository;
        private ILogger _logger;

        private object _lock = new object();

        // (0-1) % of cancellation req
        private const decimal _cancellationThreshold = 0.8m;

        // volume difference between spoof and real trade
        private const decimal _relativeSizeMultipleForSpoofExceedingReal = 2.5m; 

        public SpoofingRule(
            IRuleBreachFactory ruleBreachFactory,
            IRuleBreachRepository ruleBreachRepository,
            ILogger<SpoofingRule> logger)
        {
            _ruleBreachFactory = ruleBreachFactory ?? throw new ArgumentNullException(nameof(ruleBreachFactory));
            _ruleBreachRepository = ruleBreachRepository ?? throw new ArgumentNullException(nameof(ruleBreachRepository));

            _spoofingWindowSize = TimeSpan.FromMinutes(30);
            _tradingHistory = new ConcurrentDictionary<SecurityId, ITradingHistoryStack>();
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void OnCompleted()
        {
            _logger.LogInformation("Spoofing rule stream completed.");
        }

        public void OnError(Exception error)
        {
            if (error == null)
            {
                return;
            }

            _logger.LogError($"An error occured in the spoofing rule stream {error.ToString()}");
        }

        public void OnNext(TradeOrderFrame value)
        {
            if (value == null
                || value.Security == null)
            {
                return;
            }

            lock (_lock)
            {
                if (!_tradingHistory.ContainsKey(value.Security.Id))
                {
                    var history = new TradingHistoryStack(_spoofingWindowSize);
                    history.Add(value, DateTime.UtcNow);
                    _tradingHistory.TryAdd(value.Security.Id, history);
                }
                else
                {
                    _tradingHistory.TryGetValue(value.Security.Id, out ITradingHistoryStack history);

                    var now = DateTime.UtcNow;
                    history.Add(value, now);
                    history.ArchiveExpiredActiveItems(now);
                }

                _tradingHistory.TryGetValue(value.Security.Id, out ITradingHistoryStack updatedHistory);
                CheckSpoofing(updatedHistory);
            }
        }

        private void CheckSpoofing(ITradingHistoryStack history)
        {
            if (history == null)
            {
                return;
            }

            var tradeWindow = history.ActiveTradeHistory();

            if (tradeWindow == null
                || !tradeWindow.Any())
            {
                return;
            }

            if (tradeWindow.All(trades => trades.Direction == tradeWindow.First().Direction))
            {
                return;
            }

            var mostRecentTrade = tradeWindow.Pop();

            if (mostRecentTrade.OrderStatus != OrderStatus.Fulfilled)
            {
                // we need to start from a fulfilled order
                return;
            }

            var buyPosition = new TradePosition(new List<TradeOrderFrame>(), _cancellationThreshold, _logger);
            var sellPosition = new TradePosition(new List<TradeOrderFrame>(), _cancellationThreshold, _logger);
            AddToPositions(buyPosition, sellPosition, mostRecentTrade);

            var hasBreachedSpoofingRule = false;
            while (!hasBreachedSpoofingRule)
            {
                if (!tradeWindow.Any())
                {
                    break;
                }

                var nextTrade = tradeWindow.Pop();

                AddToPositions(buyPosition, sellPosition, nextTrade);

                var tradingPosition =
                    mostRecentTrade.Direction == OrderDirection.Buy
                    ? buyPosition
                    : sellPosition;

                var opposingPosition =
                    mostRecentTrade.Direction == OrderDirection.Buy
                    ? sellPosition
                    : buyPosition;

                if (opposingPosition.HighCancellationRatioByTradeSize()
                    || opposingPosition.HighCancellationRatioByTradeQuantity())
                {
                    var adjustedFulfilledOrders =
                        (tradingPosition.VolumeInStatus(OrderStatus.Fulfilled)
                        * _relativeSizeMultipleForSpoofExceedingReal);

                    var opposedOrders = opposingPosition.VolumeInStatus(OrderStatus.Cancelled);

                    hasBreachedSpoofingRule = adjustedFulfilledOrders <= opposedOrders;
                }

                if (hasBreachedSpoofingRule)
                {
                    RecordRuleBreach(mostRecentTrade, tradingPosition, opposingPosition);
                }
            }
        }

        private void AddToPositions(TradePosition buyPosition, TradePosition sellPosition, TradeOrderFrame nextTrade)
        {
            switch (nextTrade.Direction)
            {
                case OrderDirection.Buy:
                    buyPosition.Add(nextTrade);
                    break;
                case OrderDirection.Sell:
                    sellPosition.Add(nextTrade);
                    break;
                default:
                    _logger.LogError("Spoofing rule not considering an out of range order direction");
                    throw new ArgumentOutOfRangeException("Order direction");
            }
        }

        private void RecordRuleBreach(
            TradeOrderFrame mostRecentTrade,
            TradePosition tradingPosition,
            TradePosition opposingPosition)
        {
            _logger.LogInformation($"Spoofing rule breach detected for {mostRecentTrade.Security?.Id.Id}");


            var volumeInPosition = tradingPosition.VolumeInStatus(OrderStatus.Fulfilled);
            var volumeSpoofed = opposingPosition.VolumeNotInStatus(OrderStatus.Fulfilled);

            var description = $"Traded ({mostRecentTrade.Direction.ToString()}) {mostRecentTrade.Security?.Id.Id} with a fulfilled volume of {volumeInPosition} and a cancelled volume of {volumeSpoofed} in other trading direction preceding the most recent fulfilled trade.";

            var spoofingBreach = _ruleBreachFactory.Build(
                ElasticSearchDtos.Rules.RuleBreachCategories.Spoofing,
                mostRecentTrade.StatusChangedOn,
                mostRecentTrade.StatusChangedOn,
                description);

            _ruleBreachRepository.Save(spoofingBreach);
        }
    }
}