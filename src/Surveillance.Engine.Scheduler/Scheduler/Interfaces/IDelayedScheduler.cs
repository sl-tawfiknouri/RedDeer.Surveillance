using Domain.Surveillance.Scheduling;

namespace Surveillance.Engine.Scheduler.Scheduler.Interfaces
{
    public interface IDelayedScheduler
    {
        void ScheduleDueTasks();
        void Save(AdHocScheduleRequest request);
    }
}