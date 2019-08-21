﻿namespace Surveillance.Reddeer.ApiClient.FactsetMarketData.Interfaces
{
    using System.Threading;
    using System.Threading.Tasks;

    using RedDeer.Contracts.SurveillanceService.Api.FactsetSecurityDaily;

    public interface IFactsetDailyBarApi
    {
        Task<FactsetSecurityResponseDto> Get(FactsetSecurityDailyRequest request);

        Task<FactsetSecurityResponseDto> GetWithTransientFaultHandling(FactsetSecurityDailyRequest request);

        Task<bool> HeartBeating(CancellationToken token);
    }
}