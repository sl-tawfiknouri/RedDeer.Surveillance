using System;
using Surveillance.Engine.Rules.RuleParameters.Interfaces;

namespace Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces
{
    public interface IPlacingOrderWithNoIntentToExecuteRuleEquitiesParameters 
        : IFilterableRule, IRuleParameter, IOrganisationalFactorable, IReferenceDataFilterable
    {
        decimal Sigma { get; }
        TimeWindows Windows { get; }
    }
}
