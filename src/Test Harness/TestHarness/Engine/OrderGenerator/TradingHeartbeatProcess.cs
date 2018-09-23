using NLog;
using System;
using Domain.Equity.Frames;
using Domain.Trades.Orders;
using TestHarness.Engine.Heartbeat.Interfaces;
using TestHarness.Engine.OrderGenerator.Interfaces;
using TestHarness.Engine.OrderGenerator.Strategies.Interfaces;

namespace TestHarness.Engine.OrderGenerator
{
    /// <summary>
    /// Equity update driven trading process
    /// </summary>
    public class TradingHeatbeatDrivenProcess : BaseTradingProcess, IOrderDataGenerator
    {
        private ExchangeFrame _lastFrame;
        private readonly IHeartbeat _heartbeat;

        private volatile bool _initiated;
        private readonly object _lock = new object();

        public TradingHeatbeatDrivenProcess(
            ILogger logger,
            ITradeStrategy<TradeOrderFrame> orderStrategy,
            IHeartbeat heartbeat) 
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
                    OrderStrategy.ExecuteTradeStrategy(_lastFrame, TradeStream);
                }
            }
        }
    }
}
