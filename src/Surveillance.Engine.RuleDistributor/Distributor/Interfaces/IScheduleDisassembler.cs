using System.Threading.Tasks;
using DomainV2.Scheduling;
using Surveillance.Systems.Auditing.Context.Interfaces;

namespace Surveillance.Engine.RuleDistributor.Distributor.Interfaces
{
    public interface IScheduleDisassembler
    {
        Task Disassemble(ISystemProcessOperationContext opCtx, ScheduledExecution execution, string messageId, string messageBody);
    }
}