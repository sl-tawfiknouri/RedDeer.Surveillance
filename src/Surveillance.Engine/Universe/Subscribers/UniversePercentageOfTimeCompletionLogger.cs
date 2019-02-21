using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Scheduling;
using Microsoft.Extensions.Logging;
using Surveillance.Engine.Rules.Universe.Interfaces;
using Surveillance.Engine.Rules.Universe.Subscribers.Interfaces;

namespace Surveillance.Engine.Rules.Universe.Subscribers
{
    public class UniversePercentageOfTimeCompletionLogger : IUniversePercentageOfTimeCompletionLogger
    {
        private readonly Stack<TimeLandMark> _landmarks;
        private readonly ILogger<UniversePercentageOfTimeCompletionLogger> _logger;

        public UniversePercentageOfTimeCompletionLogger(ILogger<UniversePercentageOfTimeCompletionLogger> logger)
        {
            _landmarks = new Stack<TimeLandMark>();
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void InitiateTimeLogger(ScheduledExecution execution)
        {
            if (execution == null)
            {
                return;
            }

            var timeSpanOfRule = execution.TimeSeriesTermination.Subtract(execution.TimeSeriesInitiation);

            var ruleMinutes = timeSpanOfRule.TotalMinutes;

            if (ruleMinutes <= 10)
            {
                _logger.LogInformation("UniversePercentageOfTimeCompletionLogger detected scheduled rule run with less than 10 minutes of universe time to expire. Will not be reporting on time percentage completion.");
                return;
            }

            var tempStack = new Stack<TimeLandMark>();

            for (double i = 1; i <= 10; i++)
            {
                var fraction = 0.1 * i;
                var additionalMinutes = (double)Math.Floor(ruleMinutes * fraction);
                var landmarkTime = execution.TimeSeriesInitiation.AddMinutes(additionalMinutes);               
                var landMark = new TimeLandMark(landmarkTime, $"Universe time line {fraction * 100}% towards completion at {landmarkTime.DateTime}");
                tempStack.Push(landMark);
            }

            while (tempStack.Any())
                _landmarks.Push(tempStack.Pop());
        }

        public void OnCompleted()
        { }

        public void OnError(Exception error)
        {
            _logger.LogError($"Exception passed to UniversePercentageOfTimeCompletionLogger {error.Message} - {error?.InnerException?.Message}");
        }

        public void OnNext(IUniverseEvent value)
        {
            if (!_landmarks.Any())
                return;

            var topOfStack = _landmarks.Peek();

            if (value.EventTime >= topOfStack.LandMark)
            {
                _logger.LogInformation(topOfStack.LogMessage);
                _landmarks.Pop();
            }
        }

        public class TimeLandMark
        {
            public TimeLandMark(DateTimeOffset timeLandMark, string logMessage)
            {
                LandMark = timeLandMark;
                LogMessage = logMessage ?? string.Empty;
            }

            public DateTimeOffset LandMark { get; }
            public string LogMessage { get; }
        }
    }
}
