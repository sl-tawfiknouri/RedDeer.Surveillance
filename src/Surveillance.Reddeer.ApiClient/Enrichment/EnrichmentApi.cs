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

    /// <summary>
    /// The enrichment.
    /// </summary>
    public class EnrichmentApi : BaseClientServiceApi, IEnrichmentApi
    {
        /// <summary>
        /// The heartbeat route.
        /// </summary>
        private const string HeartbeatRoute = "api/securityenrichment/heartbeat";

        /// <summary>
        /// The route.
        /// </summary>
        private const string Route = "api/securityenrichment/enrichment/v2";

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnrichmentApi"/> class.
        /// </summary>
        /// <param name="apiClientConfiguration">
        /// The client configuration.
        /// </param>
        /// <param name="httpClientFactory">
        /// The http client factory.
        /// </param>
        /// <param name="policyFactory">
        /// The policy factory.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public EnrichmentApi(
            IApiClientConfiguration apiClientConfiguration,
            IHttpClientFactory httpClientFactory,
            IPolicyFactory policyFactory,
            ILogger<EnrichmentApi> logger)
            : base(apiClientConfiguration, httpClientFactory, policyFactory, logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// The heart beating async.
        /// </summary>
        /// <param name="token">
        /// The token.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<bool> HeartBeatingAsync(CancellationToken token)
        {
            this.logger.LogInformation($"about to make a heartbeat request to {HeartbeatRoute}");
            var response = await this.GetHeartbeatAsync(HeartbeatRoute, token).ConfigureAwait(false);
            this.logger.LogInformation($"about to make a heartbeat request to {HeartbeatRoute}");

            return response;
        }

        /// <summary>
        /// The post async.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<SecurityEnrichmentMessage> PostAsync(SecurityEnrichmentMessage message)
        {
            this.logger.LogInformation($"about to make a request to {Route}");
            var response = await this.PostAsync(message, Route).ConfigureAwait(false);
            this.logger.LogInformation($"completed a request to {Route}");

            return response;
        }
    }
}