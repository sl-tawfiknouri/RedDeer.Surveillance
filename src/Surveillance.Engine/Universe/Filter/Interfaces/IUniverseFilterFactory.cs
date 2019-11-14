namespace Surveillance.Engine.Rules.Universe.Filter.Interfaces
{
    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.Filter;
    using Surveillance.Engine.Rules.Rules;
    using Surveillance.Engine.Rules.Rules.Interfaces;

    public interface IUniverseFilterFactory
    {
        IUniverseFilterService Build(
            IUniverseRule filteredRule,
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