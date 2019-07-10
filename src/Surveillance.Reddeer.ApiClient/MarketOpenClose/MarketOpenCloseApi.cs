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

namespace Surveillance.Reddeer.ApiClient.MarketOpenClose
{
    public class MarketOpenCloseApi : BaseClientServiceApi, IMarketOpenCloseApi
    {
        private const string HeartbeatRoute = "api/markets/heartbeat";
        private const string Route = "api/markets/get/v1";
        private readonly ILogger _logger;

        public MarketOpenCloseApi(
            IApiClientConfiguration apiClientConfiguration,
            IHttpClientFactory httpClientFactory,
            IPolicyFactory pollyFactory,
            ILogger<MarketOpenCloseApi> logger)
            : base(
                apiClientConfiguration,
                httpClientFactory,
                pollyFactory,
                logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IReadOnlyCollection<ExchangeDto>> Get()
        {
            _logger.LogInformation($"get request initiating {Route}");
            var response = await Get<ExchangeDto[]>(Route);
            _logger.LogInformation($"get request completed for {Route}");

            response = response ?? new ExchangeDto[0];

            return response;
        }

        public async Task<bool> HeartBeating(CancellationToken token)
        {
            return await GetHeartbeat(HeartbeatRoute, token);
        }
    }
}
