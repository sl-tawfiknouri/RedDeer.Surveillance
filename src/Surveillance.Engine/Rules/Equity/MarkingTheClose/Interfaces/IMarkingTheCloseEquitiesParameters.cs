using System;
using System.Collections.Generic;
using Surveillance.Engine.Rules.RuleParameters;
using Surveillance.Engine.Rules.RuleParameters.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.OrganisationalFactors;

// ReSharper disable UnusedMember.Global

namespace Surveillance.Engine.Rules.Rules.Equity.MarkingTheClose.Interfaces
{
    public interface IMarkingTheCloseEquitiesParameters : IFilterableRule, IRuleParameter, IReferenceDataFilterable, IMarketCapFilterable
    {
        decimal? PercentageThresholdDailyVolume { get; }
        decimal? PercentageThresholdWindowVolume { get; }
        decimal? PercentThresholdOffTouch { get; }
        TimeWindows Windows { get; }
        IReadOnlyCollection<ClientOrganisationalFactors> Factors { get; set; }
        bool AggregateNonFactorableIntoOwnCategory { get; set; }
    }
}