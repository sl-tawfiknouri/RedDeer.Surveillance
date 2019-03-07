using System.Threading.Tasks;
using Domain.Surveillance.Scheduling;

namespace Surveillance.Engine.DataCoordinator.Queues.Interfaces
{
    public interface IQueueScheduleRulePublisher
    {
        Task Send(ScheduledExecution message);
    }
}
