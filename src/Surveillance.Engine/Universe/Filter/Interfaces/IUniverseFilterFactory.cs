using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Filter;
using Surveillance.Engine.Rules.Rules;

namespace Surveillance.Engine.Rules.Universe.Filter.Interfaces
{
    public interface IUniverseFilterFactory
    {
        IUniverseFilterService Build(
            RuleFilter accounts,
            RuleFilter traders,
            RuleFilter markets,
            RuleFilter funds,
            RuleFilter strategies,
            RuleFilter sectors,
            RuleFilter industries,
            RuleFilter regions,
            RuleFilter countries,
            DecimalRangeRuleFilter marketCap,
            RuleRunMode ruleRunMode,
            string ruleName,
            IUniverseDataRequestsSubscriber universeDataRequestsSubscriber,
            ISystemProcessOperationRunRuleContext operationRunRuleContext);
    }
}