namespace Surveillance.Engine.Rules.Factories.Equities.Interfaces
{
    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
    using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
    using Surveillance.Engine.Rules.Rules;
    using Surveillance.Engine.Rules.Rules.Equity.MarkingTheClose.Interfaces;

    public interface IEquityRuleMarkingTheCloseFactory
    {
        IMarkingTheCloseRule Build(
            IMarkingTheCloseEquitiesParameters equitiesParameters,
            ISystemProcessOperationRunRuleContext ruleCtx,
            IUniverseAlertStream alertStream,
            RuleRunMode runMode,
            IUniverseDataRequestsSubscriber dataRequestSubscriber);
    }
}