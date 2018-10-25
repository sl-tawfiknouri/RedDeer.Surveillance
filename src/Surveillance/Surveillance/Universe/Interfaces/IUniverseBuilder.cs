using System.Threading.Tasks;
using Domain.Scheduling;
using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.Universe.Interfaces
{
    public interface IUniverseBuilder
    {
        Task<IUniverse> Summon(ScheduledExecution execution, ISystemProcessOperationContext opCtx);
    }
}