using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataSynchroniser.Api.Factset.Factset.Interfaces;
using Microsoft.Extensions.Logging;
using RedDeer.Contracts.SurveillanceService.Api.FactsetSecurityDaily;
using SharedKernel.Contracts.Markets;
using Surveillance.Reddeer.ApiClient.FactsetMarketData.Interfaces;

namespace DataSynchroniser.Api.Factset.Factset
{
    public class FactsetDataRequestsApiManager : IFactsetDataRequestsApiManager
    {
        private readonly IFactsetDailyBarApi _dailyBarRepository;
        private readonly ILogger<FactsetDataRequestsApiManager> _logger;

        public FactsetDataRequestsApiManager(
            IFactsetDailyBarApi dailyBarRepository,
            ILogger<FactsetDataRequestsApiManager> logger)
        {
            _dailyBarRepository = dailyBarRepository ?? throw new ArgumentNullException(nameof(dailyBarRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<FactsetSecurityResponseDto> Send(IReadOnlyCollection<MarketDataRequest> factsetRequests)
        {
            if (factsetRequests == null
                || !factsetRequests.Any())
            {
                _logger?.LogInformation($"send received a null factset requests list. Returning");
                return new FactsetSecurityResponseDto();
            }

            var requests = factsetRequests.Select(Project).ToList();
            var request = new FactsetSecurityDailyRequest
            {
                Requests = requests
            };

            try
            {
                var result = await _dailyBarRepository.GetWithTransientFaultHandling(request);

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"send encountered an exception when posting to the factset daily bar api {e.Message} {e?.InnerException?.Message}");
            }

            return new FactsetSecurityResponseDto
            {
                Request = request,
                Responses = new List<FactsetSecurityDailyResponseItem>()
            };
        }

        private FactsetSecurityRequestItem Project(MarketDataRequest req)
        {
            return new FactsetSecurityRequestItem
            {
                Figi = req?.Identifiers.Figi,
                From = req?.UniverseEventTimeFrom?.AddDays(-1) ?? DateTime.UtcNow,
                To = req?.UniverseEventTimeTo?.AddDays(1) ?? DateTime.UtcNow,
            };
        }
    }
}
