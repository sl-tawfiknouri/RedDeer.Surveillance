namespace Surveillance.Rule_Parameters.Filter.Interfaces
{
    public interface IRuleProjector
    {
        RuleFilter Project(RedDeer.Contracts.SurveillanceService.Api.RuleParameter.Filter filter);
    }
}