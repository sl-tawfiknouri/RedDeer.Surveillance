using System;
using System.Collections.Generic;
using System.Linq;

using Domain.Equity.Trading;
using Domain.Equity.Trading.Frames;
using Domain.Equity.Trading.Orders;
using Domain.Equity.Trading.Streams.Interfaces;
using MathNet.Numerics.Distributions;
using NLog;

namespace TestHarness.Engine.OrderGenerator.Strategies
{
    public class MarkovTradeStrategy : ITradeStrategy
    {
        private readonly ILogger _logger;
        private readonly double _limitStandardDeviation = 4;
        private readonly double _tradedSecurityStandardDeviation = 6;

        public MarkovTradeStrategy(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public MarkovTradeStrategy(
            ILogger logger,
            double? limitStandardDeviation,
            double? tradedSecurityStandardDeviation)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            if (limitStandardDeviation != null && limitStandardDeviation >= 0)
                _limitStandardDeviation = limitStandardDeviation.Value;

            if (tradedSecurityStandardDeviation != null && tradedSecurityStandardDeviation >= 0)
                _tradedSecurityStandardDeviation = tradedSecurityStandardDeviation.Value;
        }

        public void ExecuteTradeStrategy(ExchangeFrame frame, ITradeOrderStream tradeOrders)
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
                || !frame.Securities.Any(sec => sec != null))
            {
                _logger.Log(LogLevel.Info, "No securities were present on the exchange frame in the markov trade strategy");
                return;
            }

            var tradeableSecurities = frame.Securities.Where(sec => sec != null).ToList();
            int numberOfTradeOrders = CalculateSecuritiesToTrade(tradeableSecurities);

            if (numberOfTradeOrders <= 0)
            {
                _logger.Log(LogLevel.Info, "Markov trading strategy decided not to trade on this frame");
                return;
            }

            GenerateAndSubmitTrades(frame, tradeOrders, numberOfTradeOrders);

            return;
        }

        private void GenerateAndSubmitTrades(
            ExchangeFrame frame,
            ITradeOrderStream tradeOrders,
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

        private int CalculateSecuritiesToTrade(IReadOnlyCollection<SecurityFrame> frames)
        {
            var tradingMean = TradingMean(frames);
            var totalSecuritiesToTrade = (int)Normal.Sample(tradingMean, _tradedSecurityStandardDeviation);
            var adjustedSecuritiesToTrade = Math.Max(totalSecuritiesToTrade, 0);

            return adjustedSecuritiesToTrade;
        }

        private int TradingMean(IReadOnlyCollection<SecurityFrame> frames)
        {
            var rawCount = frames.Count();

            if (rawCount <= 0)
            {
                return 0;
            }

            var sqrt = Math.Sqrt(rawCount);

            if (sqrt < 10)
            {
                return rawCount;
            }

            return (int)sqrt;
        }

        private TradeOrderFrame GenerateTrade(SecurityFrame frame, ExchangeFrame exchFrame)
        {
            if (frame == null)
            {
                return null;
            }

            var direction = CalculateTradeDirection();
            var orderType = CalculateTradeOrderType();
            var limit = CalculateLimit(frame, direction, orderType);
            var volume = CalculateVolume(frame);
            var orderStatus = CalculateOrderStatus();

            return new TradeOrderFrame(
                orderType,
                exchFrame.Exchange,
                frame.Security,
                limit,
                volume,
                direction,
                orderStatus);
        }

        private OrderStatus CalculateOrderStatus()
        {
            var orderStatusSample = DiscreteUniform.Sample(0, 2);
            var orderStatus = (OrderStatus)orderStatusSample;

            return orderStatus;
        }

        private OrderDirection CalculateTradeDirection()
        {
            var buyOrSellSample = DiscreteUniform.Sample(0, 1);
            var buyOrSell = (OrderDirection)buyOrSellSample;

            return buyOrSell;
        }

        private OrderType CalculateTradeOrderType()
        {
            var tradeOrderTypeSample = DiscreteUniform.Sample(0, 1);

            var tradeOrderType = (OrderType)tradeOrderTypeSample;

            return tradeOrderType;
        }

        private Price? CalculateLimit(SecurityFrame frame, OrderDirection buyOrSell, OrderType tradeOrderType)
        {
            if (tradeOrderType != OrderType.Limit)
            {
                return null;
            }

            if (buyOrSell == OrderDirection.Buy)
            {
                var price = (decimal)Normal.Sample((double)frame.Spread.Buy.Value, _limitStandardDeviation);
                var adjustedPrice = Math.Max(0, Math.Round(price, 2));

                return new Price(adjustedPrice);
            }
            else if (buyOrSell == OrderDirection.Sell)
            {
                var price = (decimal)Normal.Sample((double)frame.Spread.Sell.Value, _limitStandardDeviation);
                var adjustedPrice = Math.Max(0, Math.Round(price, 2));

                return new Price(adjustedPrice);
            }

            return null;
        }

        private int CalculateVolume(SecurityFrame frame)
        {
            var upperLimit = Math.Max(frame.Volume.Traded, 1);
            var tradingVolume = (int)Math.Sqrt(upperLimit);
            var volume = DiscreteUniform.Sample(0, tradingVolume);

            return volume;
        }
    }
}
