using System;
using Domain.Equity.Trading.Orders;

namespace Domain.Equity.Trading
{
    public interface ITradeOrderStream : IObservable<TradeOrder>
    {
        void Add(TradeOrder order);
    }
}
