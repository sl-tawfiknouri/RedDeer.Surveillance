using System;
using System.Collections.Concurrent;

namespace Domain.Equity.Trading
{
    public class UnsubscriberFactory : IUnsubscriberFactory
    {
        /// <summary>
        /// Create unsubscriber instances
        /// </summary>
        public Unsubscriber Create(
            ConcurrentDictionary<IObserver<ExchangeTick>, IObserver<ExchangeTick>> observers,
            IObserver<ExchangeTick> observer)
        {
            return new Unsubscriber(observers, observer);
        }
    }
}
