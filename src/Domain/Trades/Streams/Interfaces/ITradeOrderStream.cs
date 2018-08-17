using System;
using Domain.Equity.Trading.Orders;

namespace Domain.Equity.Trading.Streams.Interfaces
{
    public interface ITradeOrderStream : IObservable<TradeOrder>
    {
        void Add(TradeOrder order);
    }
}
