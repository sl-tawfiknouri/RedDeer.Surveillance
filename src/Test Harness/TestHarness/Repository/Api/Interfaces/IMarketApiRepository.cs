using System.Collections.Generic;
using System.Threading.Tasks;
using RedDeer.Contracts.SurveillanceService.Api.Markets;

namespace TestHarness.Repository.Api.Interfaces
{
    public interface IMarketApiRepository
    {
        Task<bool> HeartBeating();
        Task<IReadOnlyCollection<ExchangeDto>> Get();
    }
}