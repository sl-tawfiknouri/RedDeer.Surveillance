namespace TestHarness.Engine.OrderGenerator.Strategies
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Domain.Core.Financial.Assets;
    using Domain.Core.Financial.Money;
    using Domain.Core.Markets.Collections;
    using Domain.Core.Markets.Timebars;
    using Domain.Core.Trading.Orders;
    using Domain.Surveillance.Streams.Interfaces;

    using MathNet.Numerics.Distributions;

    using Microsoft.Extensions.Logging;

    using TestHarness.Engine.OrderGenerator.Strategies.Interfaces;

    public class MarkovTradeStrategy : ITradeStrategy<Order>
    {
        private readonly double _limitStandardDeviation = 4;

        private readonly ILogger _logger;

        private readonly ITradeVolumeStrategy _tradeVolumeStrategy;

        public MarkovTradeStrategy(ILogger logger, ITradeVolumeStrategy volumeStrategy)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._tradeVolumeStrategy = volumeStrategy ?? throw new ArgumentNullException(nameof(volumeStrategy));
        }

        // ReSharper disable once UnusedMember.Global
        public MarkovTradeStrategy(ILogger logger, double? limitStandardDeviation, ITradeVolumeStrategy volumeStrategy)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._tradeVolumeStrategy = volumeStrategy ?? throw new ArgumentNullException(nameof(volumeStrategy));

            if (limitStandardDeviation != null && limitStandardDeviation >= 0)
                this._limitStandardDeviation = limitStandardDeviation.Value;
        }

        public void ExecuteTradeStrategy(EquityIntraDayTimeBarCollection frame, IOrderStream<Order> tradeOrders)
        {
            if (tradeOrders == null)
            {
                this._logger.Log(LogLevel.Error, "Received a null trade orders in the markov trade strategy");
                throw new ArgumentNullException(nameof(tradeOrders));
            }

            if (frame == null)
            {
                this._logger.LogInformation("A null frame was passed to the markov trade strategy");
                return;
            }

            if (frame.Securities == null || frame.Securities.All(sec => sec == null))
            {
                this._logger.LogInformation(
                    "No securities were present on the exchange frame in the markov trade strategy");
                return;
            }

            var tradableSecurities = frame.Securities.Where(sec => sec != null).ToList();
            var numberOfTradeOrders = this._tradeVolumeStrategy.CalculateSecuritiesToTrade(tradableSecurities);

            if (numberOfTradeOrders <= 0)
            {
                this._logger.LogInformation("Markov trading strategy decided not to trade on this frame");
                return;
            }

            this.GenerateAndSubmitTrades(frame, tradeOrders, numberOfTradeOrders);
        }

        private Money CalculateExecutedPrice(EquityInstrumentIntraDayTimeBar tick)
        {
            if (tick.SpreadTimeBar.Price.Value >= 0.01m) return tick.SpreadTimeBar.Price;

            return new Money(0.01m, tick.SpreadTimeBar.Price.Currency);
        }

        private Money? CalculateLimit(
            EquityInstrumentIntraDayTimeBar tick,
            OrderDirections buyOrSell,
            OrderTypes tradeOrderType)
        {
            if (tradeOrderType != OrderTypes.LIMIT) return null;

            if (buyOrSell == OrderDirections.BUY)
            {
                var price = (decimal)Normal.Sample((double)tick.SpreadTimeBar.Bid.Value, this._limitStandardDeviation);
                var adjustedPrice = Math.Max(0, Math.Round(price, 2));

                return new Money(adjustedPrice, tick.SpreadTimeBar.Bid.Currency);
            }

            if (buyOrSell == OrderDirections.SELL)
            {
                var price = (decimal)Normal.Sample((double)tick.SpreadTimeBar.Ask.Value, this._limitStandardDeviation);
                var adjustedPrice = Math.Max(0, Math.Round(price, 2));

                return new Money(adjustedPrice, tick.SpreadTimeBar.Ask.Currency);
            }

            return null;
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

        private int CalculateVolume(EquityInstrumentIntraDayTimeBar tick)
        {
            var upperLimit = Math.Max(tick.SpreadTimeBar.Volume.Traded, 1);
            var tradingVolume = (int)Math.Sqrt(upperLimit);
            var volume = DiscreteUniform.Sample(1, tradingVolume);

            return volume;
        }

        private void GenerateAndSubmitTrades(
            EquityIntraDayTimeBarCollection frame,
            IOrderStream<Order> tradeOrders,
            int numberOfTradeOrders)
        {
            var securitiesToTradeIds = this.SecuritiesToTrade(frame, numberOfTradeOrders);
            var securitiesToTrade = securitiesToTradeIds.Select(sec => frame.Securities.ElementAt(sec)).ToList();
            var trades = securitiesToTrade.Select(sec => this.GenerateTrade(sec, frame)).Where(trade => trade != null)
                .ToList();

            foreach (var trade in trades) tradeOrders.Add(trade);

            this._logger.LogInformation($"Submitted {trades.Count} trade orders in frame");
        }

        private string GenerateClientFactorString()
        {
            return DiscreteUniform.Sample(1, 50).ToString();
        }

        private Order GenerateTrade(EquityInstrumentIntraDayTimeBar tick, EquityIntraDayTimeBarCollection exchFrame)
        {
            if (tick == null) return null;

            var direction = this.CalculateTradeDirection();
            var orderType = this.CalculateTradeOrderType();
            var limit = this.CalculateLimit(tick, direction, orderType);
            var executedPrice = this.CalculateExecutedPrice(tick);
            var volume = this.CalculateVolume(tick);
            var orderStatus = this.CalculateOrderStatus();
            var orderStatusLastChanged = tick.TimeStamp.AddMilliseconds(300);
            var orderSubmittedOn = tick.TimeStamp;
            var traderId = this.GenerateClientFactorString();
            var dealerInstructions = "Process Asap";
            var orderCurrency = tick?.SpreadTimeBar.Price.Currency.Code ?? string.Empty;

            var cancelledDate = orderStatus == OrderStatus.Cancelled ? (DateTime?)orderSubmittedOn : null;
            var filledDate = orderStatus == OrderStatus.Filled ? (DateTime?)orderSubmittedOn : null;

            return new Order(
                tick.Security,
                tick.Market,
                null,
                $"order-{Guid.NewGuid()}",
                DateTime.UtcNow,
                string.Empty,
                string.Empty,
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
                traderId,
                "Clearing-Bank",
                dealerInstructions,
                new OrderBroker(string.Empty, string.Empty, "Mr Broker", DateTime.Now, true),
                null,
                null,
                OptionEuropeanAmerican.NONE,
                new DealerOrder[0]);
        }

        private IReadOnlyCollection<int> SecuritiesToTrade(EquityIntraDayTimeBarCollection frame, int securitiesToTrade)
        {
            var upperLimit = frame.Securities.Count - 1;
            var securitiesToTradeIds = new List<int>();
            for (var count = 0; count < securitiesToTrade; count++)
                securitiesToTradeIds.Add(DiscreteUniform.Sample(0, upperLimit));

            return securitiesToTradeIds;
        }
    }
}