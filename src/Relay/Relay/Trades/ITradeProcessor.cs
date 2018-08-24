using Domain.Equity.Trading.Orders;
using System;

namespace Relay.Trades
{
    public interface ITradeProcessor<T> : IObserver<T>, IObservable<T>
    {
    }
}
