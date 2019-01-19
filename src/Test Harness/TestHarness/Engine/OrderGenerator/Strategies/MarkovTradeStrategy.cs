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

        public void ExecuteTradeStrategy(EquityIntraDayTimeBarCollection frame, IOrderStream<Order> tradeOrders)
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
            EquityIntraDayTimeBarCollection frame,
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

        private IReadOnlyCollection<int> SecuritiesToTrade(EquityIntraDayTimeBarCollection frame, int securitiesToTrade)
        {
            var upperLimit = frame.Securities.Count - 1;
            var securitiesToTradeIds = new List<int>();
            for (var count = 0; count < securitiesToTrade; count++)
            {
                securitiesToTradeIds.Add(DiscreteUniform.Sample(0, upperLimit));
            }

            return securitiesToTradeIds;
        }

        private Order GenerateTrade(EquityInstrumentIntraDayTimeBar tick, EquityIntraDayTimeBarCollection exchFrame)
        {
            if (tick == null)
            {
                return null;
            }

            var direction = CalculateTradeDirection();
            var orderType = CalculateTradeOrderType();
            var limit = CalculateLimit(tick, direction, orderType);
            var executedPrice = tick.SpreadTimeBar.Price;
            var volume = CalculateVolume(tick);
            var orderStatus = CalculateOrderStatus();
            var orderStatusLastChanged = tick.TimeStamp.AddMilliseconds(300);
            var orderSubmittedOn = tick.TimeStamp;
            var traderId = GenerateClientFactorString();
            var dealerInstructions = "Process Asap";
            var orderCurrency = tick?.SpreadTimeBar.Price.Currency.Value ?? string.Empty;

            var cancelledDate = orderStatus == OrderStatus.Cancelled ? (DateTime?) orderSubmittedOn : null;
            var filledDate = orderStatus == OrderStatus.Filled ? (DateTime?)orderSubmittedOn : null;
            
            return new Order(
                tick.Security,
                tick.Market,
                null,
                "order-v1",
                DateTime.UtcNow,
                "order-v1",
                "order-group-v1",
                Guid.NewGuid().ToString(),
                orderSubmittedOn,
                orderSubmittedOn,
                null,
                cancelledDate,
                filledDate,
                orderStatusLastChanged,
                OrderTypes.MARKET,
                direction,
                new Currency(orderCurrency),
                new Currency(orderCurrency),
                OrderCleanDirty.CLEAN,
                null,
                limit,
                executedPrice,
                volume,
                volume,
                traderId,
                "Clearing-Bank",
                dealerInstructions,
                null,
                null,
                OptionEuropeanAmerican.NONE,
                new DealerOrder[0]);
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

        private OrderDirections CalculateTradeDirection()
        {
            var buyOrSellSample = DiscreteUniform.Sample(1, 2);
            var buyOrSell = (OrderDirections)buyOrSellSample;

            return buyOrSell;
        }

        private OrderTypes CalculateTradeOrderType()
        {
            var tradeOrderTypeSample = DiscreteUniform.Sample(0, 1);

            var tradeOrderType = (OrderTypes)tradeOrderTypeSample;

            return tradeOrderType;
        }

        private CurrencyAmount? CalculateLimit(EquityInstrumentIntraDayTimeBar tick, OrderDirections buyOrSell, OrderTypes tradeOrderType)
        {
            if (tradeOrderType != OrderTypes.LIMIT)
            {
                return null;
            }

            if (buyOrSell == OrderDirections.BUY)
            {
                var price = (decimal)Normal.Sample((double)tick.SpreadTimeBar.Bid.Value, _limitStandardDeviation);
                var adjustedPrice = Math.Max(0, Math.Round(price, 2));

                return new CurrencyAmount(adjustedPrice, tick.SpreadTimeBar.Bid.Currency);
            }
            else if (buyOrSell == OrderDirections.SELL)
            {
                var price = (decimal)Normal.Sample((double)tick.SpreadTimeBar.Ask.Value, _limitStandardDeviation);
                var adjustedPrice = Math.Max(0, Math.Round(price, 2));

                return new CurrencyAmount(adjustedPrice, tick.SpreadTimeBar.Ask.Currency);
            }

            return null;
        }

        private int CalculateVolume(EquityInstrumentIntraDayTimeBar tick)
        {
            var upperLimit = Math.Max(tick.SpreadTimeBar.Volume.Traded, 1);
            var tradingVolume = (int)Math.Sqrt(upperLimit);
            var volume = DiscreteUniform.Sample(0, tradingVolume);

            return volume;
        }
    }
}
