// ReSharper disable UnusedMember.Global

namespace Surveillance.Engine.Rules.Rules.Equity.MarkingTheClose.Interfaces
{
    using System.Collections.Generic;

    using Surveillance.Engine.Rules.RuleParameters;
    using Surveillance.Engine.Rules.RuleParameters.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.OrganisationalFactors;

    public interface IMarkingTheCloseEquitiesParameters : IFilterableRule,
                                                          IRuleParameter,
                                                          IReferenceDataFilterable,
                                                          IMarketCapFilterable,
                                                          IVenueVolumeFilterable
    {
        bool AggregateNonFactorableIntoOwnCategory { get; set; }

        IReadOnlyCollection<ClientOrganisationalFactors> Factors { get; set; }

        decimal? PercentageThresholdDailyVolume { get; }

        decimal? PercentageThresholdWindowVolume { get; }

        decimal? PercentThresholdOffTouch { get; }

        TimeWindows Windows { get; }
    }
}