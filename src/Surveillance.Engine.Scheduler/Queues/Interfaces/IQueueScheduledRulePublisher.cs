namespace Surveillance.Engine.Scheduler.Queues.Interfaces
{
    using System.Threading.Tasks;

    using Domain.Surveillance.Scheduling;

    /// <summary>
    /// The QueueScheduledRulePublisher interface.
    /// </summary>
    public interface IQueueScheduledRulePublisher
    {
        /// <summary>
        /// The publish.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task Publish(AdHocScheduleRequest message);
    }
}