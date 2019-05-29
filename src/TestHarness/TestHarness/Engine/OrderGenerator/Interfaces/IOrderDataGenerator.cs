using System;
using Domain.Core.Markets.Collections;
using Domain.Core.Trading.Orders;
using Domain.Surveillance.Streams.Interfaces;

namespace TestHarness.Engine.OrderGenerator.Interfaces
{
    public interface IOrderDataGenerator : IObserver<EquityIntraDayTimeBarCollection>
    {
        void InitiateTrading(IStockExchangeStream stockStream, IOrderStream<Order> tradeStream);
        void TerminateTrading();
    }
}