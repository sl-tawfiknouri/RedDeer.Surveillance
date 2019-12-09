using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataSynchroniser.Api.Refinitive.Interfaces;
using Firefly.Service.Data.TickPriceHistory.Shared.Protos;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging;
using SharedKernel.Contracts.Markets;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Data.Universe.Refinitiv.Interfaces;

namespace DataSynchroniser.Api.Refinitive
{
    public class RefinitivDataSynchroniser : IRefinitivDataSynchroniser
    {
        private readonly ITickPriceHistoryServiceClientFactory _tickPriceHistoryServiceClientFactory;
        private readonly IRefinitivTickPriceHistoryApiConfig _refinitivTickPriceHistoryApiConfig;
        private readonly ILogger<IRefinitivDataSynchroniser> _logger;

        public RefinitivDataSynchroniser(ITickPriceHistoryServiceClientFactory tickPriceHistoryServiceClientFactory, IRefinitivTickPriceHistoryApiConfig refinitivTickPriceHistoryApiConfig,
            ILogger<IRefinitivDataSynchroniser> logger)
        {
            this._tickPriceHistoryServiceClientFactory = tickPriceHistoryServiceClientFactory ?? throw new ArgumentNullException(nameof(tickPriceHistoryServiceClientFactory));
            this._refinitivTickPriceHistoryApiConfig = refinitivTickPriceHistoryApiConfig ?? throw new ArgumentNullException(nameof(refinitivTickPriceHistoryApiConfig));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(string systemProcessOperationId, ISystemProcessOperationThirdPartyDataRequestContext dataRequestContext, IReadOnlyCollection<MarketDataRequest> marketDataRequests)
        {

            if (marketDataRequests == null || !marketDataRequests.Any())
            {
                this._logger.LogError($"{nameof(RefinitivDataSynchroniser)} Handle received a null or empty market data request collection");
                return;
            }

            var maxToDate = marketDataRequests.Select(a => a.UniverseEventTimeTo).Max();
            var minFromDate = marketDataRequests.Select(a => a.UniverseEventTimeFrom).Min();
            
            if (maxToDate != null && minFromDate != null)
            {
                var request = new GetEodPricingRequest
                {
                    StartUtc = minFromDate.Value.ToUniversalTime().ToTimestamp(),
                    EndUtc = maxToDate.Value.ToUniversalTime().ToTimestamp(),
                    PollPeriod = _refinitivTickPriceHistoryApiConfig.RefinitivTickPriceHistoryApiPollingSeconds,
                    TimeOut = new Duration() { Seconds = _refinitivTickPriceHistoryApiConfig.RefinitivTickPriceHistoryApiTimeOutDurationSeconds },
                };

                var ids = marketDataRequests.Select(req => new SecurityIdentifiers()
                {
                    Ric = req.Identifiers.Ric,
                    Cusip = req.Identifiers.Cusip,
                    Isin = req.Identifiers.Isin,
                    Sedol = req.Identifiers.Sedol
                });

                request.Identifiers.AddRange(ids);

                if (request.Identifiers.All(s => s.Ric == null))
                {
                    this._logger.LogError($"{nameof(RefinitivDataSynchroniser)} Handle received a request that didn't have a RIC");
                    return;
                }

                this._logger.LogInformation($"{nameof(RefinitivDataSynchroniser)} Making request for the date range of {request.StartUtc} - {request.EndUtc} using the following RIC identifiers: " +
                                           $"{string.Join(" ", request.Identifiers.Select(s => s.Ric).ToArray())}");

                var tickPriceHistoryServiceClient = _tickPriceHistoryServiceClientFactory.Create();
                await tickPriceHistoryServiceClient.GetEodPricingAsync(request);
                
                this._logger.LogInformation($"{nameof(RefinitivDataSynchroniser)} Request returned for the date range of {request.StartUtc} - {request.EndUtc} using the following RIC identifiers: " +
                                            $"{string.Join(" ", request.Identifiers.Select(s => s.Ric).ToArray())}");
            }
            else
            {
                this._logger.LogError($"{nameof(RefinitivDataSynchroniser)} Handle received a request collection with no dates");
                return;
            }
        }
    }
}