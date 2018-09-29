using System.Threading.Tasks;
using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;

namespace Surveillance.DataLayer.Api.RuleParameter.Interfaces
{
    public interface IRuleParameterApiRepository
    {
        Task<RuleParameterDto> Get();
    }
}