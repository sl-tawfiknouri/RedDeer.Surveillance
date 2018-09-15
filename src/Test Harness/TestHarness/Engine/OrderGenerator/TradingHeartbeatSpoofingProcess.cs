using System;
using NLog;
using TestHarness.Engine.Heartbeat.Interfaces;
using TestHarness.Engine.OrderGenerator.Interfaces;
using MathNet.Numerics.Distributions;
using System.Linq;
using Domain.Equity;
using Domain.Equity.Frames;
using Domain.Trades.Orders;
using TestHarness.Engine.OrderGenerator.Strategies.Interfaces;

namespace TestHarness.Engine.OrderGenerator
{
    /// <summary>
    /// Fairly basic spoofing process
    /// for generating very vanilla spoofing type trades
    /// </summary>
    public class TradingHeartbeatSpoofingProcess : BaseTradingProcess, IOrderDataGenerator
    {
        private ExchangeFrame _lastFrame;
        private readonly IPulsatingHeartbeat _heartbeat;

        private volatile bool _initiated;
        private readonly object _lock = new object();

        public TradingHeartbeatSpoofingProcess(
            IPulsatingHeartbeat heartbeat,
            ILogger logger,
            ITradeStrategy<TradeOrderFrame> orderStrategy)
            : base(logger, orderStrategy)
        {
            _heartbeat = heartbeat ?? throw new ArgumentNullException(nameof(heartbeat));
        }

        public override void OnNext(ExchangeFrame value)
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
                if (_lastFrame != null 
                    && _lastFrame.Securities?.Count > 0)
                {
                    var selectSecurityToSpoof = DiscreteUniform.Sample(0, _lastFrame.Securities.Count() - 1);
                    var spoofSecurity = _lastFrame.Securities.Skip(selectSecurityToSpoof).FirstOrDefault();

                    // limited to six as recursion > 8 deep tends to get tough on the stack and raise the risk of a SO error
                    // if you want to increase this beyond 20 update spoofed order code for volume as well.
                    var spoofSize = DiscreteUniform.Sample(1, 6);
                    var spoofedOrders = SpoofedOrder(spoofSecurity, spoofSize, spoofSize).OrderBy(x => x.StatusChangedOn);
                    var counterTrade = CounterTrade(spoofSecurity);

                    foreach (var item in spoofedOrders)
                    {
                        _tradeStream.Add(item);
                    }

                    _tradeStream.Add(counterTrade);
                }
            }
        }

        private TradeOrderFrame[] SpoofedOrder(SecurityTick security, int remainingSpoofedOrders, int totalSpoofedOrders)
        {
            if (security == null
                || remainingSpoofedOrders <= 0)
            {
                return new TradeOrderFrame[0];
            }

            var priceOffset = (100 + (remainingSpoofedOrders)) / 100m;
            var limitPriceValue = security.Spread.Bid.Value * priceOffset;
            var limitPrice = new Price(limitPriceValue, security.Spread.Bid.Currency);

            var individualTradeVolumeLimit = (100 / totalSpoofedOrders);
            var volumeTarget = (100 + DiscreteUniform.Sample(0, individualTradeVolumeLimit)) / 100m;
            var volume = (int)(security.Volume.Traded * volumeTarget);

            var statusChangedOn = DateTime.UtcNow.AddMinutes(-10 + remainingSpoofedOrders);
            var tradePlacedOn = statusChangedOn;

            var spoofedTrade = new TradeOrderFrame
                (OrderType.Limit,
                _lastFrame.Exchange,
                security?.Security,
                limitPrice,
                volume,
                OrderPosition.BuyLong,
                OrderStatus.Cancelled,
                statusChangedOn,
                tradePlacedOn,
                "Spoofing-Trader",
                "",
                "Broker-1",
                "Broker-2");

            return 
                new[] { spoofedTrade }
                .Concat(SpoofedOrder(security, remainingSpoofedOrders - 1, totalSpoofedOrders))
                .ToArray();
        }

        private TradeOrderFrame CounterTrade(SecurityTick security)
        {
            var volumeToTrade = (int)Math.Round(security.Volume.Traded * 0.01m, MidpointRounding.AwayFromZero);
            var statusChangedOn = DateTime.UtcNow;
            var tradePlacedOn = statusChangedOn;

            return new TradeOrderFrame(
                OrderType.Market,
                _lastFrame.Exchange,
                security?.Security,
                null,
                volumeToTrade,
                OrderPosition.SellLong,
                OrderStatus.Fulfilled,
                statusChangedOn,
                tradePlacedOn,
                "Spoofing-Trader",
                "",
                "Broker-1",
                "Broker-2");
        }
    }
}
