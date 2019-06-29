using System;
using System.Threading.Tasks;
using Domain.Surveillance.Scheduling;
using Surveillance.Auditing.Context.Interfaces;

namespace Surveillance.Engine.Rules.Universe.Interfaces
{
    public interface IUniverseBuilder
    {
        Task<IUniverse> Summon(ScheduledExecution execution, ISystemProcessOperationContext opCtx);
        Task<IUniverse> Summon(ScheduledExecution execution, ISystemProcessOperationContext opCtx, bool includeGenesis, bool includeEschaton, DateTimeOffset? realUniverseEpoch, DateTimeOffset? futureUniverseEpoch);
    }
}