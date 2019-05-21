using System;
using System.Threading;
using Domain.Surveillance.Scheduling;
using Surveillance.Engine.Rules.Rules.Cancellation.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Cancellation
{
    public class CancellableRule : ICancellableRule
    {
        public CancellableRule(
            ScheduledExecution schedule,
            CancellationTokenSource cancellationToken)
        {
            Schedule = schedule ?? throw new ArgumentNullException(nameof(schedule));
            CancellationToken = cancellationToken;
        }

        public ScheduledExecution Schedule { get; }
        public CancellationTokenSource CancellationToken { get; }
    }
}
