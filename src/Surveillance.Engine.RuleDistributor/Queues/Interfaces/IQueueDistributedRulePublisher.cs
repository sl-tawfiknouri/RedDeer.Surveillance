using System.Threading.Tasks;
using DomainV2.Scheduling;

namespace Surveillance.Engine.RuleDistributor.Queues.Interfaces
{
    public interface IQueueDistributedRulePublisher
    {
        Task ScheduleExecution(ScheduledExecution distributedExecution);
    }
}