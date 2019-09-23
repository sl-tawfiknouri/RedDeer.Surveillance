namespace TestHarness.Engine.OrderGenerator
{
    using System;

    using Domain.Core.Markets.Collections;
    using Domain.Core.Trading.Orders;

    using Microsoft.Extensions.Logging;

    using TestHarness.Engine.Heartbeat.Interfaces;
    using TestHarness.Engine.OrderGenerator.Strategies.Interfaces;

    /// <summary>
    ///     Equity update driven trading process
    /// </summary>
    public class TradingHeartBeatDrivenProcess : BaseTradingProcess
    {
        private readonly IHeartbeat _heartbeat;

        private readonly object _lock = new object();

        private volatile bool _initiated;

        private EquityIntraDayTimeBarCollection _lastFrame;

        public TradingHeartBeatDrivenProcess(ILogger logger, ITradeStrategy<Order> orderStrategy, IHeartbeat heartbeat)
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

        private void TradeOnHeartbeat(object sender, EventArgs e)
        {
            lock (this._lock)
            {
                if (this._lastFrame != null) this.OrderStrategy.ExecuteTradeStrategy(this._lastFrame, this.TradeStream);
            }
        }
    }
}