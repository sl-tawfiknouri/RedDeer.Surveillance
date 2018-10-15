using System;
using Domain.Equity.Frames;
using Domain.Equity.Streams.Interfaces;

namespace TestHarness.Engine.EquitiesStorage.Interfaces
{
    public interface IEquityDataStorage : IObserver<ExchangeFrame>
    {
        void Initiate(IStockExchangeStream stream);

        void Terminate();
    }
}
