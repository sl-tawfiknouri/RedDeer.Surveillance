﻿using System;
using TestHarness.Engine.Heartbeat.Interfaces;
using MathNet.Numerics.Distributions;
using System.Linq;
using Microsoft.Extensions.Logging;
using TestHarness.Engine.OrderGenerator.Strategies.Interfaces;
using Domain.Core.Financial.Assets;
using Domain.Core.Financial.Money;
using Domain.Core.Markets.Collections;
using Domain.Core.Markets.Timebars;
using Domain.Core.Trading.Orders;

namespace TestHarness.Engine.OrderGenerator
{
    /// <summary>
    /// Fairly basic spoofing process
    /// for generating very vanilla spoofing type trades
    /// </summary>
    public class TradingHeartbeatSpoofingProcess : BaseTradingProcess
    {
        private EquityIntraDayTimeBarCollection _lastFrame;
        private readonly IPulsatingHeartbeat _heartbeat;

        private volatile bool _initiated;
        private readonly object _lock = new object();

        public TradingHeartbeatSpoofingProcess(
            IPulsatingHeartbeat heartbeat,
            ILogger logger,
            ITradeStrategy<Order> orderStrategy)
            : base(logger, orderStrategy)
        {
            _heartbeat = heartbeat ?? throw new ArgumentNullException(nameof(heartbeat));
        }

        public override void OnNext(EquityIntraDayTimeBarCollection value)
        {
            lock (_lock)
            {
                if (!_initiated)
                {
                    _heartbeat.OnBeat(TradeOnHeartbeat);
                    _initiated = true;
                }

                if (value == null)
                {
                    return;
                }

                _lastFrame = value;
            }
        }

        protected override void _InitiateTrading()
        {
            _heartbeat.Start();
        }

        protected override void _TerminateTradingStrategy()
        {
            _heartbeat.Stop();
        }

        private void TradeOnHeartbeat(object sender, EventArgs e)
        {
            lock (_lock)
            {
                if (_lastFrame == null
                    || !_lastFrame.Securities.Any())
                {
                    return;
                }

                var selectSecurityToSpoof = DiscreteUniform.Sample(0, _lastFrame.Securities.Count - 1);
                var spoofSecurity = _lastFrame.Securities.Skip(selectSecurityToSpoof).FirstOrDefault();

                // limited to six as recursion > 8 deep tends to get tough on the stack and raise the risk of a SO error
                // if you want to increase this beyond 20 update spoofed order code for volume as well.
                var spoofSize = DiscreteUniform.Sample(1, 6);
                var spoofedOrders = SpoofedOrder(spoofSecurity, spoofSize, spoofSize).OrderBy(x => x.MostRecentDateEvent());
                var counterTrade = CounterTrade(spoofSecurity);

                foreach (var item in spoofedOrders)
                {
                    TradeStream.Add(item);
                }

                TradeStream.Add(counterTrade);
            }
        }

        private Order[] SpoofedOrder(EquityInstrumentIntraDayTimeBar security, int remainingSpoofedOrders, int totalSpoofedOrders)
        {
            if (security == null
                || remainingSpoofedOrders <= 0)
            {
                return new Order[0];
            }

            var priceOffset = (100 + (remainingSpoofedOrders)) / 100m;
            var limitPriceValue = security.SpreadTimeBar.Bid.Value * priceOffset;
            var limitPrice = new Money(limitPriceValue, security.SpreadTimeBar.Bid.Currency);

            var individualTradeVolumeLimit = (100 / totalSpoofedOrders);
            var volumeTarget = (100 + DiscreteUniform.Sample(0, individualTradeVolumeLimit)) / 100m;
            var volume = (int)(security.SpreadTimeBar.Volume.Traded * volumeTarget);

            var statusChangedOn = DateTime.UtcNow.AddMinutes(-10 + remainingSpoofedOrders);
            var tradePlacedOn = statusChangedOn;

            var spoofedTrade =
                new Order(
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
                    OptionEuropeanAmerican.NONE,
                    new DealerOrder[0]);

            return 
                new[] { spoofedTrade }
                .Concat(SpoofedOrder(security, remainingSpoofedOrders - 1, totalSpoofedOrders))
                .ToArray();
        }

        private Order CounterTrade(EquityInstrumentIntraDayTimeBar security)
        {
            var volumeToTrade = (int)Math.Round(security.SpreadTimeBar.Volume.Traded * 0.01m, MidpointRounding.AwayFromZero);
            var statusChangedOn = DateTime.UtcNow;
            var tradePlacedOn = statusChangedOn;

            var spoofedTrade =
                new Order(
                    security.Security,
                    _lastFrame.Exchange,
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
                    OptionEuropeanAmerican.NONE,
                    new DealerOrder[0]);

            return spoofedTrade;
        }
    }
}
