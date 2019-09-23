namespace Surveillance.Engine.RuleDistributor.Queues.Interfaces
{
    using System.Threading.Tasks;

    using Domain.Surveillance.Scheduling;

    public interface IQueueDistributedRulePublisher
    {
        Task ScheduleExecution(ScheduledExecution distributedExecution);
    }
}