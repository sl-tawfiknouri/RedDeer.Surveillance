using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataSynchroniser.Api.Factset.Factset.Interfaces;
using Domain.Markets;
using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.Aurora.Market.Interfaces;

namespace DataSynchroniser.Api.Factset.Factset
{
    public class FactsetDataRequestsManager : IFactsetDataRequestsManager
    {
        private readonly IFactsetDataRequestsSenderManager _requestSender;
        private readonly IReddeerMarketDailySummaryRepository _responseStorage;
        private readonly ILogger<FactsetDataRequestsManager> _logger;

        public FactsetDataRequestsManager(
            IFactsetDataRequestsSenderManager requestSender,
            IReddeerMarketDailySummaryRepository responseRepository,
            ILogger<FactsetDataRequestsManager> logger)
        {
            _requestSender = requestSender ?? throw new ArgumentNullException(nameof(requestSender));
            _responseStorage = responseRepository ?? throw new ArgumentNullException(nameof(responseRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Submit(IReadOnlyCollection<MarketDataRequest> factsetRequests)
        {
            if (factsetRequests == null
                || !factsetRequests.Any())
            {
                _logger.LogError($"{nameof(FactsetDataRequestsManager)} had null or empty factset requests");
                return;
            }

            try
            {
                var requests = factsetRequests.Where(fr => !fr?.IsCompleted ?? false).ToList();

                if ( !requests.Any())
                {
                    _logger.LogInformation($"{nameof(FactsetDataRequestsManager)} Submit had zero factset requests that were not already completed.");
                    return;
                }

                _logger.LogInformation($"{nameof(FactsetDataRequestsManager)} Send about to send {requests.Count} requests to the request sender");
                var dailySummaries = await _requestSender.Send(requests);
                _logger.LogInformation($"{nameof(FactsetDataRequestsManager)} Send has sent {requests.Count} requests to the request sender");

                _logger.LogInformation($"{nameof(FactsetDataRequestsManager)} Send about to record the response for {requests.Count} requests to the request sender");
                await _responseStorage.Save(dailySummaries?.Responses);
                _logger.LogInformation($"{nameof(FactsetDataRequestsManager)} Send has recorded the response for {requests.Count} requests to the request sender");

            }
            catch (Exception e)
            {
                _logger.LogError($"{nameof(FactsetDataRequestsManager)} send method encountered an exception!", e);
            }
        }
    }
}