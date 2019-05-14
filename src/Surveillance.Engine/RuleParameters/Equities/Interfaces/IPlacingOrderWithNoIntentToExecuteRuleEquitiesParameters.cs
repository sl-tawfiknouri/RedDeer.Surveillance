﻿using System;
using Surveillance.Engine.Rules.RuleParameters.Interfaces;

namespace Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces
{
    public interface IPlacingOrderWithNoIntentToExecuteRuleEquitiesParameters 
        : IFilterableRule, IRuleParameter, IOrganisationalFactorable
    {
        decimal Sigma { get; }
        TimeSpan WindowSize { get; }
    }
}