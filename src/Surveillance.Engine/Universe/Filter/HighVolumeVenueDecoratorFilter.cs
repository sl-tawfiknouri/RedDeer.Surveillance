using System;
using System.Collections.Generic;
using System.Linq;
using Surveillance.Engine.Rules.RuleParameters;
using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
using Surveillance.Engine.Rules.Universe.Interfaces;

namespace Surveillance.Engine.Rules.Universe.Filter
{
    public class HighVolumeVenueDecoratorFilter : IHighVolumeVenueDecoratorFilter
    {
        private readonly Queue<IUniverseEvent> _universeCache;
        private readonly TimeWindows _ruleTimeWindows;
        private DateTime _windowTime;
        private bool _eschaton;
        private readonly object _lock = new object();
        private readonly IUniverseFilterService _baseService;
        private readonly IHighVolumeVenueFilter _highVolumeVenueFilter;
        
        public HighVolumeVenueDecoratorFilter(
            TimeWindows timeWindows,
            IUniverseFilterService baseService,
            IHighVolumeVenueFilter highVolumeVenueFilter)
        {
            _ruleTimeWindows = timeWindows ?? throw new ArgumentNullException(nameof(timeWindows));
            _baseService = baseService ?? throw new ArgumentNullException(nameof(baseService));
            _universeCache = new Queue<IUniverseEvent>();
            _highVolumeVenueFilter = highVolumeVenueFilter;
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

            return _baseService.Subscribe(observer);
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
                _highVolumeVenueFilter.OnNext(value);

                _universeCache.Enqueue(value);
                _windowTime = value.EventTime;

                if (value.StateChange == UniverseStateEvent.Eschaton)
                {
                    _eschaton = true;
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
                   && (_universeCache.Peek().EventTime <= FilterTime()
                        || _eschaton))
            {
                var value = _universeCache.Dequeue();

                if (value.StateChange.IsOrderType()
                   && !_highVolumeVenueFilter.UniverseEventsPassedFilter.Contains(value.UnderlyingEvent))
                {
                    // this event was not verified by the filter
                    continue;
                }

                _baseService.OnNext(value);
            }

            if (_eschaton)
            {
                _highVolumeVenueFilter?.UniverseEventsPassedFilter.Clear();
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