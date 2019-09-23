namespace DataSynchroniser.Api.Bmll
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using DataSynchroniser.Api.Bmll.Bmll.Interfaces;
    using DataSynchroniser.Api.Bmll.Filters.Interfaces;
    using DataSynchroniser.Api.Bmll.Interfaces;

    using Microsoft.Extensions.Logging;

    using SharedKernel.Contracts.Markets;

    using Surveillance.Auditing.Context.Interfaces;

    public class BmllDataSynchroniser : IBmllDataSynchroniser
    {
        private readonly IBmllDataRequestFilter _filter;

        private readonly ILogger<BmllDataSynchroniser> _logger;

        private readonly IBmllDataRequestManager _requestManager;

        public BmllDataSynchroniser(
            IBmllDataRequestManager requestManager,
            IBmllDataRequestFilter filter,
            ILogger<BmllDataSynchroniser> logger)
        {
            this._requestManager = requestManager ?? throw new ArgumentNullException(nameof(requestManager));
            this._filter = filter ?? throw new ArgumentNullException(nameof(filter));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(
            string systemProcessOperationId,
            ISystemProcessOperationThirdPartyDataRequestContext dataRequestContext,
            IReadOnlyCollection<MarketDataRequest> marketDataRequests)
        {
            this._logger.LogInformation(
                $"{nameof(BmllDataSynchroniser)} Handle began processing a request for {nameof(systemProcessOperationId)} {systemProcessOperationId}");

            if (string.IsNullOrWhiteSpace(systemProcessOperationId))
            {
                this._logger?.LogError(
                    $"{nameof(BmllDataSynchroniser)} Handle received a null or empty system process operation id returning");
                return;
            }

            if (dataRequestContext == null)
            {
                this._logger?.LogError(
                    $"{nameof(BmllDataSynchroniser)} Handle received a null data request context returning");
                return;
            }

            if (marketDataRequests == null || !marketDataRequests.Any())
            {
                this._logger?.LogError(
                    $"{nameof(BmllDataSynchroniser)} Handle received a null or empty market data requests collection");
                return;
            }

            var filteredMarketDataRequests = marketDataRequests.Where(this._filter.ValidAssetType).ToList();
            if (!filteredMarketDataRequests.Any())
            {
                this._logger?.LogInformation(
                    $"{nameof(BmllDataSynchroniser)} Handle received a null or empty market data requests collection");
                return;
            }

            await this._requestManager.Submit(systemProcessOperationId, filteredMarketDataRequests);

            this._logger.LogInformation($"{nameof(BmllDataSynchroniser)} Handle completed processing a request");
        }
    }
}