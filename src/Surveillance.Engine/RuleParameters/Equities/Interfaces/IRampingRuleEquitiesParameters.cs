using System;
using Surveillance.Engine.Rules.RuleParameters.Interfaces;

namespace Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces
{
    public interface IRampingRuleEquitiesParameters : IFilterableRule, IRuleParameter, IOrganisationalFactorable, IReferenceDataFilterable, IMarketCapFilterable, IVenueVolumeFilterable
    {
        TimeWindows Windows { get; }

        decimal AutoCorrelationCoefficient { get; }

        // optional noise reduction
        int? ThresholdOrdersExecutedInWindow { get; }
        decimal? ThresholdVolumePercentageWindow { get; }
    }
}
