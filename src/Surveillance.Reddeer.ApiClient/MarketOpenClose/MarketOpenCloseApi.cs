namespace Surveillance.Reddeer.ApiClient.MarketOpenClose
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Infrastructure.Network.HttpClient.Interfaces;

    using Microsoft.Extensions.Logging;

    using PollyFacade.Policies.Interfaces;

    using RedDeer.Contracts.SurveillanceService.Api.Markets;

    using Surveillance.Reddeer.ApiClient.Configuration.Interfaces;
    using Surveillance.Reddeer.ApiClient.MarketOpenClose.Interfaces;

    /// <summary>
    /// The market open close.
    /// </summary>
    public class MarketOpenCloseApi : BaseClientServiceApi, IMarketOpenCloseApi
    {
        /// <summary>
        /// The heartbeat route.
        /// </summary>
        private const string HeartbeatRoute = "api/markets/heartbeat";

        /// <summary>
        /// The route.
        /// </summary>
        private const string Route = "api/markets/get/v1";

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="MarketOpenCloseApi"/> class.
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
        public MarketOpenCloseApi(
            IApiClientConfiguration apiClientConfiguration,
            IHttpClientFactory httpClientFactory,
            IPolicyFactory pollyFactory,
            ILogger<MarketOpenCloseApi> logger)
            : base(apiClientConfiguration, httpClientFactory, pollyFactory, logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// The get async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<IReadOnlyCollection<ExchangeDto>> GetAsync()
        {
            this.logger.LogInformation($"get request initiating {Route}");
            var response = await this.GetAsync<ExchangeDto[]>(Route);
            this.logger.LogInformation($"get request completed for {Route}");

            response = response ?? new ExchangeDto[0];

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
            return await this.GetHeartbeatAsync(HeartbeatRoute, token);
        }
    }
}