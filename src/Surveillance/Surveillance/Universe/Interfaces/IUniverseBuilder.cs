using Surveillance.Scheduler;

namespace Surveillance.Universe.Interfaces
{
    public interface IUniverseBuilder
    {
        IUniverse Summon(ScheduledExecution execution);
    }
}