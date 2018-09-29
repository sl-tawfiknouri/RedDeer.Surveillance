using System;
using System.Collections.Concurrent;
using System.Linq;
using Domain.Equity.Streams.Interfaces;
using Surveillance.Universe.Interfaces;

namespace Surveillance.Universe
{
    /// <summary>
    /// Play the history of the universe to the observers
    /// </summary>
    public class UniversePlayer : IUniversePlayer
    {
        private readonly IUnsubscriberFactory<IUniverseEvent> _universeUnsubscriberFactory;
        private readonly ConcurrentDictionary<IObserver<IUniverseEvent>, IObserver<IUniverseEvent>> _universeObservers;

        public UniversePlayer(
            IUnsubscriberFactory<IUniverseEvent> universeUnsubscriberFactory)
        {
            _universeObservers = new ConcurrentDictionary<IObserver<IUniverseEvent>, IObserver<IUniverseEvent>>();
            _universeUnsubscriberFactory =
                universeUnsubscriberFactory
                ?? throw new ArgumentNullException(nameof(universeUnsubscriberFactory));
        }

        public void Play(IUniverse universe)
        {
            if (universe == null
                || !universe.Trades.Any())
            {
                return;
            }

            PlayUniverse(universe);
        }

        private void PlayUniverse(IUniverse universe)
        {
            if (_universeObservers == null
                || !_universeObservers.Any())
            {
                return;
            }

            foreach (var item in universe.UniverseEvents)
            {
                foreach (var obs in _universeObservers)
                {
                    obs.Value?.OnNext(item);
                }
            }
        }

        /// <summary>
        /// Subscribe to the universe
        /// </summary>
        public IDisposable Subscribe(IObserver<IUniverseEvent> observer)
        {
            if (observer == null)
            {
                throw new ArgumentNullException(nameof(observer));
            }

            if (!_universeObservers.ContainsKey(observer))
            {
                _universeObservers.TryAdd(observer, observer);
            }

            return _universeUnsubscriberFactory.Create(_universeObservers, observer);
        }
    }
}