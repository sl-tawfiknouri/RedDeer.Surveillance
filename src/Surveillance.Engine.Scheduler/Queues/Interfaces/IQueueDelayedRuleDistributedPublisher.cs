namespace Surveillance.Engine.Scheduler.Queues.Interfaces
{
    using System.Threading.Tasks;

    using Domain.Surveillance.Scheduling;

    /// <summary>
    /// The QueueDelayedRuleDistributedPublisher interface.
    /// </summary>
    public interface IQueueDelayedRuleDistributedPublisher
    {
        /// <summary>
        /// The publish.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task Publish(AdHocScheduleRequest request);
    }
}