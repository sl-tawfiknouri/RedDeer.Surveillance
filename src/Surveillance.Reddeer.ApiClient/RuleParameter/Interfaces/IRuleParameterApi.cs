using System.Threading.Tasks;
using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;
using Surveillance.Reddeer.ApiClient.Interfaces;

namespace Surveillance.Reddeer.ApiClient.RuleParameter.Interfaces
{
    public interface IRuleParameterApi : IHeartbeatApi
    {
        Task<RuleParameterDto> Get();
        Task<RuleParameterDto> Get(string id);
    }
}