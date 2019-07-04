﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Network.HttpClient.Interfaces;
using Microsoft.Extensions.Logging;
using PollyFacade.Policies.Interfaces;
using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;
using Surveillance.DataLayer.Api.RuleParameter.Interfaces;
using Surveillance.DataLayer.Configuration.Interfaces;

namespace Surveillance.DataLayer.Api.RuleParameter
{
    public class RuleParameterApiRepository : BaseClientServiceApi, IRuleParameterApiRepository
    {
        private const string HeartbeatRoute = "api/surveillanceruleparameter/heartbeat";
        private const string RouteV2 = "api/surveillanceruleparameter/get/v2";

        private readonly ILogger _logger;

        public RuleParameterApiRepository(
            IDataLayerConfiguration dataLayerConfiguration,
            IHttpClientFactory httpClientFactory,
            IPolicyFactory pollyFactory,
            ILogger<RuleParameterApiRepository> logger)
            : base(
                dataLayerConfiguration,
                httpClientFactory,
                pollyFactory,
                logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<RuleParameterDto> Get(string id)
        {
            var routeId = $"{RouteV2}/{id}";

            _logger.LogInformation($"httpclient making get request to {routeId}");
            var response = await Get<RuleParameterDto>(routeId);
            _logger.LogInformation($"httpclient making get request to {routeId}");

            return response;
        }

        public async Task<RuleParameterDto> Get()
        {
            _logger.LogInformation($"httpclient making get request to {RouteV2}");
            var response = await Get<RuleParameterDto>(RouteV2);
            _logger.LogInformation($"httpclient making get request to {RouteV2}");

            return response;
        }

        public async Task<bool> HeartBeating(CancellationToken token)
        {
            return await GetHeartbeat(HeartbeatRoute, token);
        }
    }
}
