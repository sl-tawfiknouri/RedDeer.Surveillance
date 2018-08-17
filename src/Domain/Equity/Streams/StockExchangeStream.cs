using Domain.Equity.Trading.Frames;
using Domain.Equity.Trading.Streams.Interfaces;
using System;
using System.Collections.Concurrent;

namespace Domain.Equity.Trading
{
    /// <summary>
    /// An observable stream of stock exchange ticks
    /// </summary>
    public class StockExchangeStream : IStockExchangeStream
    {
        private readonly ConcurrentDictionary<IObserver<ExchangeFrame>, IObserver<ExchangeFrame>> _observers;
        private readonly IUnsubscriberFactory<ExchangeFrame> _factory;

        public StockExchangeStream(IUnsubscriberFactory<ExchangeFrame> unsubscriberFactory)
        {
            _observers = new ConcurrentDictionary<IObserver<ExchangeFrame>, IObserver<ExchangeFrame>>();
            _factory = unsubscriberFactory ?? throw new ArgumentNullException(nameof(unsubscriberFactory));
        }

        public IDisposable Subscribe(IObserver<ExchangeFrame> observer)
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

        public void Add(ExchangeFrame frame)
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
