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

    /// <summary>
    /// The broker.
    /// </summary>
    public class BrokerApi : BaseClientServiceApi, IBrokerApi
    {
        /// <summary>
        /// The heartbeat route.
        /// </summary>
        private const string HeartbeatRoute = "api/BrokerEnrichment/Heartbeat";

        /// <summary>
        /// The route.
        /// </summary>
        private const string Route = "api/BrokerEnrichment/Enrichment/V1";

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="BrokerApi"/> class.
        /// </summary>
        /// <param name="apiClientConfiguration">
        /// The client configuration.
        /// </param>
        /// <param name="httpClientFactory">
        /// The http client factory.
        /// </param>
        /// <param name="pollyFactory">
        /// The polly factory.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public BrokerApi(
            IApiClientConfiguration apiClientConfiguration,
            IHttpClientFactory httpClientFactory,
            IPolicyFactory pollyFactory,
            ILogger<BrokerApi> logger)
            : base(apiClientConfiguration, httpClientFactory, pollyFactory, logger)
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
            return await this.GetHeartbeatAsync(HeartbeatRoute, token);
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
        public async Task<BrokerEnrichmentMessage> PostAsync(BrokerEnrichmentMessage message)
        {
            this.logger.LogInformation($"get called with broker enrichment message for {Route}");
            var response = await this.PostAsync(message, Route);
            this.logger.LogInformation($"get completed with broker enrichment message for {Route}");

            return response;
        }
    }
}