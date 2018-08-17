using Domain.Equity.Trading.Frames;
using System;

namespace Domain.Equity.Trading.Streams.Interfaces
{
    public interface IStockExchangeStream : IObservable<ExchangeFrame>
    {
        void Add(ExchangeFrame frame);
    }
}