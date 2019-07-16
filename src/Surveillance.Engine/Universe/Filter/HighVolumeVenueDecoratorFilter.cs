﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Domain.Surveillance.Streams.Interfaces;
using Microsoft.Extensions.Logging;
using Surveillance.Engine.Rules.RuleParameters;
using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
using Surveillance.Engine.Rules.Universe.Interfaces;

namespace Surveillance.Engine.Rules.Universe.Filter
{
    public class HighVolumeVenueDecoratorFilter : IUniverseFilterService
    {
        private readonly Queue<IUniverseEvent> _universeCache;
        private readonly HashSet<UniverseStateEvent> _orderEvents = new HashSet<UniverseStateEvent>
        {
            UniverseStateEvent.Order,
            UniverseStateEvent.OrderFilled,
            UniverseStateEvent.OrderPlaced
        };

        private readonly TimeWindows _ruleTimeWindows;
        private DateTime _windowTime;
        private bool _eschaton;
        private readonly object _lock = new object();
        private readonly IUniverseFilterService _baseService;
        private readonly IUnsubscriberFactory<IUniverseEvent> _universeUnsubscriberFactory;
        private readonly ConcurrentDictionary<IObserver<IUniverseEvent>, IObserver<IUniverseEvent>> _universeObservers;

        private readonly ILogger<HighVolumeVenueFilter> _logger;

        public HighVolumeVenueDecoratorFilter(
            TimeWindows timeWindows,
            IUniverseFilterService baseService,
            IUnsubscriberFactory<IUniverseEvent> universeUnsubscriberFactory,
            ILogger<HighVolumeVenueFilter> logger)
        {
            _ruleTimeWindows = timeWindows ?? throw new ArgumentNullException(nameof(timeWindows));
            _baseService = baseService ?? throw new ArgumentNullException(nameof(baseService));
            _universeCache = new Queue<IUniverseEvent>();
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

            lock (_lock)
            {
                _universeCache.Enqueue(value);
                _windowTime = value.EventTime;

                if (value.StateChange == UniverseStateEvent.Eschaton)
                {
                    _eschaton = true;
                }

                if (_orderEvents.Contains(value.StateChange))
                {
                    // perform filtering @ trade time
                    // perform filtering against cached history at market close for each day
                    // so we imagine there being a separate class that is fed these events as they occur
                    // in alignment with real time

                    // a separate class holds the filtering
                    

                }

                ProcessCache();
            }
        }

        private void ProcessCache()
        {
            if (!_universeCache.Any())
            {
                return;
            }

            while (_universeCache.Any()
                   && 
                    (_universeCache.Peek().EventTime <= FilterTime()
                     || _eschaton))
            {
                var value = _universeCache.Dequeue();
                
                foreach (var obs in _universeObservers)
                {
                    obs.Value.OnNext(value);
                }
            }
        }

        /// <summary>
        /// Alice: “How long is forever?"
        /// White Rabbit: “Sometimes, just one second."
        /// </summary>
        private DateTime FilterTime()
        {
            if (_ruleTimeWindows == null)
            {
                return _windowTime;
            }

            return _windowTime - _ruleTimeWindows.BackwardWindowSize;
        }
    }
}
