using System;
using Surveillance.Engine.Rules.RuleParameters.Interfaces;

namespace Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces
{
    public interface IWashTradeRuleEquitiesParameters : IFilterableRule, IRuleParameter, IOrganisationalFactorable, IWashTradeRuleParameters
    {
        TimeWindows Windows { get; }
    }
}
