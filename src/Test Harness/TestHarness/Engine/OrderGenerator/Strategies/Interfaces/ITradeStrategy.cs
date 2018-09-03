using Domain.Equity.Frames;
using Domain.Trades.Streams.Interfaces;

namespace TestHarness.Engine.OrderGenerator.Strategies.Interfaces
{
    public interface ITradeStrategy<T>
    {
        void ExecuteTradeStrategy(ExchangeFrame tick, ITradeOrderStream<T> tradeOrders);
    }
}
