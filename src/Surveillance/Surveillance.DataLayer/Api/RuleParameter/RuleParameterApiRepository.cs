﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;
using Surveillance.DataLayer.Api.RuleParameter.Interfaces;
using Surveillance.DataLayer.Configuration.Interfaces;

namespace Surveillance.DataLayer.Api.RuleParameter
{
    public class RuleParameterApiRepository : BaseApiRepository, IRuleParameterApiRepository
    {
        private const string HeartbeatRoute = "api/surveillanceruleparameter/heartbeat";
        private const string RouteV2 = "api/surveillanceruleparameter/get/v2";

        private readonly ILogger _logger;

        public RuleParameterApiRepository(
            IDataLayerConfiguration dataLayerConfiguration,
            ILogger<RuleParameterApiRepository> logger) 
            : base(dataLayerConfiguration, logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<RuleParameterDto> Get(string id)
        {
            var httpClient = BuildHttpClient();
            _logger.LogInformation($"RuleParameterApiRepository GET by ID {id} request initiating");

            try
            {
                var response = await httpClient.GetAsync($"{RouteV2}/{id}");

                if (response == null
                    || !response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"Unsuccessful rule parameter api repository GET by id {id} request. {response?.StatusCode}");

                    return new RuleParameterDto();
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var deserialisedResponse = JsonConvert.DeserializeObject<RuleParameterDto>(jsonResponse);

                if (deserialisedResponse == null)
                {
                    _logger.LogWarning($"RuleParameterApiRepository has a null deserialised response for GET by id {id} request");
                }

                _logger.LogInformation($"RuleParameterApiRepository GET by ID {id} request returning response");

                return deserialisedResponse ?? new RuleParameterDto();
            }
            catch (Exception e)
            {
                _logger.LogError("ruleParameterApiRepository: " + e.Message);
            }

            return new RuleParameterDto();
        }

        public async Task<RuleParameterDto> Get()
        {
            var httpClient = BuildHttpClient();
            _logger.LogInformation($"RuleParameterApiRepository GET request initiating");

            try
            {
                var response = await httpClient.GetAsync(RouteV2);

                if (response == null
                    || !response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"Unsuccessful rule parameter api repository GET request. {response?.StatusCode}");

                    return new RuleParameterDto();
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var deserialisedResponse = JsonConvert.DeserializeObject<RuleParameterDto>(jsonResponse);

                if (deserialisedResponse == null)
                {
                    _logger.LogWarning($"RuleParameterApiRepository has a null deserialised response for GET request");
                }

                _logger.LogInformation($"RuleParameterApiRepository GET request returning response");

                return deserialisedResponse ?? new RuleParameterDto();
            }
            catch (Exception e)
            {
                _logger.LogError("RuleParameterApiRepository: " + e.Message);
            }

            return new RuleParameterDto();
        }

        public async Task<bool> HeartBeating(CancellationToken token)
        {
            try
            {
                var client = BuildHttpClient();
                var result = await client.GetAsync(HeartbeatRoute, token);

                if (!result.IsSuccessStatusCode)
                    _logger.LogError($"RuleParameterApiRepository HEARTBEAT NEGATIVE");
                else
                    _logger.LogInformation($"HEARTBEAT POSITIVE FOR RULE PARAMETER API REPOSITORY");

                return result.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                _logger.LogError($"RuleParameterApiRepository HEARTBEAT NEGATIVE");
            }

            return false;
        }
    }
}
