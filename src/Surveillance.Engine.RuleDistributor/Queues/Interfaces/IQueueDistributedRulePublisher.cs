namespace Surveillance.Engine.RuleDistributor.Queues.Interfaces
{
    using System.Threading.Tasks;

    using Domain.Surveillance.Scheduling;

    /// <summary>
    /// The QueueDistributedRulePublisher interface.
    /// </summary>
    public interface IQueueDistributedRulePublisher
    {
        /// <summary>
        /// The schedule execution.
        /// </summary>
        /// <param name="distributedExecution">
        /// The distributed execution.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task ScheduleExecution(ScheduledExecution distributedExecution);
    }
}