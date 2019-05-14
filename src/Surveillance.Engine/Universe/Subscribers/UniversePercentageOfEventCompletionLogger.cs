using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Surveillance.Engine.Rules.Universe.Interfaces;
using Surveillance.Engine.Rules.Universe.Subscribers.Interfaces;

namespace Surveillance.Engine.Rules.Universe.Subscribers
{
    /// <summary>
    /// This logger is a bad mix with a lazy transient universe
    /// Beware before using it
    /// </summary>
    public class UniversePercentageOfEventCompletionLogger : IUniversePercentageOfEventCompletionLogger
    {
        private readonly IList<PercentageLandMark> _list;
        private readonly ILogger<UniversePercentageOfEventCompletionLogger> _logger;

        public UniversePercentageOfEventCompletionLogger(
            ILogger<UniversePercentageOfEventCompletionLogger> logger)
        {
            _list = new List<PercentageLandMark>();
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void InitiateEventLogger(IUniverse universe)
        {
            if (universe == null)
            {
                return;
            }

            var events = (decimal)universe.UniverseEvents.Count();

            if (events < 10m)
            {
                _logger.LogInformation($"Less than 10 events in the universe. Not logging (%) completion.");
                return;
            }

            for (decimal i = 1; i <= 10m; i++)
            {
                var fraction = 0.1m * i;
                var percentageEventIndex = (int)Math.Floor(events * fraction);
                var universeEvent = universe.UniverseEvents.ElementAt(percentageEventIndex - 1);
                var landMark = new PercentageLandMark(universeEvent, $"Universe event processing {fraction * 100}% towards completion. {universeEvent.EventTime}");
                _list.Add(landMark);
            }
        }

        public void OnCompleted()
        { }

        public void OnError(Exception error)
        {
            _logger.LogError($"Exception passed to Universe Percentage Of Event Completion Logger {error.Message} - {error.InnerException?.Message}");
        }

        public void OnNext(IUniverseEvent value)
        {
            if (value == null)
            {
                return;
            }

            if (!_list.Any())
            {
                return;
            }

            var match = _list.FirstOrDefault(m => ReferenceEquals(m.Events, value));
            if (match != null)
            {
                _logger.LogInformation(match?.LogMessage);
                _list.Remove(match);
            }
        }

        public class PercentageLandMark
        {
            public PercentageLandMark(IUniverseEvent events, string logMessage)
            {
                Events = events;
                LogMessage = logMessage ?? string.Empty;
            }

            public IUniverseEvent Events { get; }
            public string LogMessage { get; }
        }
    }
}
