namespace DataSynchroniser.Api.Factset.Factset
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using DataSynchroniser.Api.Factset.Factset.Interfaces;

    using Microsoft.Extensions.Logging;

    using SharedKernel.Contracts.Markets;

    using Surveillance.DataLayer.Aurora.Market.Interfaces;

    public class FactsetDataRequestsManager : IFactsetDataRequestsManager
    {
        private readonly ILogger<FactsetDataRequestsManager> _logger;

        private readonly IFactsetDataRequestsApiManager _requestApi;

        private readonly IReddeerMarketDailySummaryRepository _responseStorage;

        public FactsetDataRequestsManager(
            IFactsetDataRequestsApiManager requestApi,
            IReddeerMarketDailySummaryRepository responseRepository,
            ILogger<FactsetDataRequestsManager> logger)
        {
            this._requestApi = requestApi ?? throw new ArgumentNullException(nameof(requestApi));
            this._responseStorage = responseRepository ?? throw new ArgumentNullException(nameof(responseRepository));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Submit(
            IReadOnlyCollection<MarketDataRequest> factsetRequests,
            string systemProcessOperationId)
        {
            if (factsetRequests == null || !factsetRequests.Any())
            {
                this._logger.LogError(
                    $"{nameof(FactsetDataRequestsManager)} had null or empty factset requests for operation {systemProcessOperationId}");
                return;
            }

            try
            {
                var requests = factsetRequests.Where(fr => !fr?.IsCompleted ?? false).ToList();

                if (!requests.Any())
                {
                    this._logger.LogInformation(
                        $"{nameof(FactsetDataRequestsManager)} Submit had zero factset requests that were not already completed for operation {systemProcessOperationId}.");
                    return;
                }

                this._logger.LogInformation(
                    $"{nameof(FactsetDataRequestsManager)} Send about to send {requests.Count} requests to the request sender for operation {systemProcessOperationId}");
                var dailySummaries = await this._requestApi.Send(requests);
                this._logger.LogInformation(
                    $"{nameof(FactsetDataRequestsManager)} Send has sent {requests.Count} requests to the request sender for operation {systemProcessOperationId}");

                this._logger.LogInformation(
                    $"{nameof(FactsetDataRequestsManager)} Send about to record the response for {requests.Count} requests to the request sender for operation {systemProcessOperationId}");
                await this._responseStorage.Save(dailySummaries?.Responses);
                this._logger.LogInformation(
                    $"{nameof(FactsetDataRequestsManager)} Send has recorded the response for {requests.Count} requests to the request sender for operation {systemProcessOperationId}");
            }
            catch (Exception e)
            {
                this._logger.LogError(
                    $"{nameof(FactsetDataRequestsManager)} send method encountered an exception! for operation {systemProcessOperationId}",
                    e);
            }
        }
    }
}