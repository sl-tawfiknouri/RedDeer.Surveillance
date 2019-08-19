namespace Surveillance.Engine.DataCoordinator.Queues.Interfaces
{
    using System.Threading.Tasks;

    using Domain.Surveillance.Scheduling;

    public interface IQueueScheduleRulePublisher
    {
        Task Send(ScheduledExecution message);
    }
}