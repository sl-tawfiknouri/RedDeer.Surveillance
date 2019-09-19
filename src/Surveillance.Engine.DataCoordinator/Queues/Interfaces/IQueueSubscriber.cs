namespace Surveillance.Engine.DataCoordinator.Queues.Interfaces
{
    using System.Threading.Tasks;

    /// <summary>
    /// The QueueSubscriber interface.
    /// </summary>
    public interface IQueueSubscriber
    {
        /// <summary>
        /// The execute coordination message.
        /// </summary>
        /// <param name="messageId">
        /// The message id.
        /// </param>
        /// <param name="messageBody">
        /// The message body.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task ExecuteCoordinationMessageAsync(string messageId, string messageBody);

        /// <summary>
        /// The initiate.
        /// </summary>
        void Initiate();

        /// <summary>
        /// The terminate.
        /// </summary>
        void Terminate();
    }
}