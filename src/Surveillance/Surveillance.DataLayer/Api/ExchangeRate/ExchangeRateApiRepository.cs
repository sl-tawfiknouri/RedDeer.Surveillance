using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RedDeer.Contracts.SurveillanceService.Api.ExchangeRate;
using Surveillance.DataLayer.Api.ExchangeRate.Interfaces;
using Surveillance.DataLayer.Configuration.Interfaces;
using Utilities.HttpClient.Interfaces;

namespace Surveillance.DataLayer.Api.ExchangeRate
{
    public class ExchangeRateApiRepository : IExchangeRateApiRepository
    {
        private const string HeartbeatRoute = "api/exchangerates/heartbeat";
        private const string Route = "api/exchangerates/get/v1";
        private readonly IDataLayerConfiguration _dataLayerConfiguration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger _logger;

        public ExchangeRateApiRepository(
            IDataLayerConfiguration dataLayerConfiguration,
            IHttpClientFactory httpClientFactory,
            ILogger<ExchangeRateApiRepository> logger)
        {
            _dataLayerConfiguration = dataLayerConfiguration ?? throw new ArgumentNullException(nameof(dataLayerConfiguration));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IDictionary<DateTime, IReadOnlyCollection<ExchangeRateDto>>> Get(DateTime commencement, DateTime termination)
        {
            _logger.LogInformation($"ExchangeRateApiRepository GET request for date {commencement} to {termination}");

            try
            {
                // US date format as that's default when deserialising on a UK machine as asp.net mvc core
                // If this starts breaking the culture of the machine the client service is on would be worth investigating
                // for posterity this url worked @ 29/09/2018 (http://localhost:8080/api/exchangerates/get/v1?commencement=09/27/2017&termination=09/26/2018)

                var routeWithQString = $"{Route}?commencement={commencement.ToString("MM/dd/yyyy")}&termination={termination.ToString("MM/dd/yyyy")}";

                using (var httpClient = _httpClientFactory.ClientServiceHttpClient(
                    _dataLayerConfiguration.ClientServiceUrl,
                    _dataLayerConfiguration.SurveillanceUserApiAccessToken))
                {
                    var response = await httpClient.GetAsync(routeWithQString);

                    if (response == null
                        || !response.IsSuccessStatusCode)
                    {
                        _logger.LogWarning($"Unsuccessful exchange rate api repository GET request. {response?.StatusCode}");

                        return new Dictionary<DateTime, IReadOnlyCollection<ExchangeRateDto>>();
                    }

                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var deserialisedResponse = JsonConvert.DeserializeObject<ExchangeRateDto[]>(jsonResponse);

                    if (deserialisedResponse == null
                        || !deserialisedResponse.Any())
                    {
                        _logger.LogWarning($"ExchangeRateApiRepository GET request returned a null or empty response");
                        return new Dictionary<DateTime, IReadOnlyCollection<ExchangeRateDto>>();
                    }

                    var result = deserialisedResponse.GroupBy(dr => dr.DateTime).ToDictionary(i => i.Key, i => i.ToList() as IReadOnlyCollection<ExchangeRateDto>);

                    _logger.LogInformation($"ExchangeRateApiRepository GET request returning results");

                    return result;
                }
            }
            catch (Exception e)
            {
                _logger.LogError("ExchangeRateApiRepository: " + e.Message);
            }

            return new Dictionary<DateTime, IReadOnlyCollection<ExchangeRateDto>>();
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
                        _logger.LogError($"ExchangeRateApiRepository HEARTBEAT NEGATIVE FOR API END POINT");
                    else
                    {
                        _logger.LogInformation($"HEARTBEAT POSITIVE FOR EXCHANGE RATE API REPOSITORY");
                    }

                    return response.IsSuccessStatusCode;
                }
            }
            catch (Exception)
            {
                _logger.LogError($"ExchangeRateApiRepository HEARTBEAT NEGATIVE FOR API END POINT");
            }

            return false;
        }
    }
}