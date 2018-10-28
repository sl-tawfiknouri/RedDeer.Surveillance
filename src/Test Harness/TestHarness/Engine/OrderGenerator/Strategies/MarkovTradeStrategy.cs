using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Equity;
using Domain.Equity.Frames;
using Domain.Trades.Orders;
using Domain.Trades.Streams.Interfaces;
using MathNet.Numerics.Distributions;
using NLog;
using TestHarness.Engine.OrderGenerator.Strategies.Interfaces;

namespace TestHarness.Engine.OrderGenerator.Strategies
{
    public class MarkovTradeStrategy : ITradeStrategy<TradeOrderFrame>
    {
        private readonly ILogger _logger;
        private readonly ITradeVolumeStrategy _tradeVolumeStrategy;

        private readonly double _limitStandardDeviation = 4;

        public MarkovTradeStrategy(
            ILogger logger,
            ITradeVolumeStrategy volumeStrategy)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tradeVolumeStrategy = volumeStrategy ?? throw new ArgumentNullException(nameof(volumeStrategy));
        }

        // ReSharper disable once UnusedMember.Global
        public MarkovTradeStrategy(
            ILogger logger,
            double? limitStandardDeviation,
            ITradeVolumeStrategy volumeStrategy)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tradeVolumeStrategy = volumeStrategy ?? throw new ArgumentNullException(nameof(volumeStrategy));

            if (limitStandardDeviation != null && limitStandardDeviation >= 0)
                _limitStandardDeviation = limitStandardDeviation.Value;
        }

        public void ExecuteTradeStrategy(ExchangeFrame frame, ITradeOrderStream<TradeOrderFrame> tradeOrders)
        {
            if (tradeOrders == null)
            {
                _logger.Log(LogLevel.Error, "Received a null trade orders in the markov trade strategy");
                throw new ArgumentNullException(nameof(tradeOrders));
            }

            if (frame == null)
            {
                _logger.Log(LogLevel.Info, "A null frame was passed to the markov trade strategy");
                return;
            }

            if (frame.Securities == null
                || frame.Securities.All(sec => sec == null))
            {
                _logger.Log(LogLevel.Info, "No securities were present on the exchange frame in the markov trade strategy");
                return;
            }

            var tradableSecurities = frame.Securities.Where(sec => sec != null).ToList();
            var numberOfTradeOrders = _tradeVolumeStrategy.CalculateSecuritiesToTrade(tradableSecurities);

            if (numberOfTradeOrders <= 0)
            {
                _logger.Log(LogLevel.Info, "Markov trading strategy decided not to trade on this frame");
                return;
            }

            GenerateAndSubmitTrades(frame, tradeOrders, numberOfTradeOrders);
        }

        private void GenerateAndSubmitTrades(
            ExchangeFrame frame,
            ITradeOrderStream<TradeOrderFrame> tradeOrders,
            int numberOfTradeOrders)
        {
            var securitiesToTradeIds = SecuritiesToTrade(frame, numberOfTradeOrders);
            var securitiesToTrade = securitiesToTradeIds.Select(sec => frame.Securities.ElementAt(sec)).ToList();
            var trades = securitiesToTrade.Select(sec => GenerateTrade(sec, frame)).Where(trade => trade != null).ToList();

            foreach (var trade in trades)
            {
                tradeOrders.Add(trade);
            }

            _logger.Log(LogLevel.Info, $"Submitted {trades.Count} trade orders in frame");
        }

        private IReadOnlyCollection<int> SecuritiesToTrade(ExchangeFrame frame, int securitiesToTrade)
        {
            var upperLimit = frame.Securities.Count - 1;
            var securitiesToTradeIds = new List<int>();
            for (var count = 0; count < securitiesToTrade; count++)
            {
                securitiesToTradeIds.Add(DiscreteUniform.Sample(0, upperLimit));
            }

            return securitiesToTradeIds;
        }

        private TradeOrderFrame GenerateTrade(SecurityTick tick, ExchangeFrame exchFrame)
        {
            if (tick == null)
            {
                return null;
            }

            var position = CalculateTradeDirection();
            var orderType = CalculateTradeOrderType();
            var limit = CalculateLimit(tick, position, orderType);
            var executedPrice = tick.Spread.Price;
            var volume = CalculateVolume(tick);
            var orderStatus = CalculateOrderStatus();
            var orderStatusLastChanged = DateTime.UtcNow;
            var orderSubmittedOn = orderStatusLastChanged.AddMilliseconds(-300);
            var traderId = GenerateIdString();
            var traderClientId = GenerateProbabilisticIdString();
            var accountId = GenerateProbabilisticIdString();
            var dealerInstructions = string.Empty;
            var partyBrokerId = GenerateIdString();
            var counterPartyBrokerId = GenerateIdString();
            var tradeRationale = string.Empty;
            var tradeStrategy = string.Empty;
            var orderCurrency = tick?.Spread.Price.Currency ?? string.Empty;

            return new TradeOrderFrame(
                orderType,
                exchFrame.Exchange,
                tick.Security,
                limit,
                executedPrice,
                volume,
                volume,
                position,
                orderStatus,
                orderStatusLastChanged,
                orderSubmittedOn,
                traderId,
                traderClientId,
                accountId,
                dealerInstructions,
                partyBrokerId,
                counterPartyBrokerId,
                tradeRationale,
                tradeStrategy,
                orderCurrency);
        }

        private string GenerateIdString()
        {
            return DiscreteUniform.Sample(1, 999999).ToString();
        }

        private string GenerateProbabilisticIdString()
        {
            var generateId = DiscreteUniform.Sample(0, 1) == 0;

            if (generateId)
            {
                return string.Empty;
            }

            return GenerateIdString();
        }

        private OrderStatus CalculateOrderStatus()
        {
            var orderStatusSample = DiscreteUniform.Sample(0, 2);
            var orderStatus = (OrderStatus)orderStatusSample;

            return orderStatus;
        }

        private OrderPosition CalculateTradeDirection()
        {
            var buyOrSellSample = DiscreteUniform.Sample(0, 1);
            var buyOrSell = (OrderPosition)buyOrSellSample;

            return buyOrSell;
        }

        private OrderType CalculateTradeOrderType()
        {
            var tradeOrderTypeSample = DiscreteUniform.Sample(0, 1);

            var tradeOrderType = (OrderType)tradeOrderTypeSample;

            return tradeOrderType;
        }

        private Price? CalculateLimit(SecurityTick tick, OrderPosition buyOrSell, OrderType tradeOrderType)
        {
            if (tradeOrderType != OrderType.Limit)
            {
                return null;
            }

            if (buyOrSell == OrderPosition.Buy)
            {
                var price = (decimal)Normal.Sample((double)tick.Spread.Bid.Value, _limitStandardDeviation);
                var adjustedPrice = Math.Max(0, Math.Round(price, 2));

                return new Price(adjustedPrice, tick.Spread.Bid.Currency);
            }
            else if (buyOrSell == OrderPosition.Sell)
            {
                var price = (decimal)Normal.Sample((double)tick.Spread.Ask.Value, _limitStandardDeviation);
                var adjustedPrice = Math.Max(0, Math.Round(price, 2));

                return new Price(adjustedPrice, tick.Spread.Ask.Currency);
            }

            return null;
        }

        private int CalculateVolume(SecurityTick tick)
        {
            var upperLimit = Math.Max(tick.Volume.Traded, 1);
            var tradingVolume = (int)Math.Sqrt(upperLimit);
            var volume = DiscreteUniform.Sample(0, tradingVolume);

            return volume;
        }
    }
}
