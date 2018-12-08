using System;
using DomainV2.Equity.Frames;
using DomainV2.Equity.Streams.Interfaces;

namespace TestHarness.Engine.EquitiesStorage.Interfaces
{
    public interface IEquityDataStorage : IObserver<ExchangeFrame>
    {
        void Initiate(IStockExchangeStream stream);

        void Terminate();
    }
}
