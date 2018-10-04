using Surveillance.Rules.High_Profits.Interfaces;
using Surveillance.Rule_Parameters.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.Factories.Interfaces
{
    public interface IHighProfitRuleFactory
    {
        IHighProfitRule Build(IHighProfitsRuleParameters parameters, ISystemProcessOperationRunRuleContext ruleCtx);
        string RuleVersion { get; }
    }
}