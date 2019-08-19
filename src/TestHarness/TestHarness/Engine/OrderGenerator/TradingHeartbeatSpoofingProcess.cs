namespace TestHarness.Engine.OrderGenerator
{
    using System;
    using System.Linq;

    using Domain.Core.Financial.Assets;
    using Domain.Core.Financial.Money;
    using Domain.Core.Markets.Collections;
    using Domain.Core.Markets.Timebars;
    using Domain.Core.Trading.Orders;

    using MathNet.Numerics.Distributions;

    using Microsoft.Extensions.Logging;

    using TestHarness.Engine.Heartbeat.Interfaces;
    using TestHarness.Engine.OrderGenerator.Strategies.Interfaces;

    /// <summary>
    ///     Fairly basic spoofing process
    ///     for generating very vanilla spoofing type trades
    /// </summary>
    public class TradingHeartbeatSpoofingProcess : BaseTradingProcess
    {
        private readonly IPulsatingHeartbeat _heartbeat;

        private readonly object _lock = new object();

        private volatile bool _initiated;

        private EquityIntraDayTimeBarCollection _lastFrame;

        public TradingHeartbeatSpoofingProcess(
            IPulsatingHeartbeat heartbeat,
            ILogger logger,
            ITradeStrategy<Order> orderStrategy)
            : base(logger, orderStrategy)
        {
            this._heartbeat = heartbeat ?? throw new ArgumentNullException(nameof(heartbeat));
        }

        public override void OnNext(EquityIntraDayTimeBarCollection value)
        {
            lock (this._lock)
            {
                if (!this._initiated)
                {
                    this._heartbeat.OnBeat(this.TradeOnHeartbeat);
                    this._initiated = true;
                }

                if (value == null) return;

                this._lastFrame = value;
            }
        }

        protected override void _InitiateTrading()
        {
            this._heartbeat.Start();
        }

        protected override void _TerminateTradingStrategy()
        {
            this._heartbeat.Stop();
        }

        private Order CounterTrade(EquityInstrumentIntraDayTimeBar security)
        {
            var volumeToTrade = (int)Math.Round(
                security.SpreadTimeBar.Volume.Traded * 0.01m,
                MidpointRounding.AwayFromZero);
            var statusChangedOn = DateTime.UtcNow;
            var tradePlacedOn = statusChangedOn;

            var spoofedTrade = new Order(
                security.Security,
                this._lastFrame.Exchange,
                null,
                Guid.NewGuid().ToString(),
                DateTime.UtcNow,
                "order-v1",
                "order-v1",
                "order-group-1",
                tradePlacedOn,
                tradePlacedOn,
                null,
                null,
                null,
                statusChangedOn,
                OrderTypes.MARKET,
                OrderDirections.SELL,
                security.SpreadTimeBar.Price.Currency,
                security.SpreadTimeBar.Price.Currency,
                OrderCleanDirty.NONE,
                null,
                security.SpreadTimeBar.Price,
                security.SpreadTimeBar.Price,
                volumeToTrade,
                volumeToTrade,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                OptionEuropeanAmerican.NONE,
                new DealerOrder[0]);

            return spoofedTrade;
        }

        private Order[] SpoofedOrder(
            EquityInstrumentIntraDayTimeBar security,
            int remainingSpoofedOrders,
            int totalSpoofedOrders)
        {
            if (security == null || remainingSpoofedOrders <= 0) return new Order[0];

            var priceOffset = (100 + remainingSpoofedOrders) / 100m;
            var limitPriceValue = security.SpreadTimeBar.Bid.Value * priceOffset;
            var limitPrice = new Money(limitPriceValue, security.SpreadTimeBar.Bid.Currency);

            var individualTradeVolumeLimit = 100 / totalSpoofedOrders;
            var volumeTarget = (100 + DiscreteUniform.Sample(0, individualTradeVolumeLimit)) / 100m;
            var volume = (int)(security.SpreadTimeBar.Volume.Traded * volumeTarget);

            var statusChangedOn = DateTime.UtcNow.AddMinutes(-10 + remainingSpoofedOrders);
            var tradePlacedOn = statusChangedOn;

            var spoofedTrade = new Order(
                security.Security,
                security.Market,
                null,
                Guid.NewGuid().ToString(),
                DateTime.UtcNow,
                "order-v1",
                "order-v1",
                "order-group-1",
                tradePlacedOn,
                tradePlacedOn,
                null,
                null,
                statusChangedOn,
                null,
                OrderTypes.LIMIT,
                OrderDirections.BUY,
                security.SpreadTimeBar.Price.Currency,
                security.SpreadTimeBar.Price.Currency,
                OrderCleanDirty.NONE,
                null,
                limitPrice,
                limitPrice,
                volume,
                volume,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                OptionEuropeanAmerican.NONE,
                new DealerOrder[0]);

            return new[] { spoofedTrade }
                .Concat(this.SpoofedOrder(security, remainingSpoofedOrders - 1, totalSpoofedOrders)).ToArray();
        }

        private void TradeOnHeartbeat(object sender, EventArgs e)
        {
            lock (this._lock)
            {
                if (this._lastFrame == null || !this._lastFrame.Securities.Any()) return;

                var selectSecurityToSpoof = DiscreteUniform.Sample(0, this._lastFrame.Securities.Count - 1);
                var spoofSecurity = this._lastFrame.Securities.Skip(selectSecurityToSpoof).FirstOrDefault();

                // limited to six as recursion > 8 deep tends to get tough on the stack and raise the risk of a SO error
                // if you want to increase this beyond 20 update spoofed order code for volume as well.
                var spoofSize = DiscreteUniform.Sample(1, 6);
                var spoofedOrders = this.SpoofedOrder(spoofSecurity, spoofSize, spoofSize)
                    .OrderBy(x => x.MostRecentDateEvent());
                var counterTrade = this.CounterTrade(spoofSecurity);

                foreach (var item in spoofedOrders) this.TradeStream.Add(item);

                this.TradeStream.Add(counterTrade);
            }
        }
    }
}