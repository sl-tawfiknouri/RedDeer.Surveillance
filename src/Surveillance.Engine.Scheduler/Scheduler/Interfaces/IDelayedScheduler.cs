using System.Threading.Tasks;

namespace Surveillance.Engine.Scheduler.Scheduler.Interfaces
{
    public interface IDelayedScheduler
    {
        Task ScheduleDueTasks();
    }
}