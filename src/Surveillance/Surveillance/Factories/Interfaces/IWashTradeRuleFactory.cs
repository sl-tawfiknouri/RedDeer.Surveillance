﻿using Surveillance.Analytics.Streams.Interfaces;
using Surveillance.RuleParameters.Interfaces;
using Surveillance.Rules.WashTrade.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.Factories.Interfaces
{
    public interface IWashTradeRuleFactory
    {
        IWashTradeRule Build(IWashTradeRuleParameters parameters, ISystemProcessOperationRunRuleContext ruleCtx, IUniverseAlertStream alertStream);
    }
}