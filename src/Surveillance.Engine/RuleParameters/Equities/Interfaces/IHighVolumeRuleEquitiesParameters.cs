using System;
using Surveillance.Engine.Rules.RuleParameters.Interfaces;

namespace Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces
{
    public interface IHighVolumeRuleEquitiesParameters : IFilterableRule, IRuleParameter, IOrganisationalFactorable, IReferenceDataFilterable
    {
        decimal? HighVolumePercentageDaily { get; }
        decimal? HighVolumePercentageWindow { get; }
        decimal? HighVolumePercentageMarketCap { get; }
        TimeWindows Windows { get; }
    }
}