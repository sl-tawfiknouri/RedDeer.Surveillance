using System;
using System.Collections.Concurrent;
using System.Linq;
using Domain.Equity.Streams.Interfaces;
using Domain.Trades.Orders;
using Surveillance.Universe.Interfaces;

namespace Surveillance.Universe
{
    /// <summary>
    /// Play the history of the universe to the observers
    /// </summary>
    public class UniversePlayer : IUniversePlayer
    {
        private readonly IUnsubscriberFactory<TradeOrderFrame> _unsubscriberFactory;
        private readonly ConcurrentDictionary<IObserver<TradeOrderFrame>, IObserver<TradeOrderFrame>> _observers;

        public UniversePlayer(IUnsubscriberFactory<TradeOrderFrame> unsubscriberFactory)
        {
            _observers = new ConcurrentDictionary<IObserver<TradeOrderFrame>, IObserver<TradeOrderFrame>>();
            _unsubscriberFactory = unsubscriberFactory ?? throw new ArgumentNullException(nameof(unsubscriberFactory));
        }

        public void Play(IUniverse universe)
        {
            if (universe == null
                || !universe.Trades.Any())
            {
                return;
            }

            if (_observers == null
                || !_observers.Any())
            {
                return;
            }

            foreach (var item in universe.Trades)
            {
                foreach (var obs in _observers)
                {
                    obs.Value?.OnNext(item);
                }
            }
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
    }
}
