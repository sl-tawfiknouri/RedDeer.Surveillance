namespace Surveillance.Engine.Rules.Universe.Filter.Interfaces
{
    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.Filter;
    using Surveillance.Engine.Rules.Rules;

    public interface IHighMarketCapFilterFactory
    {
        IHighMarketCapFilter Build(
            RuleRunMode ruleRunMode,
            DecimalRangeRuleFilter marketCap,
            string ruleName,
            IUniverseDataRequestsSubscriber universeDataRequestsSubscriber,
            ISystemProcessOperationRunRuleContext operationRunRuleContext);
    }
}