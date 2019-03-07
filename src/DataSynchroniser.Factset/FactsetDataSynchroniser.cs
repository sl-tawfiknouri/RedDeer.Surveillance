﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataSynchroniser.Api.Factset.Factset.Interfaces;
using DataSynchroniser.Api.Factset.Filters.Interfaces;
using DataSynchroniser.Api.Factset.Interfaces;
using Microsoft.Extensions.Logging;
using SharedKernel.Contracts.Markets;
using Surveillance.Auditing.Context.Interfaces;

namespace DataSynchroniser.Api.Factset
{
    public class FactsetDataSynchroniser : IFactsetDataSynchroniser
    {
        private readonly IFactsetDataRequestsManager _factsetDataRequestsManager;
        private readonly IFactsetDataRequestFilter _requestFilter;
        private readonly ILogger<FactsetDataSynchroniser> _logger;

        public FactsetDataSynchroniser(
            IFactsetDataRequestsManager factsetDataRequestsManager,
            IFactsetDataRequestFilter requestFilter,
            ILogger<FactsetDataSynchroniser> logger)
        {
            _factsetDataRequestsManager = factsetDataRequestsManager ?? throw new ArgumentNullException(nameof(factsetDataRequestsManager));
            _requestFilter = requestFilter ?? throw new ArgumentNullException(nameof(requestFilter));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(
            string systemProcessOperationId,
            ISystemProcessOperationThirdPartyDataRequestContext dataRequestContext,
            IReadOnlyCollection<MarketDataRequest> marketDataRequests)
        {
            if (string.IsNullOrWhiteSpace(systemProcessOperationId))
            {
                _logger.LogError($"{nameof(FactsetDataSynchroniser)} Handle received a null or empty system process operation id");
                return;
            }

            if (dataRequestContext == null)
            {
                _logger.LogError($"{nameof(FactsetDataSynchroniser)} Handle received a null data request context");
                return;
            }

            if (marketDataRequests == null
                || !marketDataRequests.Any())
            {
                _logger.LogError($"{nameof(FactsetDataSynchroniser)} Handle received a null or empty market data request collection");
                return;
            }

            var filteredMarketDataRequests = marketDataRequests.Where(_requestFilter.ValidAssetType).ToList();

            if (!filteredMarketDataRequests.Any())
            {
                _logger.LogInformation($"{nameof(FactsetDataSynchroniser)} Handle received market data requests but none passed the data request filter (equity CFI)");
                return;
            }

            await _factsetDataRequestsManager.Submit(filteredMarketDataRequests);
        }
    }
}
