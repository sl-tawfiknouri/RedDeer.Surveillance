using DomainV2.Equity.Frames;
using DomainV2.Streams.Interfaces;

namespace TestHarness.Engine.OrderGenerator.Strategies.Interfaces
{
    public interface ITradeStrategy<T>
    {
        void ExecuteTradeStrategy(ExchangeFrame tick, IOrderStream<T> tradeOrders);
    }
}
