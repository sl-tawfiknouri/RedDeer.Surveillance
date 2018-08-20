using Domain.Equity.Trading.Orders;
using Domain.Equity.Trading.Streams.Interfaces;
using System;
using System.Collections.Concurrent;

namespace Domain.Equity.Trading
{
    public class TradeOrderStream : ITradeOrderStream
    {
        private readonly ConcurrentDictionary<IObserver<TradeOrderFrame>, IObserver<TradeOrderFrame>> _observers;
        private readonly IUnsubscriberFactory<TradeOrderFrame> _unsubscriberFactory;

        public TradeOrderStream(IUnsubscriberFactory<TradeOrderFrame> unsubscriberFactory)
        {
            _observers = new ConcurrentDictionary<IObserver<TradeOrderFrame>, IObserver<TradeOrderFrame>>();
            _unsubscriberFactory = unsubscriberFactory ?? throw new ArgumentNullException(nameof(unsubscriberFactory));
        }

        public IDisposable Subscribe(IObserver<TradeOrderFrame> observer)
        {
            if (observer == null)
            {
                throw new ArgumentNullException(nameof(observer));
            }

            if (!_observers.ContainsKey(observer))
            {
                _observers.TryAdd(observer, observer);
            }

            return _unsubscriberFactory.Create(_observers, observer);
        }

        public void Add(TradeOrderFrame order)
        {
            if (order == null)
            {
                return;
            }

            if (_observers == null)
            {
                return;
            }

            foreach (var obs in _observers)
                obs.Value?.OnNext(order);
        }
    }
}
