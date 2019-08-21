namespace Surveillance.Engine.Scheduler.Queues.Interfaces
{
    using System.Threading.Tasks;

    using Domain.Surveillance.Scheduling;

    public interface IQueueScheduledRulePublisher
    {
        Task Publish(AdHocScheduleRequest message);
    }
}