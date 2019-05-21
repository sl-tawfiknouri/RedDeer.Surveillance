using System.Threading;
using Domain.Surveillance.Scheduling;

namespace Surveillance.Engine.Rules.Rules.Cancellation.Interfaces
{
    public interface ICancellableRule
    {
        CancellationTokenSource CancellationToken { get; }
        ScheduledExecution Schedule { get; }
    }
}