using System;
using RedDeer.Contracts.SurveillanceService.Api.Markets;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Surveillance.DataLayer.Api.Interfaces;
using System.Net;
using Surveillance.DataLayer.Configuration.Interfaces;

namespace Surveillance.DataLayer.Api
{
    public class MarketOpenCloseApiRepository : IMarketOpenCloseApiRepository
    {
        private const string ApiAuthHeader = "authtoken";
        private const string Route = "api/markets/get/v1";
        private readonly IDataLayerConfiguration _dataLayerConfiguration;
        private readonly ILogger<MarketOpenCloseApiRepository> _logger;

        public MarketOpenCloseApiRepository(
            IDataLayerConfiguration dataLayerConfiguration, 
            ILogger<MarketOpenCloseApiRepository> logger)
        {
            _dataLayerConfiguration =
                dataLayerConfiguration
                ?? throw new ArgumentNullException(nameof(dataLayerConfiguration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IReadOnlyCollection<ExchangeDto>> Get()
        {
            var httpClient = BuildHttpClient();

            try
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

                return deserialisedResponse ?? new ExchangeDto[0];
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }

            return new ExchangeDto[0];
        }

        private HttpClient BuildHttpClient()
        {
            if (string.IsNullOrWhiteSpace(_dataLayerConfiguration.ClientServiceUrl))
            {
                throw new ArgumentOutOfRangeException(nameof(_dataLayerConfiguration.ClientServiceUrl));
            }

            var cookies = new CookieContainer();
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true,
                UseCookies = true,
                CookieContainer = cookies
            };

            var httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(_dataLayerConfiguration.ClientServiceUrl)
            };

            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(
                    ApiAuthHeader,
                    _dataLayerConfiguration.SurveillanceUserApiAccessToken);

            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return httpClient;
        }
    }
}
