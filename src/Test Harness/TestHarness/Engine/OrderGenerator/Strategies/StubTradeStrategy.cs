using DomainV2.Equity.TimeBars;
using DomainV2.Streams.Interfaces;
using DomainV2.Trading;
using TestHarness.Engine.OrderGenerator.Strategies.Interfaces;

namespace TestHarness.Engine.OrderGenerator.Strategies
{
    public class StubTradeStrategy : ITradeStrategy<Order>
    {
        public void ExecuteTradeStrategy(MarketTimeBarCollection tick, IOrderStream<Order> tradeOrders)
        {
        }
    }
}
