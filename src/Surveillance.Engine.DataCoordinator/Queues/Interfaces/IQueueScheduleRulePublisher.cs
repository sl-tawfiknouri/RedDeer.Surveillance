namespace Surveillance.Engine.DataCoordinator.Queues.Interfaces
{
    using System.Threading.Tasks;

    using Domain.Surveillance.Scheduling;

    /// <summary>
    /// The QueueScheduleRulePublisher interface.
    /// </summary>
    public interface IQueueScheduleRulePublisher
    {
        /// <summary>
        /// The send.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task Send(ScheduledExecution message);
    }
}