using Domain.Surveillance.Scheduling;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Universe.Interfaces;

namespace Surveillance.Engine.Rules.Universe.Lazy.Interfaces
{
    public interface ILazyTransientUniverseFactory
    {
        IUniverse Build(ScheduledExecution execution, ISystemProcessOperationContext opCtx);
    }
}