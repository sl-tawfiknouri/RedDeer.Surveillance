using System.Threading.Tasks;
using Domain.Scheduling;

namespace Surveillance.Universe.Interfaces
{
    public interface IUniverseBuilder
    {
        Task<IUniverse> Summon(ScheduledExecution execution);
    }
}