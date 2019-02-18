using System.Threading.Tasks;
using Domain.Scheduling;
using Surveillance.Auditing.Context.Interfaces;

namespace Surveillance.Engine.RuleDistributor.Distributor.Interfaces
{
    public interface IScheduleDisassembler
    {
        Task Disassemble(ISystemProcessOperationContext opCtx, ScheduledExecution execution, string messageId, string messageBody);
    }
}