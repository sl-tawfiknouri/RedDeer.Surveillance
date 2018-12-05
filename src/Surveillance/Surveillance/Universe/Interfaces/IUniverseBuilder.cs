using System.Threading.Tasks;
using DomainV2.Scheduling;
using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.Universe.Interfaces
{
    public interface IUniverseBuilder
    {
        Task<IUniverse> Summon(ScheduledExecution execution, ISystemProcessOperationContext opCtx);
    }
}