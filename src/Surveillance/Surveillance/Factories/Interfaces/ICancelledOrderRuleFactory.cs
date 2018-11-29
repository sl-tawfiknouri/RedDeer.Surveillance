using Surveillance.Analytics.Streams.Interfaces;
using Surveillance.RuleParameters.Interfaces;
using Surveillance.Rules.CancelledOrders.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.Factories.Interfaces
{
    public interface ICancelledOrderRuleFactory
    {
        ICancelledOrderRule Build(
            ICancelledOrderRuleParameters parameters,
            ISystemProcessOperationRunRuleContext ruleCtx,
            IUniverseAlertStream alertStream);

    }
}