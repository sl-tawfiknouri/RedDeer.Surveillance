using System;

namespace Domain.Equity.Trading
{
    public interface IStockExchangeStream : IObservable<ExchangeFrame>
    {
        void Add(ExchangeFrame tick);
    }
}