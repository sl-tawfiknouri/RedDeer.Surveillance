using Surveillance.Rules.Cancelled_Orders.Interfaces;
using Surveillance.Rule_Parameters.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.Factories.Interfaces
{
    public interface ICancelledOrderRuleFactory
    {
        ICancelledOrderRule Build(ICancelledOrderRuleParameters parameters, ISystemProcessOperationRunRuleContext ruleCtx);
        string Version { get; }
    }
}