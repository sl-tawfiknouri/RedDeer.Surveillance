using System;
using Domain.Equity.Trading.Orders;

namespace Domain.Equity.Trading.Streams.Interfaces
{
    public interface ITradeOrderStream : IObservable<TradeOrderFrame>
    {
        void Add(TradeOrderFrame order);
    }
}
