namespace Surveillance.Engine.Rules.Rules.Cancellation
{
    using System;
    using System.Threading;

    using Domain.Surveillance.Scheduling;

    using Surveillance.Engine.Rules.Rules.Cancellation.Interfaces;

    public class CancellableRule : ICancellableRule
    {
        public CancellableRule(ScheduledExecution schedule, CancellationTokenSource cancellationToken)
        {
            this.Schedule = schedule ?? throw new ArgumentNullException(nameof(schedule));
            this.CancellationToken = cancellationToken;
        }

        public CancellationTokenSource CancellationToken { get; }

        public ScheduledExecution Schedule { get; }
    }
}