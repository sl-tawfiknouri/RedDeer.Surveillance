using System;
using System.Collections.Concurrent;
using Domain.Surveillance.Streams.Interfaces;
using Microsoft.Extensions.Logging;
using Surveillance.Engine.Rules.RuleParameters;
using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
using Surveillance.Engine.Rules.Universe.Interfaces;

namespace Surveillance.Engine.Rules.Universe.Filter
{
    public class HighVolumeVenueDecoratorFilter : IUniverseFilterService
    {
        private readonly TimeWindows _timeWindows;
        private readonly IUniverseFilterService _baseService;
        private readonly IUnsubscriberFactory<IUniverseEvent> _universeUnsubscriberFactory;
        private readonly ConcurrentDictionary<IObserver<IUniverseEvent>, IObserver<IUniverseEvent>> _universeObservers;

        private readonly ILogger<IHighVolumeVenueFilter> _logger;

        public HighVolumeVenueDecoratorFilter(
            TimeWindows timeWindows,
            IUniverseFilterService baseService,
            IUnsubscriberFactory<IUniverseEvent> universeUnsubscriberFactory,
            ILogger<IHighVolumeVenueFilter> logger)
        {
            _timeWindows = timeWindows ?? throw new ArgumentNullException(nameof(timeWindows));
            _baseService = baseService ?? throw new ArgumentNullException(nameof(baseService));
            _universeObservers = new ConcurrentDictionary<IObserver<IUniverseEvent>, IObserver<IUniverseEvent>>();
            _universeUnsubscriberFactory = universeUnsubscriberFactory ?? throw new ArgumentNullException(nameof(universeUnsubscriberFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Domain.Surveillance.Scheduling.Rules Rule => _baseService.Rule;
        public string Version => _baseService.Version;

        public IDisposable Subscribe(IObserver<IUniverseEvent> observer)
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (observer == null)
            {
                return null;
            }

            if (!_universeObservers.ContainsKey(observer))
            {
                _logger.LogInformation($"subscribing a new observer");
                _universeObservers.TryAdd(observer, observer);
            }

            return _universeUnsubscriberFactory.Create(_universeObservers, observer);
        }

        public void OnCompleted()
        {
            _baseService.OnCompleted();
        }

        public void OnError(Exception error)
        {
            _baseService.OnError(error);
        }

        public void OnNext(IUniverseEvent value)
        {
            if (value == null)
            {
                return;
            }

            foreach (var obs in _universeObservers)
            {
                obs.Value.OnNext(value);
            }
        }
    }
}
