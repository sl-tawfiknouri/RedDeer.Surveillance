using Domain.Equity.TimeBars;
using Domain.Streams.Interfaces;

namespace TestHarness.Engine.OrderGenerator.Strategies.Interfaces
{
    public interface ITradeStrategy<T>
    {
        void ExecuteTradeStrategy(EquityIntraDayTimeBarCollection tick, IOrderStream<T> tradeOrders);
    }
}
