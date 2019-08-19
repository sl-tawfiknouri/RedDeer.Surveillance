namespace Surveillance.Engine.Rules.RuleParameters.Filter.Interfaces
{
    using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;

    public interface IRuleProjector
    {
        RuleFilter Project(Filter filter);
    }
}