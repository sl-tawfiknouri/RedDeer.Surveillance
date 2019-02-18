using System;
using System.Collections.Concurrent;
using DomainV2.Equity.Streams.Interfaces;
using Microsoft.Extensions.Logging;
using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;

namespace Surveillance.Engine.Rules.Analytics.Streams
{
    public class UniverseAlertStream : IUniverseAlertStream
    {
        private readonly ConcurrentDictionary<IObserver<IUniverseAlertEvent>, IObserver<IUniverseAlertEvent>> _observers;
        private readonly IUnsubscriberFactory<IUniverseAlertEvent> _factory;
        private readonly ILogger<UniverseAlertStream> _logger;

        public UniverseAlertStream(
            IUnsubscriberFactory<IUniverseAlertEvent> unsubscriberFactory,
            ILogger<UniverseAlertStream> logger)
        {
            _observers = new ConcurrentDictionary<IObserver<IUniverseAlertEvent>, IObserver<IUniverseAlertEvent>>();
            _factory = unsubscriberFactory ?? throw new ArgumentNullException(nameof(unsubscriberFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Add(IUniverseAlertEvent alertEvent)
        {
            if (alertEvent == null)
            {
                return;
            }

            _logger.LogInformation($"UniverseAlertStream received a new alert for rule {alertEvent.Rule}. Part of rule run {alertEvent.Context?.Id()}");

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
                _logger.LogInformation($"UniverseAlertStream added a new observer {observer}");
                _observers.TryAdd(observer, observer);
            }

            return _factory.Create(_observers, observer);
        }
    }
}
