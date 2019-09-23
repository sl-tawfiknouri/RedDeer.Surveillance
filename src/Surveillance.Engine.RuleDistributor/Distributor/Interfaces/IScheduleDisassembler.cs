namespace Surveillance.Engine.RuleDistributor.Distributor.Interfaces
{
    using System.Threading.Tasks;

    using Domain.Surveillance.Scheduling;

    using Surveillance.Auditing.Context.Interfaces;

    public interface IScheduleDisassembler
    {
        Task Disassemble(
            ISystemProcessOperationContext opCtx,
            ScheduledExecution execution,
            string messageId,
            string messageBody);
    }
}