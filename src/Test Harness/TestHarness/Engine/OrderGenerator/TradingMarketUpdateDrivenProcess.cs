using Domain.Equity.Trading.Frames;
using Domain.Equity.Trading.Orders;
using NLog;
using TestHarness.Engine.OrderGenerator.Interfaces;
using TestHarness.Engine.OrderGenerator.Strategies;

namespace TestHarness.Engine.OrderGenerator
{
    /// <summary>
    /// Equity update driven trading process
    /// </summary>
    public class TradingMarketUpdateDrivenProcess : BaseTradingProcess, IOrderDataGenerator
    {
        public TradingMarketUpdateDrivenProcess(
            ILogger logger,
            ITradeStrategy<TradeOrderFrame> orderStrategy) 
            : base(logger, orderStrategy)
        {
        }

        public override void OnNext(ExchangeFrame value)
        {
            if (value == null)
            {
                return;
            }

            _orderStrategy.ExecuteTradeStrategy(value, _tradeStream);
        }

        protected override void _InitiateTrading()
        {
        }

        protected override void _TerminateTradingStrategy()
        {
        }
    }
}
