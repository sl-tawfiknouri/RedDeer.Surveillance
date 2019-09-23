namespace TestHarness.Engine.EquitiesStorage.Interfaces
{
    using System;

    using Domain.Core.Markets.Collections;
    using Domain.Surveillance.Streams.Interfaces;

    public interface IEquityDataStorage : IObserver<EquityIntraDayTimeBarCollection>
    {
        void Initiate(IStockExchangeStream stream);

        void Terminate();
    }
}