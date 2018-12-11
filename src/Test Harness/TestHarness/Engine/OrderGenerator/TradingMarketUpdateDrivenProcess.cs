using DomainV2.Equity.Frames;
using DomainV2.Trading;
using Microsoft.Extensions.Logging;
using TestHarness.Engine.OrderGenerator.Strategies.Interfaces;

namespace TestHarness.Engine.OrderGenerator
{
    /// <summary>
    /// Equity update driven trading process
    /// </summary>
    public class TradingMarketUpdateDrivenProcess : BaseTradingProcess
    {
        public TradingMarketUpdateDrivenProcess(
            ILogger logger,
            ITradeStrategy<Order> orderStrategy) 
            : base(logger, orderStrategy)
        {
        }

        public override void OnNext(ExchangeFrame value)
        {
            if (value == null)
            {
                return;
            }

            OrderStrategy.ExecuteTradeStrategy(value, TradeStream);
        }

        protected override void _InitiateTrading()
        {
        }

        protected override void _TerminateTradingStrategy()
        {
        }
    }
}
