namespace Surveillance.Engine.Rules.Universe.Subscribers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Domain.Surveillance.Scheduling;

    using Microsoft.Extensions.Logging;

    using Surveillance.Data.Universe.Interfaces;
    using Surveillance.Engine.Rules.Universe.Subscribers.Interfaces;

    /// <summary>
    /// The universe percentage of time completion logger.
    /// </summary>
    public class UniversePercentageOfTimeCompletionLogger : IUniversePercentageOfTimeCompletionLogger
    {
        /// <summary>
        /// The landmarks.
        /// </summary>
        private readonly Stack<TimeLandMark> landmarks;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<UniversePercentageOfTimeCompletionLogger> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="UniversePercentageOfTimeCompletionLogger"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public UniversePercentageOfTimeCompletionLogger(ILogger<UniversePercentageOfTimeCompletionLogger> logger)
        {
            this.landmarks = new Stack<TimeLandMark>();
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// The initiate time logger.
        /// </summary>
        /// <param name="execution">
        /// The execution.
        /// </param>
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
                this.logger.LogInformation(
                    "UniversePercentageOfTimeCompletionLogger detected scheduled rule run with less than 10 minutes of universe time to expire. Will not be reporting on time percentage completion.");
                return;
            }

            var tempStack = new Stack<TimeLandMark>();

            for (double i = 1; i <= 10; i++)
            {
                var fraction = 0.1 * i;
                var additionalMinutes = Math.Floor(ruleMinutes * fraction);
                var landmarkTime = execution.TimeSeriesInitiation.AddMinutes(additionalMinutes);
                var landMark = new TimeLandMark(
                    landmarkTime,
                    $"Universe time line {fraction * 100}% towards completion at {landmarkTime.DateTime}");
                tempStack.Push(landMark);
            }

            while (tempStack.Any())
            {
                this.landmarks.Push(tempStack.Pop());
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
                $"Exception passed to UniversePercentageOfTimeCompletionLogger {error.Message} - {error?.InnerException?.Message}");
        }

        /// <summary>
        /// The on next.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public void OnNext(IUniverseEvent value)
        {
            if (!this.landmarks.Any())
            {
                return;
            }

            var topOfStack = this.landmarks.Peek();

            if (value.EventTime >= topOfStack.LandMark)
            {
                this.logger.LogInformation(topOfStack.LogMessage);
                this.landmarks.Pop();
            }
        }

        /// <summary>
        /// The time land mark.
        /// </summary>
        public class TimeLandMark
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="TimeLandMark"/> class.
            /// </summary>
            /// <param name="timeLandMark">
            /// The time land mark.
            /// </param>
            /// <param name="logMessage">
            /// The log message.
            /// </param>
            public TimeLandMark(DateTimeOffset timeLandMark, string logMessage)
            {
                this.LandMark = timeLandMark;
                this.LogMessage = logMessage ?? string.Empty;
            }

            /// <summary>
            /// Gets the land mark.
            /// </summary>
            public DateTimeOffset LandMark { get; }

            /// <summary>
            /// Gets the log message.
            /// </summary>
            public string LogMessage { get; }
        }
    }
}