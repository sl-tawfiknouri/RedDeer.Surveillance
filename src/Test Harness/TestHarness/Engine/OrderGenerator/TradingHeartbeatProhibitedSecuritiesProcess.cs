using NLog;
using System;
using TestHarness.Engine.Heartbeat.Interfaces;
using Domain.Equity;
using Domain.Equity.Frames;
using Domain.Trades.Orders;
using TestHarness.Engine.OrderGenerator.Strategies.Interfaces;

namespace TestHarness.Engine.OrderGenerator
{
    public class TradingHeartbeatProhibitedSecuritiesProcess : BaseTradingProcess
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
                    // ReSharper disable StringLiteralTypo
                    var submittedOn = DateTime.UtcNow.AddMilliseconds(-300);

                    var securityIdentifiers =
                        new SecurityIdentifiers(
                            "Lehman Bros",
                            "LB12345",
                            "LB123456789X",
                            "LBro",
                            "123456LB",
                            "LEHM",
                            "LEHM",
                            "LEHM");

                    var security = new Security(
                        securityIdentifiers,
                        "Lehman Bros",
                        "CFI",
                        "Lehman Bros");

                    var prohibitedTrade = new TradeOrderFrame
                        (OrderType.Market,
                        _lastFrame.Exchange,
                        security,
                        null,
                        new Price(50, "GBP"), 
                        666,
                        666,
                        OrderPosition.Buy,
                        OrderStatus.Booked,
                        DateTime.UtcNow,
                        submittedOn,
                        "TRADER-1",
                        "TRADER-1-CLIENT-1",
                        "ACCOUNT-12345",
                        "BUY ASAP",
                        "BROKER-1",
                        "BROKER-2",
                        "Lehman Bros looks like good value for money",
                        "Buy Long");

                    TradeStream.Add(prohibitedTrade);

                    // ReSharper restore StringLiteralTypo
                }
            }
        }
    }
}
