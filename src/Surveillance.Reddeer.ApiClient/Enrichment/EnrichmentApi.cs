namespace Surveillance.Reddeer.ApiClient.Enrichment
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Infrastructure.Network.HttpClient.Interfaces;

    using Microsoft.Extensions.Logging;

    using PollyFacade.Policies.Interfaces;

    using RedDeer.Contracts.SurveillanceService.Api.SecurityEnrichment;

    using Surveillance.Reddeer.ApiClient.Configuration.Interfaces;
    using Surveillance.Reddeer.ApiClient.Enrichment.Interfaces;

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
            : base(apiClientConfiguration, httpClientFactory, policyFactory, logger)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> HeartBeating(CancellationToken token)
        {
            this._logger.LogInformation($"about to make a heartbeat request to {HeartbeatRoute}");
            var response = await this.GetHeartbeat(HeartbeatRoute, token);
            this._logger.LogInformation($"about to make a heartbeat request to {HeartbeatRoute}");

            return response;
        }

        public async Task<SecurityEnrichmentMessage> Post(SecurityEnrichmentMessage message)
        {
            this._logger.LogInformation($"about to make a request to {Route}");
            var response = await this.Post(message, Route);
            this._logger.LogInformation($"completed a request to {Route}");

            return response;
        }
    }
}