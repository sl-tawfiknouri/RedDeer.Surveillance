using Surveillance.Engine.Rules.Rules.Cancellation.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Cancellation
{
    public interface IRuleCancellation
    {
        void Cancel(string cancelledRuleId);
        void Subscribe(ICancellableRule cancellableRule);
        void Unsubscribe(ICancellableRule cancellableRule);
    }
}