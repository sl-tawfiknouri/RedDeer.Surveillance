namespace TestHarness.Engine.OrderGenerator.Strategies.Interfaces
{
    using Domain.Core.Markets.Collections;
    using Domain.Surveillance.Streams.Interfaces;

    public interface ITradeStrategy<T>
    {
        void ExecuteTradeStrategy(EquityIntraDayTimeBarCollection tick, IOrderStream<T> tradeOrders);
    }
}