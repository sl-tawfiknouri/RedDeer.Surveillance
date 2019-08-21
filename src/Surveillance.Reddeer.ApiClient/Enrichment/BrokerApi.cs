namespace Surveillance.Reddeer.ApiClient.Enrichment
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Infrastructure.Network.HttpClient.Interfaces;

    using Microsoft.Extensions.Logging;

    using PollyFacade.Policies.Interfaces;

    using RedDeer.Contracts.SurveillanceService.Api.BrokerEnrichment;

    using Surveillance.Reddeer.ApiClient.Configuration.Interfaces;
    using Surveillance.Reddeer.ApiClient.Enrichment.Interfaces;

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
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> HeartBeating(CancellationToken token)
        {
            return await this.GetHeartbeat(HeartbeatRoute, token);
        }

        public async Task<BrokerEnrichmentMessage> Post(BrokerEnrichmentMessage message)
        {
            this._logger.LogInformation($"get called with broker enrichment message for {Route}");
            var response = await this.Post(message, Route);
            this._logger.LogInformation($"get completed with broker enrichment message for {Route}");

            return response;
        }
    }
}