namespace Surveillance.Engine.Scheduler.Queues.Interfaces
{
    using System.Threading.Tasks;

    using Domain.Surveillance.Scheduling;

    public interface IQueueDelayedRuleDistributedPublisher
    {
        Task Publish(AdHocScheduleRequest request);
    }
}