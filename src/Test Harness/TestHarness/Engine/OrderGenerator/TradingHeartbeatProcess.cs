﻿using System;
using DomainV2.Equity.TimeBars;
using DomainV2.Trading;
using Microsoft.Extensions.Logging;
using TestHarness.Engine.Heartbeat.Interfaces;
using TestHarness.Engine.OrderGenerator.Strategies.Interfaces;

namespace TestHarness.Engine.OrderGenerator
{
    /// <summary>
    /// Equity update driven trading process
    /// </summary>
    public class TradingHeartBeatDrivenProcess : BaseTradingProcess
    {
        private MarketTimeBarCollection _lastFrame;
        private readonly IHeartbeat _heartbeat;

        private volatile bool _initiated;
        private readonly object _lock = new object();

        public TradingHeartBeatDrivenProcess(
            ILogger logger,
            ITradeStrategy<Order> orderStrategy,
            IHeartbeat heartbeat) 
            : base(logger, orderStrategy)
        {
            _heartbeat = heartbeat ?? throw new ArgumentNullException(nameof(heartbeat));
        }

        public override void OnNext(MarketTimeBarCollection value)
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
