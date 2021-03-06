﻿namespace Surveillance.Engine.Rules.Factories.Equities.Interfaces
{
    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
    using Surveillance.Engine.Rules.Rules;
    using Surveillance.Engine.Rules.Rules.Equity.CancelledOrders.Interfaces;

    public interface IEquityRuleCancelledOrderFactory
    {
        ICancelledOrderRule Build(
            ICancelledOrderRuleEquitiesParameters parameters,
            ISystemProcessOperationRunRuleContext ruleCtx,
            IUniverseAlertStream alertStream,
            RuleRunMode runMode);
    }
}