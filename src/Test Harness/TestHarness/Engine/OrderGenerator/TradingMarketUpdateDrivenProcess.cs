using Domain.Equity.Frames;
using Domain.Trades.Orders;
using NLog;
using TestHarness.Engine.OrderGenerator.Interfaces;
using TestHarness.Engine.OrderGenerator.Strategies;
using TestHarness.Engine.OrderGenerator.Strategies.Interfaces;

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
