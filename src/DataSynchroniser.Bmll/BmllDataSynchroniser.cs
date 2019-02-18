using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataSynchroniser.Api.Bmll.Bmll.Interfaces;
using DataSynchroniser.Api.Bmll.Filters.Interfaces;
using DataSynchroniser.Api.Bmll.Interfaces;
using Domain.Markets;
using Microsoft.Extensions.Logging;
using Surveillance.Auditing.Context.Interfaces;

namespace DataSynchroniser.Api.Bmll
{
    public class BmllDataSynchroniser : IBmllDataSynchroniser
    {
        private readonly IBmllDataRequestManager _requestManager;
        private readonly IMarketDataRequestFilter _filter;
        private readonly ILogger<BmllDataSynchroniser> _logger;

        public BmllDataSynchroniser(
            IBmllDataRequestManager requestManager,
            IMarketDataRequestFilter filter,
            ILogger<BmllDataSynchroniser> logger)
        {
            _requestManager = requestManager ?? throw new ArgumentNullException(nameof(requestManager));
            _filter = filter ?? throw new ArgumentNullException(nameof(filter));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(
            string systemProcessOperationId,
            ISystemProcessOperationThirdPartyDataRequestContext dataRequestContext,
            IReadOnlyCollection<MarketDataRequest> marketDataRequests)
        {
            if (string.IsNullOrWhiteSpace(systemProcessOperationId))
            {
                _logger?.LogError($"BmllDataSynchroniser Handle received a null or empty system process operation id returning");
                return;
            }

            if (dataRequestContext == null)
            {
                _logger?.LogError($"BmllDataSynchroniser Handle received a null data request context returning");
                return;
            }

            if (marketDataRequests == null
                || !marketDataRequests.Any())
            {
                _logger?.LogError($"BmllDataSynchroniser Handle received a null or empty market data requests collection");
                return;
            }

            var filteredMarketDataRequests = marketDataRequests.Where(_filter.Filter).ToList();
            if (!filteredMarketDataRequests.Any())
            {
                _logger?.LogInformation($"BmllDataSynchroniser Handle received a null or empty market data requests collection");
                return;
            }

            await _requestManager.Submit(systemProcessOperationId, filteredMarketDataRequests);
        }
    }
}
