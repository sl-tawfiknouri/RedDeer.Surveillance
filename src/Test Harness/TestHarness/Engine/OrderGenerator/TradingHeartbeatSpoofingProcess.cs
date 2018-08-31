using System;
using Domain.Equity.Trading.Frames;
using Domain.Equity.Trading.Orders;
using NLog;
using TestHarness.Engine.Heartbeat.Interfaces;
using TestHarness.Engine.OrderGenerator.Interfaces;
using TestHarness.Engine.OrderGenerator.Strategies;
using MathNet.Numerics.Distributions;
using System.Linq;
using Domain.Equity.Trading;

namespace TestHarness.Engine.OrderGenerator
{
    public class TradingHeartbeatSpoofingProcess : BaseTradingProcess, IOrderDataGenerator
    {
        private ExchangeFrame _lastFrame;
        private IPulsatingHeartbeat _heartbeat;

        private volatile bool _initiated;
        private object _lock = new object();

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

        private TradeOrderFrame[] SpoofedOrder(SecurityFrame security, int remainingSpoofedOrders, int totalSpoofedOrders)
        {
            if (security == null
                || remainingSpoofedOrders <= 0)
            {
                return new TradeOrderFrame[0];
            }

            var priceOffset = (100 + (remainingSpoofedOrders)) / 100m;
            var limitPriceValue = security.Spread.Buy.Value * priceOffset;
            var limitPrice = new Price(limitPriceValue);

            var individualTradeVolumeLimit = (100 / totalSpoofedOrders);
            var volumeTarget = (100 + DiscreteUniform.Sample(0, individualTradeVolumeLimit)) / 100m;
            var volume = (int)(security.Volume.Traded * volumeTarget);

            var spoofedTrade = new TradeOrderFrame
                (OrderType.Limit,
                _lastFrame.Exchange,
                security?.Security,
                limitPrice,
                volume,
                OrderDirection.Buy,
                OrderStatus.Cancelled,
                DateTime.Now.AddMinutes(-10 + remainingSpoofedOrders));

            return 
                new[] { spoofedTrade }
                .Concat(SpoofedOrder(security, remainingSpoofedOrders - 1, totalSpoofedOrders))
                .ToArray();
        }

        private TradeOrderFrame CounterTrade(SecurityFrame security)
        {
            var volumeToTrade = (int)Math.Round(security.Volume.Traded * 0.01m, MidpointRounding.AwayFromZero);

            return new TradeOrderFrame(
                OrderType.Market,
                _lastFrame.Exchange,
                security?.Security,
                null,
                volumeToTrade,
                OrderDirection.Sell,
                OrderStatus.Fulfilled,
                DateTime.Now);
        }
    }
}
