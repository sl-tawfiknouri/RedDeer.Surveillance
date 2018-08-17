using Domain.Equity.Trading;

namespace TestHarness.Engine.OrderGenerator.Strategies
{
    public class ProbabilisticTradeStrategy : ITradeStrategy
    {
        public void ExecuteTradeStrategy(ExchangeFrame tick, ITradeOrderStream tradeOrders)
        {
            if (tick == null)
            {
                return;
            }

            // set square root of securities as mean

            // 

        }
    }
}
