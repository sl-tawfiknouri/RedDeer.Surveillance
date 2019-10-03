namespace Surveillance.Engine.Rules.Universe.Subscribers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.Extensions.Logging;

    using Surveillance.Data.Universe.Interfaces;
    using Surveillance.Engine.Rules.Universe.Subscribers.Interfaces;

    /// <summary>
    ///     This logger is a bad mix with a lazy transient universe
    ///     Beware before using it
    /// </summary>
    public class UniversePercentageOfEventCompletionLogger : IUniversePercentageOfEventCompletionLogger
    {
        /// <summary>
        /// The list.
        /// </summary>
        private readonly IList<PercentageLandMark> list;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<UniversePercentageOfEventCompletionLogger> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="UniversePercentageOfEventCompletionLogger"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public UniversePercentageOfEventCompletionLogger(ILogger<UniversePercentageOfEventCompletionLogger> logger)
        {
            this.list = new List<PercentageLandMark>();
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// The initiate event logger.
        /// </summary>
        /// <param name="universe">
        /// The universe.
        /// </param>
        public void InitiateEventLogger(IUniverse universe)
        {
            if (universe == null)
            {
                return;
            }

            var events = (decimal)universe.UniverseEvents.Count();

            if (events < 10m)
            {
                this.logger.LogInformation("Less than 10 events in the universe. Not logging (%) completion.");
                return;
            }

            for (decimal i = 1; i <= 10m; i++)
            {
                var fraction = 0.1m * i;
                var percentageEventIndex = (int)Math.Floor(events * fraction);
                var universeEvent = universe.UniverseEvents.ElementAt(percentageEventIndex - 1);
                var landMark = new PercentageLandMark(
                    universeEvent,
                    $"Universe event processing {fraction * 100}% towards completion. {universeEvent.EventTime}");
                this.list.Add(landMark);
            }
        }

        /// <summary>
        /// The on completed.
        /// </summary>
        public void OnCompleted()
        {
        }

        /// <summary>
        /// The on error.
        /// </summary>
        /// <param name="error">
        /// The error.
        /// </param>
        public void OnError(Exception error)
        {
            this.logger.LogError(
                $"Exception passed to Universe Percentage Of Event Completion Logger {error.Message} - {error.InnerException?.Message}");
        }

        /// <summary>
        /// The on next.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public void OnNext(IUniverseEvent value)
        {
            if (value == null)
            {
                return;
            }

            if (!this.list.Any())
            {
                return;
            }

            var match = this.list.FirstOrDefault(m => ReferenceEquals(m.Events, value));

            if (match == null)
            {
                return;
            }

            this.logger.LogInformation(match?.LogMessage);
            this.list.Remove(match);
        }

        /// <summary>
        /// The percentage land mark.
        /// </summary>
        public class PercentageLandMark
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="PercentageLandMark"/> class.
            /// </summary>
            /// <param name="events">
            /// The events.
            /// </param>
            /// <param name="logMessage">
            /// The log message.
            /// </param>
            public PercentageLandMark(IUniverseEvent events, string logMessage)
            {
                this.Events = events;
                this.LogMessage = logMessage ?? string.Empty;
            }

            /// <summary>
            /// Gets the events.
            /// </summary>
            public IUniverseEvent Events { get; }

            /// <summary>
            /// Gets the log message.
            /// </summary>
            public string LogMessage { get; }
        }
    }
}