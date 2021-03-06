﻿namespace TestHarness.Engine.OrderGenerator
{
    using Domain.Core.Markets.Collections;
    using Domain.Core.Trading.Orders;

    using Microsoft.Extensions.Logging;

    using TestHarness.Engine.OrderGenerator.Strategies.Interfaces;

    /// <summary>
    ///     Equity update driven trading process
    /// </summary>
    public class TradingMarketUpdateDrivenProcess : BaseTradingProcess
    {
        public TradingMarketUpdateDrivenProcess(ILogger logger, ITradeStrategy<Order> orderStrategy)
            : base(logger, orderStrategy)
        {
        }

        public override void OnNext(EquityIntraDayTimeBarCollection value)
        {
            if (value == null) return;

            this.OrderStrategy.ExecuteTradeStrategy(value, this.TradeStream);
        }

        protected override void _InitiateTrading()
        {
        }

        protected override void _TerminateTradingStrategy()
        {
        }
    }
}