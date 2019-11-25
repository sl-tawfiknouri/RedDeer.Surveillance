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
            
            var tickPriceHistoryServiceClient = _tickPriceHistoryServiceClientFactory.Create();

            var requests = marketDataRequests.Select(req =>
            {
                var r = new GetEodPricingRequest
                {
                    StartUtc = req.UniverseEventTimeFrom.Value.ToUniversalTime().ToTimestamp(),
                    EndUtc = req.UniverseEventTimeTo.Value.ToUniversalTime().ToTimestamp(),
                    PollPeriod = _refinitivTickPriceHistoryApiConfig.RefinitivTickPriceHistoryApiPollingSeconds,
                    TimeOut = new Duration() {Seconds = _refinitivTickPriceHistoryApiConfig.RefinitivTickPriceHistoryApiTimeOutDurationSeconds},
                };
                
                r.Identifiers.Add(new SecurityIdentifiers()
                {
                    Ric = req.Identifiers.Ric,
                    Cusip = req.Identifiers.Cusip,
                    Isin = req.Identifiers.Isin,
                    Sedol = req.Identifiers.Sedol
                });
                
                return r;
            });

            foreach (var request in requests)
            {
                if (!request.Identifiers.Any(s => s.Ric != null))
                {
                    this._logger.LogError($"{nameof(RefinitivDataSynchroniser)} Handle received a request that didn't have a RIC");
                    continue;
                }
                
                this._logger.LogInformation($"{nameof(RefinitivDataSynchroniser)} Making request to TR Service for {request.Identifiers.First().Ric} {request.StartUtc} ->  {request.EndUtc}");
                await tickPriceHistoryServiceClient.GetEodPricingAsync(request);
            }
        }
    }
}