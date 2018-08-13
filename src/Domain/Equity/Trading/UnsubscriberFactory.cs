using System;
using System.Collections.Concurrent;

namespace Domain.Equity.Trading
{
    public class UnsubscriberFactory : IUnsubscriberFactory
    {
        public Unsubscriber Create(
            ConcurrentDictionary<IObserver<ExchangeTick>, IObserver<ExchangeTick>> observers,
            IObserver<ExchangeTick> observer)
        {
            return new Unsubscriber(observers, observer);
        }
    }
}
