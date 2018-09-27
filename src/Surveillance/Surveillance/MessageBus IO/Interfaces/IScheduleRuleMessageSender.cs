using System.Threading.Tasks;
using Domain.Scheduling;

namespace Surveillance.MessageBus_IO.Interfaces
{
    public interface IScheduleRuleMessageSender
    {
        Task Send(ScheduledExecution message);
    }
}