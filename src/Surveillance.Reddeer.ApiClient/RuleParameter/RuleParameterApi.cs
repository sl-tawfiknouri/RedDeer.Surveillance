namespace Surveillance.Reddeer.ApiClient.RuleParameter
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Infrastructure.Network.HttpClient.Interfaces;

    using Microsoft.Extensions.Logging;

    using PollyFacade.Policies.Interfaces;

    using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;

    using Surveillance.Reddeer.ApiClient.Configuration.Interfaces;
    using Surveillance.Reddeer.ApiClient.RuleParameter.Interfaces;

    /// <summary>
    /// The rule parameter.
    /// </summary>
    public class RuleParameterApi : BaseClientServiceApi, IRuleParameterApi
    {
        /// <summary>
        /// The heartbeat route.
        /// </summary>
        private const string HeartbeatRoute = "api/surveillanceruleparameter/heartbeat";

        /// <summary>
        /// The route v 2.
        /// </summary>
        private const string RouteV2 = "api/surveillanceruleparameter/get/v2";

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="RuleParameterApi"/> class.
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
        public RuleParameterApi(
            IApiClientConfiguration apiClientConfiguration,
            IHttpClientFactory httpClientFactory,
            IPolicyFactory pollyFactory,
            ILogger<RuleParameterApi> logger)
            : base(apiClientConfiguration, httpClientFactory, pollyFactory, logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// The get async.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<RuleParameterDto> GetAsync(string id)
        {
            var routeId = $"{RouteV2}/{id}";

            this.logger.LogInformation($"http client making get request to {routeId}");
            var response = await this.GetAsync<RuleParameterDto>(routeId).ConfigureAwait(false);
            this.logger.LogInformation($"http client making get request to {routeId}");

            return response;
        }

        /// <summary>
        /// The get async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<RuleParameterDto> GetAsync()
        {
            this.logger.LogInformation($"http client making get request to {RouteV2}");
            var response = await this.GetAsync<RuleParameterDto>(RouteV2).ConfigureAwait(false);
            this.logger.LogInformation($"http client making get request to {RouteV2}");

            return response;
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
            return await this.GetHeartbeatAsync(HeartbeatRoute, token).ConfigureAwait(false);
        }
    }
}