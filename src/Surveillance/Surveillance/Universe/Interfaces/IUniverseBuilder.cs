using System.Threading.Tasks;
using Surveillance.Scheduler;

namespace Surveillance.Universe.Interfaces
{
    public interface IUniverseBuilder
    {
        Task<IUniverse> Summon(ScheduledExecution execution);
    }
}