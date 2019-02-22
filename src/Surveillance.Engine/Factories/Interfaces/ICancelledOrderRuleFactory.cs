﻿using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Interfaces;
using Surveillance.Engine.Rules.Rules;
using Surveillance.Engine.Rules.Rules.CancelledOrders.Interfaces;

namespace Surveillance.Engine.Rules.Factories.Interfaces
{
    public interface ICancelledOrderRuleFactory
    {
        ICancelledOrderRule Build(
            ICancelledOrderRuleParameters parameters,
            ISystemProcessOperationRunRuleContext ruleCtx,
            IUniverseAlertStream alertStream,
            RuleRunMode runMode);
    }
}