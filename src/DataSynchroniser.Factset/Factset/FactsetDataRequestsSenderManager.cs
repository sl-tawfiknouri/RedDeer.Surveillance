using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataSynchroniser.Api.Factset.Factset.Interfaces;
using Domain.Markets;
using Microsoft.Extensions.Logging;
using RedDeer.Contracts.SurveillanceService.Api.FactsetSecurityDaily;
using Surveillance.DataLayer.Api.FactsetMarketData.Interfaces;

namespace DataSynchroniser.Api.Factset.Factset
{
    public class FactsetDataRequestsSenderManager : IFactsetDataRequestsSenderManager
    {
        private readonly IFactsetDailyBarApiRepository _dailyBarRepository;
        private readonly ILogger<FactsetDataRequestsSenderManager> _logger;

        public FactsetDataRequestsSenderManager(
            IFactsetDailyBarApiRepository dailyBarRepository,
            ILogger<FactsetDataRequestsSenderManager> logger)
        {
            _dailyBarRepository = dailyBarRepository ?? throw new ArgumentNullException(nameof(dailyBarRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<FactsetSecurityResponseDto> Send(List<MarketDataRequest> factsetRequests)
        {
            if (factsetRequests == null
                || !factsetRequests.Any())
            {
                _logger?.LogInformation($"FactsetDataRequestsSenderManager Send received a null factset requests list. Returning");
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
                _logger.LogError($"FactsetDataRequestsSenderManager send encountered an exception when posting to the factset daily bar api", e);
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
