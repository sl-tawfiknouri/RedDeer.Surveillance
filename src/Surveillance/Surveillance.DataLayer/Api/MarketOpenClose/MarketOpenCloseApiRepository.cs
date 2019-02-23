using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RedDeer.Contracts.SurveillanceService.Api.Markets;
using Surveillance.DataLayer.Api.MarketOpenClose.Interfaces;
using Surveillance.DataLayer.Configuration.Interfaces;
using Utilities.HttpClient.Interfaces;

namespace Surveillance.DataLayer.Api.MarketOpenClose
{
    public class MarketOpenCloseApiRepository : IMarketOpenCloseApiRepository
    {
        private const string HeartbeatRoute = "api/markets/heartbeat";
        private const string Route = "api/markets/get/v1";
        private readonly IDataLayerConfiguration _dataLayerConfiguration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger _logger;

        public MarketOpenCloseApiRepository(
            IDataLayerConfiguration dataLayerConfiguration,
            IHttpClientFactory httpClientFactory,
            ILogger<MarketOpenCloseApiRepository> logger)
        {
            _dataLayerConfiguration = dataLayerConfiguration ?? throw new ArgumentNullException(nameof(dataLayerConfiguration));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IReadOnlyCollection<ExchangeDto>> Get()
        {
            _logger.LogInformation($"MarketOpenCloseApiRepository GET request initiating");

            try
            {
                using (var httpClient = _httpClientFactory.ClientServiceHttpClient(
                    _dataLayerConfiguration.ClientServiceUrl,
                    _dataLayerConfiguration.SurveillanceUserApiAccessToken))
                {
                    var response = await httpClient.GetAsync(Route);

                    if (response == null
                        || !response.IsSuccessStatusCode)
                    {
                        _logger.LogWarning($"Unsuccessful market open close api repository GET request. {response?.StatusCode}");

                        return new ExchangeDto[0];
                    }

                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var deserialisedResponse = JsonConvert.DeserializeObject<ExchangeDto[]>(jsonResponse);

                    if (deserialisedResponse == null)
                    {
                        _logger.LogWarning($"MarketOpenCloseApiRepository had a null deserialised response");
                    }

                    _logger.LogInformation($"MarketOpenCloseApiRepository returning result");
                    return deserialisedResponse ?? new ExchangeDto[0];
                }
            }
            catch (Exception e)
            {
                _logger.LogError("MarketOpenCloseApiRepository: " + e.Message);
            }

            return new ExchangeDto[0];
        }

        public async Task<bool> HeartBeating(CancellationToken token)
        {
            try
            {
                using (var httpClient = _httpClientFactory.ClientServiceHttpClient(
                    _dataLayerConfiguration.ClientServiceUrl,
                    _dataLayerConfiguration.SurveillanceUserApiAccessToken))
                {
                    var result = await httpClient.GetAsync(HeartbeatRoute, token);

                    if (!result.IsSuccessStatusCode)
                        _logger.LogError($"MarketOpenCloseApiRepository HEARTBEAT NEGATIVE");
                    else
                        _logger.LogInformation($"HEARTBEAT POSITIVE FOR MARKET OPEN CLOSE API REPOSITORY");

                    return result.IsSuccessStatusCode;
                }
            }
            catch (Exception)
            {
                _logger.LogError($"MarketOpenCloseApiRepository HEARTBEAT NEGATIVE");
            }

            return false;
        }
    }
}
