namespace TestHarness.Engine.OrderGenerator.Strategies
{
    using Domain.Core.Markets.Collections;
    using Domain.Core.Trading.Orders;
    using Domain.Surveillance.Streams.Interfaces;

    using TestHarness.Engine.OrderGenerator.Strategies.Interfaces;

    public class StubTradeStrategy : ITradeStrategy<Order>
    {
        public void ExecuteTradeStrategy(EquityIntraDayTimeBarCollection tick, IOrderStream<Order> tradeOrders)
        {
        }
    }
}