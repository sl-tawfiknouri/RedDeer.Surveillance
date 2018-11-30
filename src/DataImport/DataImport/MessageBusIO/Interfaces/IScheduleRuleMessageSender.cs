using System.Threading.Tasks;
using Domain.Scheduling;

namespace DataImport.MessageBusIO.Interfaces
{
    public interface IScheduleRuleMessageSender
    {
        Task Send(ScheduledExecution message);
    }
}