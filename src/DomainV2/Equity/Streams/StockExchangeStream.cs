using System;
using System.Collections.Concurrent;
using DomainV2.Equity.Streams.Interfaces;
using DomainV2.Equity.TimeBars;

namespace DomainV2.Equity.Streams
{
    /// <summary>
    /// An observable stream of stock exchange ticks
    /// </summary>
    public class StockExchangeStream : IStockExchangeStream
    {
        private readonly ConcurrentDictionary<IObserver<MarketTimeBarCollection>, IObserver<MarketTimeBarCollection>> _observers;
        private readonly IUnsubscriberFactory<MarketTimeBarCollection> _factory;

        public StockExchangeStream(IUnsubscriberFactory<MarketTimeBarCollection> unsubscriberFactory)
        {
            _observers = new ConcurrentDictionary<IObserver<MarketTimeBarCollection>, IObserver<MarketTimeBarCollection>>();
            _factory = unsubscriberFactory ?? throw new ArgumentNullException(nameof(unsubscriberFactory));
        }

        public IDisposable Subscribe(IObserver<MarketTimeBarCollection> observer)
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

        public void Add(MarketTimeBarCollection frame)
        {
            if (frame == null)
            {
                return;
            }

            if (_observers == null)
            {
                return;
            }

            foreach (var obs in _observers)
                obs.Value?.OnNext(frame);
        }
    }
}
