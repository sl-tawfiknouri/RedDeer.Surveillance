using System;
using Domain.Equity.Streams.Interfaces;
using Domain.Equity.TimeBars;

namespace TestHarness.Engine.EquitiesStorage.Interfaces
{
    public interface IEquityDataStorage : IObserver<EquityIntraDayTimeBarCollection>
    {
        void Initiate(IStockExchangeStream stream);

        void Terminate();
    }
}
