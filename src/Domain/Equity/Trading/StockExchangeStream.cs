using System;
using System.Collections.Concurrent;

namespace Domain.Equity.Trading
{
    /// <summary>
    /// An observable stream of stock exchange ticks
    /// </summary>
    public class StockExchangeStream : IStockExchangeStream
    {
        private readonly ConcurrentDictionary<IObserver<ExchangeTick>, IObserver<ExchangeTick>> _observers;
        private readonly IUnsubscriberFactory _factory;

        public StockExchangeStream(IUnsubscriberFactory unsubscriberFactory)
        {
            _observers = new ConcurrentDictionary<IObserver<ExchangeTick>, IObserver<ExchangeTick>>();
            _factory = unsubscriberFactory ?? throw new ArgumentNullException(nameof(unsubscriberFactory));
        }

        public IDisposable Subscribe(IObserver<ExchangeTick> observer)
        {
            if (observer == null)
            {
                throw new ArgumentNullException(nameof(observer));
            }

            if (!_observers.ContainsKey(observer))
            {
                _observers.TryAdd(observer, observer);
            }

            return _factory.Create(_observers, observer);
        }

        public void Add(ExchangeTick tick)
        {
            if (tick == null)
            {
                return;
            }

            if (_observers == null)
            {
                return;
            }

            foreach (var obs in _observers)
                obs.Value?.OnNext(tick);
        }
    }
}
