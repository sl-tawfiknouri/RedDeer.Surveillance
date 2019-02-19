using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataSynchroniser.Api.Factset.Factset.Interfaces;
using DataSynchroniser.Api.Factset.Filters.Interfaces;
using DataSynchroniser.Api.Factset.Interfaces;
using Domain.Markets;
using Microsoft.Extensions.Logging;
using Surveillance.Auditing.Context.Interfaces;

namespace DataSynchroniser.Api.Factset
{
    public class FactsetDataSynchroniser : IFactsetDataSynchroniser
    {
        private readonly IFactsetDataRequestsManager factsetDataRequestsManager;
        private readonly IMarketDataRequestFilter _requestFilter;
        private readonly ILogger<FactsetDataSynchroniser> _logger;

        public FactsetDataSynchroniser(
            IFactsetDataRequestsManager dataRequestsManager,
            IMarketDataRequestFilter requestFilter,
            ILogger<FactsetDataSynchroniser> logger)
        {
            factsetDataRequestsManager = dataRequestsManager ?? throw new ArgumentNullException(nameof(dataRequestsManager));
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
                _logger.LogError($"FactsetDataSynchroniser Handle received a null or empty system process operation id");
                return;
            }

            if (dataRequestContext == null)
            {
                _logger.LogError($"FactsetDataSynchroniser Handle received a null data request context");
                return;
            }

            if (marketDataRequests == null
                || !marketDataRequests.Any())
            {
                _logger.LogError($"FactsetDataSynchroniser Handle received a null or empty market data request collection");
                return;
            }

            var filteredMarketDataRequests = marketDataRequests.Where(_requestFilter.Filter).ToList();

            if (!filteredMarketDataRequests.Any())
            {
                _logger.LogInformation($"FactsetDataSynchroniser Handle received market data requests but none passed the filter");
                return;
            }

            await factsetDataRequestsManager.Submit(filteredMarketDataRequests);
        }
    }
}
