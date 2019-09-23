namespace Surveillance.Engine.Rules.Factories.Equities.Interfaces
{
    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
    using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
    using Surveillance.Engine.Rules.Rules;
    using Surveillance.Engine.Rules.Rules.Equity.Ramping.Interfaces;

    public interface IEquityRuleRampingFactory
    {
        IRampingRule Build(
            IRampingRuleEquitiesParameters equitiesParameters,
            ISystemProcessOperationRunRuleContext ruleCtx,
            IUniverseAlertStream alertStream,
            RuleRunMode runMode,
            IUniverseDataRequestsSubscriber dataRequestSubscriber);
    }
}