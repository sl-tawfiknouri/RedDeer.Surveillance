using System;
using Surveillance.Engine.Rules.RuleParameters.Interfaces;

namespace Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces
{
    public interface ILayeringRuleEquitiesParameters : IFilterableRule, IRuleParameter, IOrganisationalFactorable
    {
        TimeWindows Windows { get; }
        decimal? PercentageOfMarketDailyVolume { get; }
        decimal? PercentageOfMarketWindowVolume { get; }
        bool? CheckForCorrespondingPriceMovement { get; }
    }
}