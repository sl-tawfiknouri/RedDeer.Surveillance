namespace Surveillance.Engine.Rules.Rules.Cancellation
{
    using System;
    using System.Threading;

    using Domain.Surveillance.Scheduling;

    using Surveillance.Engine.Rules.Rules.Cancellation.Interfaces;

    /// <summary>
    /// The cancellable rule.
    /// </summary>
    public class CancellableRule : ICancellableRule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CancellableRule"/> class.
        /// </summary>
        /// <param name="schedule">
        /// The schedule.
        /// </param>
        /// <param name="cancellationToken">
        /// The cancellation token.
        /// </param>
        public CancellableRule(ScheduledExecution schedule, CancellationTokenSource cancellationToken)
        {
            this.Schedule = schedule ?? throw new ArgumentNullException(nameof(schedule));
            this.CancellationToken = cancellationToken;
        }

        /// <summary>
        /// Gets the cancellation token to invoke when cancelling a rule run.
        /// </summary>
        public CancellationTokenSource CancellationToken { get; }

        /// <summary>
        /// Gets the schedule for the rule run.
        /// </summary>
        public ScheduledExecution Schedule { get; }
    }
}