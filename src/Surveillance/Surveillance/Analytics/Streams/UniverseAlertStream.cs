using System;
using System.Collections.Concurrent;
using Domain.Equity.Streams.Interfaces;
using Surveillance.Analytics.Streams.Interfaces;

namespace Surveillance.Analytics.Streams
{
    public class UniverseAlertStream : IUniverseAlertStream
    {
        private readonly ConcurrentDictionary<IObserver<IUniverseAlertEvent>, IObserver<IUniverseAlertEvent>> _observers;
        private readonly IUnsubscriberFactory<IUniverseAlertEvent> _factory;

        public UniverseAlertStream(IUnsubscriberFactory<IUniverseAlertEvent> unsubscriberFactory)
        {
            _observers = new ConcurrentDictionary<IObserver<IUniverseAlertEvent>, IObserver<IUniverseAlertEvent>>();
            _factory = unsubscriberFactory ?? throw new ArgumentNullException(nameof(unsubscriberFactory));
        }

        public void Add(IUniverseAlertEvent alertEvent)
        {
            if (alertEvent == null)
            {
                return;
            }

            foreach (var sub in _observers)
                sub.Value?.OnNext(alertEvent);
        }

        public IDisposable Subscribe(IObserver<IUniverseAlertEvent> observer)
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
    }
}
