﻿using Surveillance.Rule_Parameters.Interfaces;
using Surveillance.Rules.Layering.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.Factories.Interfaces
{
    public interface ILayeringRuleFactory
    {
        ILayeringRule Build(ILayeringRuleParameters parameters, ISystemProcessOperationRunRuleContext ruleCtx);
        string RuleVersion { get; }
    }
}