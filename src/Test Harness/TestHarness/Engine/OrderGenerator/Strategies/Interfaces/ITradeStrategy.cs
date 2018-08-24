using Domain.Equity.Trading.Frames;
using Domain.Equity.Trading.Orders;
using Domain.Equity.Trading.Streams.Interfaces;

namespace TestHarness.Engine.OrderGenerator.Strategies
{
    public interface ITradeStrategy<T>
    {
        void ExecuteTradeStrategy(ExchangeFrame tick, ITradeOrderStream<T> tradeOrders);
    }
}
