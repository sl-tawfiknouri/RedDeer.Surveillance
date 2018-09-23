using NLog;
using System;
using TestHarness.Engine.Heartbeat.Interfaces;
using Domain.Equity;
using Domain.Equity.Frames;
using Domain.Trades.Orders;
using TestHarness.Engine.OrderGenerator.Interfaces;
using TestHarness.Engine.OrderGenerator.Strategies.Interfaces;

namespace TestHarness.Engine.OrderGenerator
{
    public class TradingHeartbeatProhibitedSecuritiesProcess : BaseTradingProcess, IOrderDataGenerator
    {
        private ExchangeFrame _lastFrame;
        private readonly IPulsatingHeartbeat _heartbeat;

        private volatile bool _initiated;
        private readonly object _lock = new object();

        public TradingHeartbeatProhibitedSecuritiesProcess(
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
                if (_lastFrame != null)
                {
                    var submittedOn = DateTime.UtcNow.AddMilliseconds(-300);

                    var prohibitedTrade = new TradeOrderFrame
                        (OrderType.Market,
                        _lastFrame.Exchange,
                        new Security(
                            new SecurityIdentifiers("Lehman Bros", "LB12345", "LB123456789X", "LBro", "123456LB", "LEHM"),
                            "Lehman Bros",
                            "CFI"),
                        null,
                        666,
                        OrderPosition.BuyLong,
                        OrderStatus.Placed,
                        DateTime.UtcNow,
                        submittedOn,
                        "TRADER-1",
                        "",
                        "BROKER-1",
                        "BROKER-2");

                    TradeStream.Add(prohibitedTrade);
                }
            }
        }
    }
}
