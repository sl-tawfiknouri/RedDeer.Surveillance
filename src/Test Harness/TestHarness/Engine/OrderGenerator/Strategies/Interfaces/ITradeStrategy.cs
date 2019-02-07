using DomainV2.Equity.TimeBars;
using DomainV2.Streams.Interfaces;

namespace TestHarness.Engine.OrderGenerator.Strategies.Interfaces
{
    public interface ITradeStrategy<T>
    {
        void ExecuteTradeStrategy(EquityIntraDayTimeBarCollection tick, IOrderStream<T> tradeOrders);
    }
}
