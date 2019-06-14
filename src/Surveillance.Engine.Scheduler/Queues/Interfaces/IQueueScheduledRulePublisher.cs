using System.Threading.Tasks;
using Domain.Surveillance.Scheduling;

namespace Surveillance.Engine.Scheduler.Queues.Interfaces
{
    public interface IQueueScheduledRulePublisher
    {
        Task Publish(AdHocScheduleRequest message);
    }
}