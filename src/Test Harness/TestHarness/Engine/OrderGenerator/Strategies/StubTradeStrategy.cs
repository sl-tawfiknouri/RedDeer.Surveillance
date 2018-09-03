using Domain.Equity.Frames;
using Domain.Trades.Orders;
using Domain.Trades.Streams.Interfaces;
using TestHarness.Engine.OrderGenerator.Strategies.Interfaces;

namespace TestHarness.Engine.OrderGenerator.Strategies
{
    public class StubTradeStrategy : ITradeStrategy<TradeOrderFrame>
    {
        public void ExecuteTradeStrategy(ExchangeFrame tick, ITradeOrderStream<TradeOrderFrame> tradeOrders)
        {
        }
    }
}
