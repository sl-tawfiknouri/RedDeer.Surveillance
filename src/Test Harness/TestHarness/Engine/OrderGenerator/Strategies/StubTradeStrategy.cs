using Domain.Equity.Trading.Frames;
using Domain.Equity.Trading.Orders;
using Domain.Equity.Trading.Streams.Interfaces;

namespace TestHarness.Engine.OrderGenerator.Strategies.Interfaces
{
    public class StubTradeStrategy : ITradeStrategy<TradeOrderFrame>
    {
        public void ExecuteTradeStrategy(ExchangeFrame tick, ITradeOrderStream<TradeOrderFrame> tradeOrders)
        {
        }
    }
}
