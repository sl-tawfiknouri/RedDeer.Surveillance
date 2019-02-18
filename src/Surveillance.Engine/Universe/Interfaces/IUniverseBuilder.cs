using System.Threading.Tasks;
using DomainV2.Scheduling;
using Surveillance.Auditing.Context.Interfaces;

namespace Surveillance.Engine.Rules.Universe.Interfaces
{
    public interface IUniverseBuilder
    {
        Task<IUniverse> Summon(ScheduledExecution execution, ISystemProcessOperationContext opCtx);
    }
}