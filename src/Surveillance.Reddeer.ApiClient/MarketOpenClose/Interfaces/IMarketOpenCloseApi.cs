namespace Surveillance.Reddeer.ApiClient.MarketOpenClose.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using RedDeer.Contracts.SurveillanceService.Api.Markets;

    using Surveillance.Reddeer.ApiClient.Interfaces;

    public interface IMarketOpenCloseApi : IHeartbeatApi
    {
        Task<IReadOnlyCollection<ExchangeDto>> Get();
    }
}