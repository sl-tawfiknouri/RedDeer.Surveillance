using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataSynchroniser.Api.Factset.Factset.Interfaces;
using Domain.Markets;
using Microsoft.Extensions.Logging;

namespace DataSynchroniser.Api.Factset.Factset
{
    public class FactsetDataRequestsManager : IFactsetDataRequestsManager
    {
        private readonly IFactsetDataRequestsSenderManager _requestSender;
        private readonly IFactsetDataRequestsStorageManager _responseStorage;
        private readonly ILogger<FactsetDataRequestsManager> _logger;

        public FactsetDataRequestsManager(
            IFactsetDataRequestsSenderManager requestSender,
            IFactsetDataRequestsStorageManager responseStorage,
            ILogger<FactsetDataRequestsManager> logger)
        {
            _requestSender = requestSender ?? throw new ArgumentNullException(nameof(requestSender));
            _responseStorage = responseStorage ?? throw new ArgumentNullException(nameof(responseStorage));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Submit(List<MarketDataRequest> factsetRequests)
        {
            if (factsetRequests == null
                || !factsetRequests.Any())
            {
                _logger.LogError($"FactsetDataRequestsManager had null or empty factset requests");
            }

            try
            {
                var requests = factsetRequests.Where(fr => !fr?.IsCompleted ?? false).ToList();

                if ( !requests.Any())
                {
                    _logger.LogInformation($"FactsetDataRequestsManager Submit had zero factset requests that were not already completed.");
                    return;
                }

                _logger.LogInformation($"FactsetDataRequestsManager Send about to send {requests.Count} requests to the request sender");
                var dailySummaries = await _requestSender.Send(requests);
                _logger.LogInformation($"FactsetDataRequestsManager Send has sent {requests.Count} requests to the request sender");

                _logger.LogInformation($"FactsetDataRequestsManager Send about to record the response for {requests.Count} requests to the request sender");
                await _responseStorage.Store(dailySummaries);
                _logger.LogInformation($"FactsetDataRequestsManager Send has recorded the response for {requests.Count} requests to the request sender");

            }
            catch (Exception e)
            {
                _logger.LogError("FactsetDataRequestsManager send method encountered an exception!", e);
            }
        }
    }
}