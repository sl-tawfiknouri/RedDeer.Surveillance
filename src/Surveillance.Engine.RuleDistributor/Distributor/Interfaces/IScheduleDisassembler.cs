namespace Surveillance.Engine.RuleDistributor.Distributor.Interfaces
{
    using System.Threading.Tasks;

    using Domain.Surveillance.Scheduling;

    using Surveillance.Auditing.Context.Interfaces;

    /// <summary>
    /// The ScheduleDisassembler interface.
    /// </summary>
    public interface IScheduleDisassembler
    {
        /// <summary>
        /// The disassemble.
        /// </summary>
        /// <param name="operationContext">
        /// The operation context.
        /// </param>
        /// <param name="execution">
        /// The execution.
        /// </param>
        /// <param name="messageId">
        /// The message id.
        /// </param>
        /// <param name="messageBody">
        /// The message body.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task Disassemble(
            ISystemProcessOperationContext operationContext,
            ScheduledExecution execution,
            string messageId,
            string messageBody);
    }
}