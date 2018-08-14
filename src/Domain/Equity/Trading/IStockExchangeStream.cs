using System;

namespace Domain.Equity.Trading
{
    public interface IStockExchangeStream : IObservable<ExchangeTick>
    {
        void Add(ExchangeTick tick);
    }
}