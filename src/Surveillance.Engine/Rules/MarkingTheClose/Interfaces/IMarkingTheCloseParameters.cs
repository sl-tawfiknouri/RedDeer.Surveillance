using System;
using System.Collections.Generic;
using Surveillance.Engine.Rules.RuleParameters.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.OrganisationalFactors;

// ReSharper disable UnusedMember.Global

namespace Surveillance.Engine.Rules.Rules.MarkingTheClose.Interfaces
{
    public interface IMarkingTheCloseParameters : IFilterableRule, IRuleParameter
    {
        decimal? PercentageThresholdDailyVolume { get; }
        decimal? PercentageThresholdWindowVolume { get; }
        decimal? PercentThresholdOffTouch { get; }
        TimeSpan Window { get; }
        IReadOnlyCollection<ClientOrganisationalFactors> Factors { get; set; }
        bool AggregateNonFactorableIntoOwnCategory { get; set; }
    }
}