using System.Threading.Tasks;
using DomainV2.Scheduling;

namespace Surveillance.MessageBusIO.Interfaces
{
    public interface IScheduleRuleMessageSender
    {
        Task Send(ScheduledExecution message);
    }
}