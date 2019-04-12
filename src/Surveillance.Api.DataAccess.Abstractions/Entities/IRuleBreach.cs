using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;

namespace Surveillance.Api.DataAccess.Abstractions.Entities
{
    public interface IRuleBreach
    {
        string AssetCfi { get; set; }
        string CorrelationId { get; set; }
        string Created { get; }
        string StartOfRuleBreachPeriod { get; }
        string EndOfRuleBreachPeriod { get; }

        int SystemOperationId { get; set; }



        string Description { get; set; }
        int Id { get; set; }
        bool IsBackTest { get; set; }
        int OrganisationalFactorType { get; set; }
        OrganisationalFactors OrganisationFactor { get; }
        string OrganisationalFactorValue { get; set; }
        string ReddeerEnrichmentId { get; set; }
        string RuleId { get; set; }
        string Title { get; set; }
        string Venue { get; set; }
    }
}