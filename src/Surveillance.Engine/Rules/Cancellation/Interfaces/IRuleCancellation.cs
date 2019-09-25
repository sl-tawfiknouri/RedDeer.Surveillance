namespace Surveillance.Engine.Rules.Rules.Cancellation.Interfaces
{
    /// <summary>
    /// The RuleCancellation interface.
    /// </summary>
    public interface IRuleCancellation
    {
        /// <summary>
        /// The cancel.
        /// </summary>
        /// <param name="cancelledRuleId">
        /// The cancelled rule id.
        /// </param>
        void Cancel(string cancelledRuleId);

        /// <summary>
        /// The subscribe.
        /// </summary>
        /// <param name="cancellableRule">
        /// The cancellable rule.
        /// </param>
        void Subscribe(ICancellableRule cancellableRule);

        /// <summary>
        /// The unsubscribe.
        /// </summary>
        /// <param name="cancellableRule">
        /// The cancellable rule.
        /// </param>
        void Unsubscribe(ICancellableRule cancellableRule);
    }
}