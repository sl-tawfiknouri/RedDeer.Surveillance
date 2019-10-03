namespace Surveillance.Engine.Rules.Universe.Filter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Domain.Surveillance.Rules.Interfaces;
    using Domain.Surveillance.Scheduling;

    using Surveillance.Data.Universe;
    using Surveillance.Data.Universe.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters;
    using Surveillance.Engine.Rules.Universe.Filter.Interfaces;

    /// <summary>
    /// The high volume venue decorator filter.
    /// </summary>
    public class HighVolumeVenueDecoratorFilter : IHighVolumeVenueDecoratorFilter
    {
        /// <summary>
        /// The base service.
        /// </summary>
        private readonly IUniverseFilterService baseService;

        /// <summary>
        /// The high volume venue filter.
        /// </summary>
        private readonly IHighVolumeVenueFilter highVolumeVenueFilter;

        /// <summary>
        /// The lock.
        /// </summary>
        private readonly object @lock = new object();

        /// <summary>
        /// The rule time windows.
        /// </summary>
        private readonly TimeWindows ruleTimeWindows;

        /// <summary>
        /// The universe cache.
        /// </summary>
        private readonly Queue<IUniverseEvent> universeCache;

        /// <summary>
        /// The eschaton.
        /// </summary>
        private bool eschaton;

        /// <summary>
        /// The window time.
        /// </summary>
        private DateTime windowTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="HighVolumeVenueDecoratorFilter"/> class.
        /// </summary>
        /// <param name="timeWindows">
        /// The time windows.
        /// </param>
        /// <param name="baseService">
        /// The base service.
        /// </param>
        /// <param name="highVolumeVenueFilter">
        /// The high volume venue filter.
        /// </param>
        public HighVolumeVenueDecoratorFilter(
            TimeWindows timeWindows,
            IUniverseFilterService baseService,
            IHighVolumeVenueFilter highVolumeVenueFilter)
        {
            this.ruleTimeWindows = timeWindows ?? throw new ArgumentNullException(nameof(timeWindows));
            this.baseService = baseService ?? throw new ArgumentNullException(nameof(baseService));
            this.universeCache = new Queue<IUniverseEvent>();
            this.highVolumeVenueFilter = highVolumeVenueFilter;
        }

        /// <summary>
        /// The rule.
        /// </summary>
        public Rules Rule => this.baseService.Rule;

        /// <summary>
        /// The version.
        /// </summary>
        public string Version => this.baseService.Version;

        /// <summary>
        /// The data constraints.
        /// </summary>
        /// <returns>
        /// The <see cref="IRuleDataConstraint"/>.
        /// </returns>
        public IRuleDataConstraint DataConstraints()
        {
            return this.baseService.DataConstraints();
        }

        /// <summary>
        /// The on completed.
        /// </summary>
        public void OnCompleted()
        {
            this.baseService.OnCompleted();
        }

        /// <summary>
        /// The on error.
        /// </summary>
        /// <param name="error">
        /// The error.
        /// </param>
        public void OnError(Exception error)
        {
            this.baseService.OnError(error);
        }

        /// <summary>
        /// The on next.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public void OnNext(IUniverseEvent value)
        {
            if (value == null) return;

            lock (this.@lock)
            {
                this.highVolumeVenueFilter.OnNext(value);

                this.universeCache.Enqueue(value);
                this.windowTime = value.EventTime;

                if (value.StateChange == UniverseStateEvent.Eschaton) this.eschaton = true;

                this.ProcessCache();
            }
        }

        /// <summary>
        /// The subscribe.
        /// </summary>
        /// <param name="observer">
        /// The observer.
        /// </param>
        /// <returns>
        /// The <see cref="IDisposable"/>.
        /// </returns>
        public IDisposable Subscribe(IObserver<IUniverseEvent> observer)
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (observer == null) return null;

            return this.baseService.Subscribe(observer);
        }

        /// <summary>
        /// Alice: “How long is forever?"
        ///     White Rabbit: “Sometimes, just one second."
        /// </summary>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        private DateTime FilterTime()
        {
            if (this.ruleTimeWindows == null) return this.windowTime;

            return this.windowTime - this.ruleTimeWindows.BackwardWindowSize;
        }

        /// <summary>
        /// The process cache.
        /// </summary>
        private void ProcessCache()
        {
            if (!this.universeCache.Any()) return;

            while (this.universeCache.Any()
                   && (this.universeCache.Peek().EventTime <= this.FilterTime() || this.eschaton))
            {
                var value = this.universeCache.Dequeue();

                if (value.StateChange.IsOrderType()
                    && !this.highVolumeVenueFilter.UniverseEventsPassedFilter.Contains(value.UnderlyingEvent))
                    continue;

                this.baseService.OnNext(value);
            }

            if (this.eschaton) this.highVolumeVenueFilter?.UniverseEventsPassedFilter.Clear();
        }
    }
}