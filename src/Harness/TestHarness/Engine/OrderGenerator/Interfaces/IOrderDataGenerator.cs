using Domain.Equity.Trading.Frames;
using Domain.Equity.Trading.Streams.Interfaces;
using System;

namespace TestHarness.Engine.OrderGenerator.Interfaces
{
    public interface IOrderDataGenerator : IObserver<ExchangeFrame>
    {
        void InitiateTrading(IStockExchangeStream stockStream, ITradeOrderStream tradeStream);
        void TerminateTrading();
    }
}