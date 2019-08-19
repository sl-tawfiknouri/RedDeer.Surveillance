namespace Surveillance.Engine.Rules.Analytics.Streams
{
    using System;
    using System.Collections.Concurrent;

    using Domain.Surveillance.Streams.Interfaces;

    using Microsoft.Extensions.Logging;

    using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;

    public class UniverseAlertStream : IUniverseAlertStream
    {
        private readonly IUnsubscriberFactory<IUniverseAlertEvent> _factory;

        private readonly ILogger<UniverseAlertStream> _logger;

        private readonly ConcurrentDictionary<IObserver<IUniverseAlertEvent>, IObserver<IUniverseAlertEvent>> _observers;

        public UniverseAlertStream(
            IUnsubscriberFactory<IUniverseAlertEvent> unsubscriberFactory,
            ILogger<UniverseAlertStream> logger)
        {
            this._observers =
                new ConcurrentDictionary<IObserver<IUniverseAlertEvent>, IObserver<IUniverseAlertEvent>>();
            this._factory = unsubscriberFactory ?? throw new ArgumentNullException(nameof(unsubscriberFactory));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Add(IUniverseAlertEvent alertEvent)
        {
            if (alertEvent == null) return;

            this._logger.LogInformation(
                $"received a new alert for rule {alertEvent.Rule}. Part of rule run {alertEvent.Context?.Id()}");

            foreach (var sub in this._observers)
                sub.Value?.OnNext(alertEvent);
        }

        public IDisposable Subscribe(IObserver<IUniverseAlertEvent> observer)
        {
            if (observer == null) throw new ArgumentNullException(nameof(observer));

            if (!this._observers.ContainsKey(observer))
            {
                this._logger.LogInformation($"added a new observer {observer}");
                this._observers.TryAdd(observer, observer);
            }

            return this._factory.Create(this._observers, observer);
        }
    }
}