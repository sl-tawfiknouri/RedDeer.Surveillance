﻿using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RedDeer.Contracts.SurveillanceService.Api.SecurityEnrichment;
using Surveillance.DataLayer.Api.Enrichment.Interfaces;
using Surveillance.DataLayer.Configuration.Interfaces;

namespace Surveillance.DataLayer.Api.Enrichment
{
    public class EnrichmentApiRepository : BaseApiRepository, IEnrichmentApiRepository
    {
        private const string HeartbeatRoute = "api/securityenrichment/heartbeat";
        private const string Route = "api/securityenrichment/enrichment/v1";
        private readonly ILogger _logger;

        public EnrichmentApiRepository(
            IDataLayerConfiguration dataLayerConfiguration,
            ILogger<EnrichmentApiRepository> logger) 
            : base(dataLayerConfiguration)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<SecurityEnrichmentMessage> Get(SecurityEnrichmentMessage message)
        {
            var httpClient = BuildHttpClient();

            _logger.LogInformation($"EnrichmentApiRepository about to make a GET request");

            try
            {
                var json = JsonConvert.SerializeObject(message);
                var response = await httpClient.PostAsync(Route, new StringContent(json, Encoding.UTF8, "application/json"));

                if (response == null
                    || !response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"Unsuccessful enrichment api repository GET request. {response?.StatusCode}");

                    return new SecurityEnrichmentMessage();
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var deserialisedResponse = JsonConvert.DeserializeObject<SecurityEnrichmentMessage>(jsonResponse);

                if (deserialisedResponse == null)
                {
                    _logger.LogError($"EnrichmentApiRepository was unable to deserialise the response");
                    return new SecurityEnrichmentMessage();
                }

                _logger.LogInformation($"Enrichment Api Repository returning deserialised GET response");
                return deserialisedResponse;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }

            return new SecurityEnrichmentMessage();
        }

        public async Task<bool> HeartBeating(CancellationToken token)
        {
            var httpClient = BuildHttpClient();

            var response = await httpClient.GetAsync(HeartbeatRoute, token);

            if (!response.IsSuccessStatusCode)
                _logger.LogError($"HEARTBEAT FOR ENRICHMENT API REPOSITORY NEGATIVE");
            else
                _logger.LogInformation($"HEARTBEAT POSITIVE FOR ENRICHMENT API REPOSITORY");

            return response.IsSuccessStatusCode;
        }
    }
}
