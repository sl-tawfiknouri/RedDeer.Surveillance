namespace Surveillance.Engine.Rules.Rules.Cancellation.Interfaces
{
    public interface IRuleCancellation
    {
        void Cancel(string cancelledRuleId);

        void Subscribe(ICancellableRule cancellableRule);

        void Unsubscribe(ICancellableRule cancellableRule);
    }
}