namespace Surveillance.Engine.Scheduler.Scheduler.Interfaces
{
    using System.Threading.Tasks;

    public interface IDelayedScheduler
    {
        Task ScheduleDueTasks();
    }
}