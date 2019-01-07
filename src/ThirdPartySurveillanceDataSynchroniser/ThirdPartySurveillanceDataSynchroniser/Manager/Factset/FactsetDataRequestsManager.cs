using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ThirdPartySurveillanceDataSynchroniser.Manager.Factset.Interfaces;

namespace ThirdPartySurveillanceDataSynchroniser.Manager.Factset
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

        public async Task Submit(List<MarketDataRequestDataSource> factsetRequests)
        {
            if (factsetRequests == null
                || !factsetRequests.Any())
            {
                _logger.LogError($"FactsetDataRequestsManager had null or empty factset requests");
            }

            try
            {
                _logger.LogInformation($"FactsetDataRequestsManager Send about to send {factsetRequests.Count} requests to the request sender");
                await _requestSender.Send(factsetRequests);
                _logger.LogInformation($"FactsetDataRequestsManager Send has sent {factsetRequests.Count} requests to the request sender");

                _logger.LogInformation($"FactsetDataRequestsManager Send about to record the response for {factsetRequests.Count} requests to the request sender");
                await _responseStorage.Store();
                _logger.LogInformation($"FactsetDataRequestsManager Send has recorded the response for {factsetRequests.Count} requests to the request sender");
            }
            catch (Exception e)
            {
                _logger.LogError("FactsetDataRequestsManager send method encountered an exception!", e);
            }
        }
    }
}