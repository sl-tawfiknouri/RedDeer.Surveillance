namespace Surveillance.Api.DataAccess.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;

    using Surveillance.Api.DataAccess.Abstractions.Entities;

    public class RuleBreach : IRuleBreach
    {
        public string AssetCfi { get; set; }

        public string CorrelationId { get; set; }

        public DateTime CreatedOn { get; set; }

        public string Description { get; set; }

        public DateTime EndOfPeriodUnderInvestigation { get; set; }

        [Key]
        public int Id { get; set; }

        public bool IsBackTest { get; set; }

        public int OrganisationalFactorType { get; set; }

        public string OrganisationalFactorValue { get; set; }

        public OrganisationalFactors OrganisationFactor => (OrganisationalFactors)this.OrganisationalFactorType;

        public string ReddeerEnrichmentId { get; set; }

        public string RuleId { get; set; }

        public DateTime StartOfPeriodUnderInvestigation { get; set; }

        public int SystemOperationId { get; set; }

        public string Title { get; set; }

        public string Venue { get; set; }
    }
}