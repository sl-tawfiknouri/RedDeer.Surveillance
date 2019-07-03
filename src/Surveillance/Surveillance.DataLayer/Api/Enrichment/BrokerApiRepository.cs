using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Network.HttpClient.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RedDeer.Contracts.SurveillanceService.Api.BrokerEnrichment;
using Surveillance.DataLayer.Configuration.Interfaces;

namespace Surveillance.DataLayer.Api.Enrichment
{
    public class BrokerApiRepository : IBrokerApiRepository
    {
        private const string HeartbeatRoute = "api/BrokerEnrichment/Heartbeat";
        private const string Route = "api/BrokerEnrichment/Enrichment/V1";

        private readonly IDataLayerConfiguration _dataLayerConfiguration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger _logger;

        public BrokerApiRepository(
            IDataLayerConfiguration dataLayerConfiguration,
            IHttpClientFactory httpClientFactory,
            ILogger<BrokerApiRepository> logger)
        {
            _dataLayerConfiguration = dataLayerConfiguration ?? throw new ArgumentNullException(nameof(dataLayerConfiguration));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<BrokerEnrichmentMessage> Get(BrokerEnrichmentMessage message)
        {
            _logger.LogInformation($"EnrichmentApiRepository about to make a GET request");

            try
            {
                using (var httpClient = _httpClientFactory.ClientServiceHttpClient(
                    _dataLayerConfiguration.ClientServiceUrl,
                    _dataLayerConfiguration.SurveillanceUserApiAccessToken))
                {
                    var json = JsonConvert.SerializeObject(message);
                    var response = await httpClient.PostAsync(Route, new StringContent(json, Encoding.UTF8, "application/json"));

                    if (response == null
                        || !response.IsSuccessStatusCode)
                    {
                        _logger.LogWarning($"Unsuccessful enrichment api repository GET request. {response?.StatusCode}");

                        return new BrokerEnrichmentMessage();
                    }

                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var deserialisedResponse = JsonConvert.DeserializeObject<BrokerEnrichmentMessage>(jsonResponse);

                    if (deserialisedResponse == null)
                    {
                        _logger.LogError($"EnrichmentApiRepository was unable to deserialise the response");
                        return new BrokerEnrichmentMessage();
                    }

                    _logger.LogInformation($"Enrichment Api Repository returning deserialised GET response");

                    return deserialisedResponse;
                }
            }
            catch (Exception e)
            {
                _logger.LogError("EnrichmentApiRepository: " + e.Message);
            }

            return new BrokerEnrichmentMessage();
        }

        public async Task<bool> HeartBeating(CancellationToken token)
        {
            try
            {
                using (var httpClient = _httpClientFactory.ClientServiceHttpClient(
                    _dataLayerConfiguration.ClientServiceUrl,
                    _dataLayerConfiguration.SurveillanceUserApiAccessToken))
                {
                    var response = await httpClient.GetAsync(HeartbeatRoute, token);

                    if (!response.IsSuccessStatusCode)
                        _logger.LogError($"HEARTBEAT FOR BROKER ENRICHMENT API REPOSITORY NEGATIVE");
                    else
                        _logger.LogInformation($"HEARTBEAT POSITIVE FOR BROKER ENRICHMENT API REPOSITORY");

                    return response.IsSuccessStatusCode;
                }
            }
            catch (Exception)
            {
                _logger.LogError($"HEARTBEAT FOR BROKER ENRICHMENT API REPOSITORY NEGATIVE");
            }

            return false;
        }
    }
}
