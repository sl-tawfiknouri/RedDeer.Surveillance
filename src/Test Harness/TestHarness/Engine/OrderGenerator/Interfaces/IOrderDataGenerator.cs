using System;
using Domain.Equity.Streams.Interfaces;
using Domain.Equity.TimeBars;
using Domain.Streams.Interfaces;
using Domain.Trading;

namespace TestHarness.Engine.OrderGenerator.Interfaces
{
    public interface IOrderDataGenerator : IObserver<EquityIntraDayTimeBarCollection>
    {
        void InitiateTrading(IStockExchangeStream stockStream, IOrderStream<Order> tradeStream);
        void TerminateTrading();
    }
}