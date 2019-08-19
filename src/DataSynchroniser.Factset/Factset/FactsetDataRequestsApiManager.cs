namespace DataSynchroniser.Api.Factset.Factset
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using DataSynchroniser.Api.Factset.Factset.Interfaces;

    using Microsoft.Extensions.Logging;

    using RedDeer.Contracts.SurveillanceService.Api.FactsetSecurityDaily;

    using SharedKernel.Contracts.Markets;

    using Surveillance.Reddeer.ApiClient.FactsetMarketData.Interfaces;

    public class FactsetDataRequestsApiManager : IFactsetDataRequestsApiManager
    {
        private readonly IFactsetDailyBarApi _dailyBarRepository;

        private readonly ILogger<FactsetDataRequestsApiManager> _logger;

        public FactsetDataRequestsApiManager(
            IFactsetDailyBarApi dailyBarRepository,
            ILogger<FactsetDataRequestsApiManager> logger)
        {
            this._dailyBarRepository =
                dailyBarRepository ?? throw new ArgumentNullException(nameof(dailyBarRepository));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<FactsetSecurityResponseDto> Send(IReadOnlyCollection<MarketDataRequest> factsetRequests)
        {
            if (factsetRequests == null || !factsetRequests.Any())
            {
                this._logger?.LogInformation("send received a null factset requests list. Returning");
                return new FactsetSecurityResponseDto();
            }

            var requests = factsetRequests.Select(this.Project).ToList();
            var request = new FactsetSecurityDailyRequest { Requests = requests };

            try
            {
                var result = await this._dailyBarRepository.GetWithTransientFaultHandling(request);

                return result;
            }
            catch (Exception e)
            {
                this._logger.LogError(
                    e,
                    $"send encountered an exception when posting to the factset daily bar api {e.Message} {e?.InnerException?.Message}");
            }

            return new FactsetSecurityResponseDto
                       {
                           Request = request, Responses = new List<FactsetSecurityDailyResponseItem>()
                       };
        }

        private FactsetSecurityRequestItem Project(MarketDataRequest req)
        {
            return new FactsetSecurityRequestItem
                       {
                           Figi = req?.Identifiers.Figi,
                           From = req?.UniverseEventTimeFrom?.AddDays(-1) ?? DateTime.UtcNow,
                           To = req?.UniverseEventTimeTo?.AddDays(1) ?? DateTime.UtcNow
                       };
        }
    }
}