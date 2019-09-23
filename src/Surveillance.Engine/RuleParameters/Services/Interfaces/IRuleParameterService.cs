namespace Surveillance.Engine.Rules.RuleParameters.Services.Interfaces
{
    using System.Threading.Tasks;

    using Domain.Surveillance.Scheduling;

    using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;

    public interface IRuleParameterService
    {
        Task<RuleParameterDto> RuleParameters(ScheduledExecution execution);
    }
}