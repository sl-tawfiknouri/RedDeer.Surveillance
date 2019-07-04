using System;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Network.HttpClient.Interfaces;
using Microsoft.Extensions.Logging;
using PollyFacade.Policies.Interfaces;
using RedDeer.Contracts.SurveillanceService.Api.SecurityEnrichment;
using Surveillance.Reddeer.ApiClient.Configuration.Interfaces;
using Surveillance.Reddeer.ApiClient.Enrichment.Interfaces;

namespace Surveillance.Reddeer.ApiClient.Enrichment
{
    public class EnrichmentApi : BaseClientServiceApi, IEnrichmentApi
    {
        private const string HeartbeatRoute = "api/securityenrichment/heartbeat";
        private const string Route = "api/securityenrichment/enrichment/v2";
        private readonly ILogger _logger;

        public EnrichmentApi(
            IApiClientConfiguration apiClientConfiguration,
            IHttpClientFactory httpClientFactory,
            IPolicyFactory policyFactory,
            ILogger<EnrichmentApi> logger)
        : base(
            apiClientConfiguration,
            httpClientFactory,
            policyFactory,
            logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<SecurityEnrichmentMessage> Post(SecurityEnrichmentMessage message)
        {
            _logger.LogInformation($"about to make a request to {Route}");
            var response = await Post(message, Route);
            _logger.LogInformation($"completed a request to {Route}");

            return response;
        }

        public async Task<bool> HeartBeating(CancellationToken token)
        {
            _logger.LogInformation($"about to make a heartbeat request to {HeartbeatRoute}");
            var response = await GetHeartbeat(HeartbeatRoute, token);
            _logger.LogInformation($"about to make a heartbeat request to {HeartbeatRoute}");

            return response;
        }
    }
}
