using System.Threading.Tasks;
using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;
using Surveillance.DataLayer.Api.Interfaces;

namespace Surveillance.DataLayer.Api.RuleParameter.Interfaces
{
    public interface IRuleParameterApiRepository : IHeartbeatApi
    {
    Task<RuleParameterDto> Get();
    }
}