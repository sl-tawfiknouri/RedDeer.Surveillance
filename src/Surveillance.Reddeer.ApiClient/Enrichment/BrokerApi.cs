using System;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Network.HttpClient.Interfaces;
using Microsoft.Extensions.Logging;
using PollyFacade.Policies.Interfaces;
using RedDeer.Contracts.SurveillanceService.Api.BrokerEnrichment;
using Surveillance.Reddeer.ApiClient.Configuration.Interfaces;
using Surveillance.Reddeer.ApiClient.Enrichment.Interfaces;

namespace Surveillance.Reddeer.ApiClient.Enrichment
{
    public class BrokerApi : BaseClientServiceApi, IBrokerApi
    {
        private const string HeartbeatRoute = "api/BrokerEnrichment/Heartbeat";
        private const string Route = "api/BrokerEnrichment/Enrichment/V1";

        private readonly ILogger _logger;

        public BrokerApi(
            IApiClientConfiguration apiClientConfiguration,
            IHttpClientFactory httpClientFactory,
            IPolicyFactory pollyFactory,
            ILogger<BrokerApi> logger) 
            : base(apiClientConfiguration, httpClientFactory, pollyFactory, logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<BrokerEnrichmentMessage> Post(BrokerEnrichmentMessage message)
        {
            _logger.LogInformation($"get called with broker enrichment message for {Route}");
            var response = await Post(message, Route);
            _logger.LogInformation($"get completed with broker enrichment message for {Route}");

            return response;
        }

        public async Task<bool> HeartBeating(CancellationToken token)
        {
            return await GetHeartbeat(HeartbeatRoute, token);
        }
    }
}
