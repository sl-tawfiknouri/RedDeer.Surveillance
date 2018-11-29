using System.Threading.Tasks;
using Domain.Scheduling;

namespace Surveillance.MessageBusIO.Interfaces
{
    public interface IScheduleRuleMessageSender
    {
        Task Send(ScheduledExecution message);
    }
}