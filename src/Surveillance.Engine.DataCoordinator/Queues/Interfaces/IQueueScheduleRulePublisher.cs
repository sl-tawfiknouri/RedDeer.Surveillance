using System.Threading.Tasks;
using Domain.Scheduling;

namespace Surveillance.Engine.DataCoordinator.Queues.Interfaces
{
    public interface IQueueScheduleRulePublisher
    {
        Task Send(ScheduledExecution message);
    }
}
