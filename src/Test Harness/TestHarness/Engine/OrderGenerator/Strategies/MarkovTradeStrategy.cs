using System;
using System.Collections.Generic;
using System.Linq;
using DomainV2.Equity.TimeBars;
using DomainV2.Financial;
using DomainV2.Streams.Interfaces;
using DomainV2.Trading;
using MathNet.Numerics.Distributions;
using Microsoft.Extensions.Logging;
using TestHarness.Engine.OrderGenerator.Strategies.Interfaces;

namespace TestHarness.Engine.OrderGenerator.Strategies
{
    public class MarkovTradeStrategy : ITradeStrategy<Order>
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

        public void ExecuteTradeStrategy(MarketTimeBarCollection frame, IOrderStream<Order> tradeOrders)
        {
            if (tradeOrders == null)
            {
                _logger.Log(LogLevel.Error, "Received a null trade orders in the markov trade strategy");
                throw new ArgumentNullException(nameof(tradeOrders));
            }

            if (frame == null)
            {
                _logger.LogInformation("A null frame was passed to the markov trade strategy");
                return;
            }

            if (frame.Securities == null
                || frame.Securities.All(sec => sec == null))
            {
                _logger.LogInformation("No securities were present on the exchange frame in the markov trade strategy");
                return;
            }

            var tradableSecurities = frame.Securities.Where(sec => sec != null).ToList();
            var numberOfTradeOrders = _tradeVolumeStrategy.CalculateSecuritiesToTrade(tradableSecurities);

            if (numberOfTradeOrders <= 0)
            {
                _logger.LogInformation("Markov trading strategy decided not to trade on this frame");
                return;
            }

            GenerateAndSubmitTrades(frame, tradeOrders, numberOfTradeOrders);
        }

        private void GenerateAndSubmitTrades(
            MarketTimeBarCollection frame,
            IOrderStream<Order> tradeOrders,
            int numberOfTradeOrders)
        {
            var securitiesToTradeIds = SecuritiesToTrade(frame, numberOfTradeOrders);
            var securitiesToTrade = securitiesToTradeIds.Select(sec => frame.Securities.ElementAt(sec)).ToList();
            var trades = securitiesToTrade.Select(sec => GenerateTrade(sec, frame)).Where(trade => trade != null).ToList();

            foreach (var trade in trades)
            {
                tradeOrders.Add(trade);
            }

            _logger.LogInformation($"Submitted {trades.Count} trade orders in frame");
        }

        private IReadOnlyCollection<int> SecuritiesToTrade(MarketTimeBarCollection frame, int securitiesToTrade)
        {
            var upperLimit = frame.Securities.Count - 1;
            var securitiesToTradeIds = new List<int>();
            for (var count = 0; count < securitiesToTrade; count++)
            {
                securitiesToTradeIds.Add(DiscreteUniform.Sample(0, upperLimit));
            }

            return securitiesToTradeIds;
        }

        private Order GenerateTrade(FinancialInstrumentTimeBar tick, MarketTimeBarCollection exchFrame)
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
            var orderStatusLastChanged = tick.TimeStamp.AddMilliseconds(300);
            var orderSubmittedOn = tick.TimeStamp;
            var traderId = GenerateClientFactorString();
            var traderClientId = GenerateClientFactorString();
            var dealerInstructions = "Process Asap";
            var counterPartyBrokerId = GenerateClientFactorString();
            var tradeRationale = string.Empty;
            var tradeStrategy = string.Empty;
            var orderCurrency = tick?.Spread.Price.Currency.Value ?? string.Empty;

            var cancelledDate = orderStatus == OrderStatus.Cancelled ? (DateTime?) orderSubmittedOn : null;
            var filledDate = orderStatus == OrderStatus.Filled ? (DateTime?)orderSubmittedOn : null;
            
            return new Order(
                tick.Security,
                tick.Market,
                null,
                Guid.NewGuid().ToString(),
                orderSubmittedOn,
                orderSubmittedOn,
                null,
                cancelledDate,
                filledDate,
                orderStatusLastChanged,
                OrderTypes.MARKET,
                position,
                new Currency(orderCurrency),
                limit,
                executedPrice,
                volume,
                volume,
                "Mr. Portfolio Manager",
                traderId,
                counterPartyBrokerId,
                "Clearing-Bank",
                dealerInstructions,
                "long/short",
                tradeRationale,
                tradeStrategy,
                traderClientId,
                new Trade[0]);
        }

        private string GenerateClientFactorString()
        {
            return DiscreteUniform.Sample(1, 50).ToString();
        }

        private OrderStatus CalculateOrderStatus()
        {
            var orderStatusSample = DiscreteUniform.Sample(1, 5);
            var orderStatus = (OrderStatus)orderStatusSample;

            return orderStatus;
        }

        private OrderPositions CalculateTradeDirection()
        {
            var buyOrSellSample = DiscreteUniform.Sample(1, 2);
            var buyOrSell = (OrderPositions)buyOrSellSample;

            return buyOrSell;
        }

        private OrderTypes CalculateTradeOrderType()
        {
            var tradeOrderTypeSample = DiscreteUniform.Sample(0, 1);

            var tradeOrderType = (OrderTypes)tradeOrderTypeSample;

            return tradeOrderType;
        }

        private CurrencyAmount? CalculateLimit(FinancialInstrumentTimeBar tick, OrderPositions buyOrSell, OrderTypes tradeOrderType)
        {
            if (tradeOrderType != OrderTypes.LIMIT)
            {
                return null;
            }

            if (buyOrSell == OrderPositions.BUY)
            {
                var price = (decimal)Normal.Sample((double)tick.Spread.Bid.Value, _limitStandardDeviation);
                var adjustedPrice = Math.Max(0, Math.Round(price, 2));

                return new CurrencyAmount(adjustedPrice, tick.Spread.Bid.Currency);
            }
            else if (buyOrSell == OrderPositions.SELL)
            {
                var price = (decimal)Normal.Sample((double)tick.Spread.Ask.Value, _limitStandardDeviation);
                var adjustedPrice = Math.Max(0, Math.Round(price, 2));

                return new CurrencyAmount(adjustedPrice, tick.Spread.Ask.Currency);
            }

            return null;
        }

        private int CalculateVolume(FinancialInstrumentTimeBar tick)
        {
            var upperLimit = Math.Max(tick.Volume.Traded, 1);
            var tradingVolume = (int)Math.Sqrt(upperLimit);
            var volume = DiscreteUniform.Sample(0, tradingVolume);

            return volume;
        }
    }
}
