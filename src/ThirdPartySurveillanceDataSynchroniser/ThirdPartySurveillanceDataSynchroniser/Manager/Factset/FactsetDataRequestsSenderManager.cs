using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataSynchroniser.Manager.Factset.Interfaces;
using Microsoft.Extensions.Logging;
using RedDeer.Contracts.SurveillanceService.Api.FactsetSecurityDaily;
using Surveillance.DataLayer.Api.FactsetMarketData.Interfaces;

namespace DataSynchroniser.Manager.Factset
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

        public async Task<FactsetSecurityResponseDto> Send(List<MarketDataRequestDataSource> factsetRequests)
        {
            if (factsetRequests == null
                || !factsetRequests.Any())
            {
                _logger?.LogInformation($"FactsetDataRequestsSenderManager Send received a null factset requests list. Returning");
                return new FactsetSecurityResponseDto();
            }

            try
            {
                var cts = new CancellationTokenSource(1000 * 60 * 30);
                var checkHeartbeat = await _dailyBarRepository.HeartBeating(cts.Token);

                while (!checkHeartbeat)
                {
                    if (cts.IsCancellationRequested)
                    {
                        _logger.LogError($"FactsetDataRequestsSenderManager Send ran out of time to connect to the API. Returning an empty response.");
                        return new FactsetSecurityResponseDto();
                    }

                    _logger.LogError($"FactsetDataRequestsSenderManager Send could not elicit a successful heartbeat response. Waiting for a maximum of 30 minutes...");
                    Thread.Sleep(1000 * 30);
                    checkHeartbeat = await _dailyBarRepository.HeartBeating(cts.Token);
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"FactsetDataRequestsSenderManager send encountered an exception whilst handling heartbeats", e);
                return new FactsetSecurityResponseDto();
            }

            var requests = factsetRequests.Select(Project).ToList();
            var request = new FactsetSecurityDailyRequest
            {
                Requests = requests
            };

            try
            {
                var result = await _dailyBarRepository.Get(request);

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

        private FactsetSecurityRequestItem Project(MarketDataRequestDataSource req)
        {
            return new FactsetSecurityRequestItem
            {
                Figi = req.DataRequest?.Identifiers.Figi,
                From = req.DataRequest?.UniverseEventTimeFrom?.AddDays(-1) ?? DateTime.UtcNow,
                To = req.DataRequest?.UniverseEventTimeTo?.AddDays(1) ?? DateTime.UtcNow,
            };
        }
    }
}
