using Domain.Core.Markets.Collections;
using Domain.Surveillance.Streams.Interfaces;

namespace TestHarness.Engine.OrderGenerator.Strategies.Interfaces
{
    public interface ITradeStrategy<T>
    {
        void ExecuteTradeStrategy(EquityIntraDayTimeBarCollection tick, IOrderStream<T> tradeOrders);
    }
}
