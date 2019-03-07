using System;
using Domain.Core.Markets.Collections;
using Domain.Surveillance.Streams.Interfaces;

namespace TestHarness.Engine.EquitiesStorage.Interfaces
{
    public interface IEquityDataStorage : IObserver<EquityIntraDayTimeBarCollection>
    {
        void Initiate(IStockExchangeStream stream);

        void Terminate();
    }
}
