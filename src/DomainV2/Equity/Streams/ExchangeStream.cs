using System;
using System.Collections.Concurrent;
using Domain.Equity.Streams.Interfaces;
using Domain.Equity.TimeBars;

namespace Domain.Equity.Streams
{
    /// <summary>
    /// An observable stream of stock exchange time bars
    /// </summary>
    public class ExchangeStream : IStockExchangeStream
    {
        private readonly ConcurrentDictionary<IObserver<EquityIntraDayTimeBarCollection>, IObserver<EquityIntraDayTimeBarCollection>> _observers;
        private readonly IUnsubscriberFactory<EquityIntraDayTimeBarCollection> _factory;

        public ExchangeStream(IUnsubscriberFactory<EquityIntraDayTimeBarCollection> unsubscriberFactory)
        {
            _observers = new ConcurrentDictionary<IObserver<EquityIntraDayTimeBarCollection>, IObserver<EquityIntraDayTimeBarCollection>>();
            _factory = unsubscriberFactory ?? throw new ArgumentNullException(nameof(unsubscriberFactory));
        }

        public IDisposable Subscribe(IObserver<EquityIntraDayTimeBarCollection> observer)
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

        public void Add(EquityIntraDayTimeBarCollection frame)
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
