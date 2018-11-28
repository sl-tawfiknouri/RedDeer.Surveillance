using System;
using System.Collections.Generic;
using Surveillance.RuleParameters.Interfaces;
using Surveillance.RuleParameters.OrganisationalFactors;

// ReSharper disable UnusedMember.Global

namespace Surveillance.Rules.MarkingTheClose.Interfaces
{
    public interface IMarkingTheCloseParameters : IFilterableRule
    {
        decimal? PercentageThresholdDailyVolume { get; }
        decimal? PercentageThresholdWindowVolume { get; }
        decimal? PercentThresholdOffTouch { get; }
        TimeSpan Window { get; }
        IReadOnlyCollection<ClientOrganisationalFactors> Factors { get; set; }
        bool AggregateNonFactorableIntoOwnCategory { get; set; }
    }
}