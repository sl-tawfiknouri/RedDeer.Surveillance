using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Filter;
using Surveillance.Engine.Rules.Rules;

namespace Surveillance.Engine.Rules.Universe.Filter.Interfaces
{
    public interface IHighMarketCapFilterFactory
    {
        IHighMarketCapFilter Build(RuleRunMode ruleRunMode, DecimalRangeRuleFilter marketCap, string ruleName, ISystemProcessOperationRunRuleContext operationRunRuleContext);
    }
}
