using Domain.Core.Markets.Collections;
using Domain.Core.Trading.Orders;
using Domain.Surveillance.Streams.Interfaces;
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
