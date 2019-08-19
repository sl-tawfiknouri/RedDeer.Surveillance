﻿namespace Surveillance.Engine.Rules.RuleParameters.FixedIncome.Interfaces
{
    using Surveillance.Engine.Rules.RuleParameters.Interfaces;

    public interface IHighProfitsRuleFixedIncomeParameters : IFilterableRule, IRuleParameter, IOrganisationalFactorable
    {
        TimeWindows Windows { get; }
    }
}