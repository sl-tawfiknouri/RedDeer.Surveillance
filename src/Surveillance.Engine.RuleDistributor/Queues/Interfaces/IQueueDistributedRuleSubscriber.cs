namespace Surveillance.Engine.RuleDistributor.Queues.Interfaces
{
    /// <summary>
    /// The QueueDistributedRuleSubscriber interface.
    /// </summary>
    public interface IQueueDistributedRuleSubscriber
    {
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