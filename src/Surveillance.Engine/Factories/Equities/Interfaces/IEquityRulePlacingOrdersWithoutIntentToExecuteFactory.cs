namespace Surveillance.Engine.Rules.Factories.Equities.Interfaces
{
    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
    using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
    using Surveillance.Engine.Rules.Rules;
    using Surveillance.Engine.Rules.Rules.Equity.PlacingOrderNoIntentToExecute.Interfaces;

    public interface IEquityRulePlacingOrdersWithoutIntentToExecuteFactory
    {
        IPlacingOrdersWithNoIntentToExecuteRule Build(
            IPlacingOrderWithNoIntentToExecuteRuleEquitiesParameters parameters,
            IUniverseAlertStream alertStream,
            ISystemProcessOperationRunRuleContext ruleCtx,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            RuleRunMode runMode);
    }
}