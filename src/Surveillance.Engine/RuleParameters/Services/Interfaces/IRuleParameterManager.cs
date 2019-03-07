using System.Threading.Tasks;
using Domain.Surveillance.Scheduling;
using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;

namespace Surveillance.Engine.Rules.RuleParameters.Services.Interfaces
{
    public interface IRuleParameterManager
    {
        Task<RuleParameterDto> RuleParameters(ScheduledExecution execution);
    }
}