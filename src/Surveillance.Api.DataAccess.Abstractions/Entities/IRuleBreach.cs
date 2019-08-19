namespace Surveillance.Api.DataAccess.Abstractions.Entities
{
    using System;

    using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;

    public interface IRuleBreach
    {
        string AssetCfi { get; set; }

        string CorrelationId { get; set; }

        DateTime CreatedOn { get; }

        string Description { get; set; }

        DateTime EndOfPeriodUnderInvestigation { get; }

        int Id { get; set; }

        bool IsBackTest { get; set; }

        int OrganisationalFactorType { get; set; }

        string OrganisationalFactorValue { get; set; }

        OrganisationalFactors OrganisationFactor { get; }

        string ReddeerEnrichmentId { get; set; }

        string RuleId { get; set; }

        DateTime StartOfPeriodUnderInvestigation { get; }

        int SystemOperationId { get; set; }

        string Title { get; set; }

        string Venue { get; set; }
    }
}