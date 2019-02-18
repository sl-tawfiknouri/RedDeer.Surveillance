using System.Threading.Tasks;
using DomainV2.Scheduling;

namespace Surveillance.Engine.DataCoordinator.Queues.Interfaces
{
    public interface IScheduleRuleMessageSender
    {
        Task Send(ScheduledExecution message);
    }
}
