using System;
using System.Collections.Concurrent;

namespace Domain.Equity.Trading
{
    public interface IUnsubscriberFactory
    {
        Unsubscriber Create(ConcurrentDictionary<IObserver<ExchangeTick>, IObserver<ExchangeTick>> observers, IObserver<ExchangeTick> observer);
    }
}