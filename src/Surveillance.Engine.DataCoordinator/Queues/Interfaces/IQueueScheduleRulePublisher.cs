using System.Threading.Tasks;
using DomainV2.Scheduling;

namespace Surveillance.Engine.DataCoordinator.Queues.Interfaces
{
    public interface IQueueScheduleRulePublisher
    {
        Task Send(ScheduledExecution message);
    }
}
