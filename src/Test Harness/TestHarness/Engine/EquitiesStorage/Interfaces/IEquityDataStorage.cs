using System;
using DomainV2.Equity.Streams.Interfaces;
using DomainV2.Equity.TimeBars;

namespace TestHarness.Engine.EquitiesStorage.Interfaces
{
    public interface IEquityDataStorage : IObserver<EquityIntraDayTimeBarCollection>
    {
        void Initiate(IStockExchangeStream stream);

        void Terminate();
    }
}
