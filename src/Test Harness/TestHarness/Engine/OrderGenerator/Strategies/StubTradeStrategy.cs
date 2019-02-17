using Domain.Equity.TimeBars;
using Domain.Streams.Interfaces;
using Domain.Trading;
using TestHarness.Engine.OrderGenerator.Strategies.Interfaces;

namespace TestHarness.Engine.OrderGenerator.Strategies
{
    public class StubTradeStrategy : ITradeStrategy<Order>
    {
        public void ExecuteTradeStrategy(EquityIntraDayTimeBarCollection tick, IOrderStream<Order> tradeOrders)
        {
        }
    }
}
