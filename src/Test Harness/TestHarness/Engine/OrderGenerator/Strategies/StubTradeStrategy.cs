using Domain.Equity.Trading.Frames;
using Domain.Equity.Trading.Streams.Interfaces;

namespace TestHarness.Engine.OrderGenerator.Strategies.Interfaces
{
    public class StubTradeStrategy : ITradeStrategy
    {
        public void ExecuteTradeStrategy(ExchangeFrame tick, ITradeOrderStream tradeOrders)
        {
        }
    }
}
