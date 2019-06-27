using System;
using System.ComponentModel.DataAnnotations;
using Surveillance.Api.DataAccess.Abstractions.Entities;
using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;

namespace Surveillance.Api.DataAccess.Entities
{
    public class RuleBreach : IRuleBreach
    {
        public RuleBreach()
        {
        }

        [Key]
        public int Id { get; set; }
        public string RuleId { get; set; }
        public string CorrelationId { get; set; }
        public bool IsBackTest { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime StartOfPeriodUnderInvestigation { get; set; }
        public DateTime EndOfPeriodUnderInvestigation { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Venue { get; set; }
        public string AssetCfi { get; set; }
        public string ReddeerEnrichmentId { get; set; }
        public int SystemOperationId { get; set; }
        public int OrganisationalFactorType { get; set; }
        public OrganisationalFactors OrganisationFactor => (OrganisationalFactors)OrganisationalFactorType;
        public string OrganisationalFactorValue { get; set; }
    }
}
