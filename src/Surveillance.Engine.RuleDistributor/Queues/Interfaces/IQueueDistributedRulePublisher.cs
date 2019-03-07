using System.Threading.Tasks;
using Domain.Surveillance.Scheduling;

namespace Surveillance.Engine.RuleDistributor.Queues.Interfaces
{
    public interface IQueueDistributedRulePublisher
    {
        Task ScheduleExecution(ScheduledExecution distributedExecution);
    }
}