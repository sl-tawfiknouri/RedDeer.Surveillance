namespace Surveillance.Engine.Rules.Universe.Filter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Domain.Surveillance.Scheduling;

    using Surveillance.Engine.Rules.RuleParameters;
    using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
    using Surveillance.Engine.Rules.Universe.Interfaces;

    public class HighVolumeVenueDecoratorFilter : IHighVolumeVenueDecoratorFilter
    {
        private readonly IUniverseFilterService _baseService;

        private readonly IHighVolumeVenueFilter _highVolumeVenueFilter;

        private readonly object _lock = new object();

        private readonly TimeWindows _ruleTimeWindows;

        private readonly Queue<IUniverseEvent> _universeCache;

        private bool _eschaton;

        private DateTime _windowTime;

        public HighVolumeVenueDecoratorFilter(
            TimeWindows timeWindows,
            IUniverseFilterService baseService,
            IHighVolumeVenueFilter highVolumeVenueFilter)
        {
            this._ruleTimeWindows = timeWindows ?? throw new ArgumentNullException(nameof(timeWindows));
            this._baseService = baseService ?? throw new ArgumentNullException(nameof(baseService));
            this._universeCache = new Queue<IUniverseEvent>();
            this._highVolumeVenueFilter = highVolumeVenueFilter;
        }

        public Rules Rule => this._baseService.Rule;

        public string Version => this._baseService.Version;

        public void OnCompleted()
        {
            this._baseService.OnCompleted();
        }

        public void OnError(Exception error)
        {
            this._baseService.OnError(error);
        }

        public void OnNext(IUniverseEvent value)
        {
            if (value == null) return;

            lock (this._lock)
            {
                this._highVolumeVenueFilter.OnNext(value);

                this._universeCache.Enqueue(value);
                this._windowTime = value.EventTime;

                if (value.StateChange == UniverseStateEvent.Eschaton) this._eschaton = true;

                this.ProcessCache();
            }
        }

        public IDisposable Subscribe(IObserver<IUniverseEvent> observer)
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (observer == null) return null;

            return this._baseService.Subscribe(observer);
        }

        /// <summary>
        ///     Alice: “How long is forever?"
        ///     White Rabbit: “Sometimes, just one second."
        /// </summary>
        private DateTime FilterTime()
        {
            if (this._ruleTimeWindows == null) return this._windowTime;

            return this._windowTime - this._ruleTimeWindows.BackwardWindowSize;
        }

        private void ProcessCache()
        {
            if (!this._universeCache.Any()) return;

            while (this._universeCache.Any()
                   && (this._universeCache.Peek().EventTime <= this.FilterTime() || this._eschaton))
            {
                var value = this._universeCache.Dequeue();

                if (value.StateChange.IsOrderType()
                    && !this._highVolumeVenueFilter.UniverseEventsPassedFilter.Contains(value.UnderlyingEvent))
                    continue;

                this._baseService.OnNext(value);
            }

            if (this._eschaton) this._highVolumeVenueFilter?.UniverseEventsPassedFilter.Clear();
        }
    }
}