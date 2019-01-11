using System;
using DomainV2.Equity.Streams.Interfaces;
using DomainV2.Equity.TimeBars;
using DomainV2.Streams.Interfaces;
using DomainV2.Trading;

namespace TestHarness.Engine.OrderGenerator.Interfaces
{
    public interface IOrderDataGenerator : IObserver<MarketTimeBarCollection>
    {
        void InitiateTrading(IStockExchangeStream stockStream, IOrderStream<Order> tradeStream);
        void TerminateTrading();
    }
}