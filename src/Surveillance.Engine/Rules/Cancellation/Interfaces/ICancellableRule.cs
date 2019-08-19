namespace Surveillance.Engine.Rules.Rules.Cancellation.Interfaces
{
    using System.Threading;

    using Domain.Surveillance.Scheduling;

    public interface ICancellableRule
    {
        CancellationTokenSource CancellationToken { get; }

        ScheduledExecution Schedule { get; }
    }
}