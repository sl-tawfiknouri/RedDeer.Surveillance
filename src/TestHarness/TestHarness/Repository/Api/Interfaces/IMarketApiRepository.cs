namespace TestHarness.Repository.Api.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using RedDeer.Contracts.SurveillanceService.Api.Markets;

    public interface IMarketApiRepository
    {
        Task<IReadOnlyCollection<ExchangeDto>> Get();

        Task<bool> HeartBeating();
    }
}