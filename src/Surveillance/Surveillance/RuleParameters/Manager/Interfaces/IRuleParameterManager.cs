using System.Threading.Tasks;
using DomainV2.Scheduling;
using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;

namespace Surveillance.RuleParameters.Manager.Interfaces
{
    public interface IRuleParameterManager
    {
        Task<RuleParameterDto> RuleParameters(ScheduledExecution execution);
    }
}