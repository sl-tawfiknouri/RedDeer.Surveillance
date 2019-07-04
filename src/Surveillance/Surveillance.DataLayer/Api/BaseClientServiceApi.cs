using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Network.HttpClient.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PollyFacade.Policies.Interfaces;
using Surveillance.DataLayer.Configuration.Interfaces;

namespace Surveillance.DataLayer.Api
{
    public class BaseClientServiceApi
    {
        private readonly IDataLayerConfiguration _dataLayerConfiguration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IPolicyFactory _policyFactory;
        private readonly ILogger _logger;

        public BaseClientServiceApi(
            IDataLayerConfiguration dataLayerConfiguration,
            IHttpClientFactory httpClientFactory,
            IPolicyFactory policyFactory,
            ILogger<BaseClientServiceApi> logger)
        {
            _dataLayerConfiguration = dataLayerConfiguration ?? throw new ArgumentNullException(nameof(dataLayerConfiguration));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _policyFactory = policyFactory ?? throw new ArgumentNullException(nameof(policyFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<T> Post<T>(T message, string route) where T : new()
        {
            var policy =
                _policyFactory
                    .PolicyTimeoutGeneric<HttpResponseMessage>(
                        TimeSpan.FromMinutes(2),
                        i => !i.IsSuccessStatusCode,
                        3, 
                        TimeSpan.FromSeconds(15));

            return await Post(message, route, policy);
        }

        public async Task<T> Post<T>(T message, string route, IPolicy<HttpResponseMessage> policy) where T: new()
        {
            _logger.LogInformation($"about to make a GET request to {route}");

            try
            {
                using (var httpClient = _httpClientFactory.ClientServiceHttpClient(
                    _dataLayerConfiguration.ClientServiceUrl,
                    _dataLayerConfiguration.SurveillanceUserApiAccessToken))
                {
                    var json = JsonConvert.SerializeObject(message);

                    HttpResponseMessage response = null;
                    await policy.ExecuteAsync(async () =>
                    {
                        _logger.LogInformation($"policy about to call post");
                        response = await httpClient.PostAsync(route, new StringContent(json, Encoding.UTF8, "application/json"));
                        _logger.LogInformation($"policy received post response or timed out");
                        return response;
                    });

                    if (response == null
                        || !response.IsSuccessStatusCode)
                    {
                        _logger.LogWarning($"unsuccessful repository GET request to {route}. {response?.StatusCode}");

                        return new T();
                    }

                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var deserialisedResponse = JsonConvert.DeserializeObject<T>(jsonResponse);

                    if (deserialisedResponse == null)
                    {
                        _logger.LogError($"was unable to deserialise the response at {route} {jsonResponse}");
                        return new T();
                    }

                    _logger.LogInformation($"returning deserialised GET response from {route}");

                    return deserialisedResponse;
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception in {nameof(BaseClientServiceApi)} {e.Message} {e.InnerException?.Message}");
            }

            return new T();
        }
        
        public async Task<T> Get<T>(string route) where T : class
        {
            _logger.LogInformation($"get request initiating for {route}");

            var policy =
                _policyFactory
                    .PolicyTimeoutGeneric<HttpResponseMessage>(
                        TimeSpan.FromMinutes(2),
                        i => !i.IsSuccessStatusCode,
                        3,
                        TimeSpan.FromSeconds(15));

            return await Get<T>(route, policy);
        }

        public async Task<T> Get<T>(string route, IPolicy<HttpResponseMessage> policy) where T : class
        {
            _logger.LogInformation($"get request initiating for {route}");

            try
            {
                using (var httpClient = _httpClientFactory.ClientServiceHttpClient(
                    _dataLayerConfiguration.ClientServiceUrl,
                    _dataLayerConfiguration.SurveillanceUserApiAccessToken))
                {
                    HttpResponseMessage response = null;
                    await policy.ExecuteAsync(async () =>
                    {
                        _logger.LogInformation($"policy about to call get at {route}");
                        response = await httpClient.GetAsync(route);
                        _logger.LogInformation($"policy received post response or timed out for {route}");
                        return response;
                    });
                    
                    if (response == null
                        || !response.IsSuccessStatusCode)
                    {
                        _logger.LogWarning($"failed get request at {route} {response?.StatusCode}");
                        return null;
                    }

                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var deserialisedResponse = JsonConvert.DeserializeObject<T>(jsonResponse);

                    if (deserialisedResponse == null)
                    {
                        _logger.LogWarning($"had a null deserialised response for {route}");
                    }

                    _logger.LogInformation($"returning get result from {route}");
                    return deserialisedResponse;
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"exception on get request to {route} {e.Message} {e.InnerException?.Message}");
            }

            return null;
        }

        public async Task<bool> GetHeartbeat(string route, CancellationToken token)
        {
            var policy =
                _policyFactory
                    .PolicyTimeoutGeneric<HttpResponseMessage>(
                        TimeSpan.FromSeconds(10),
                        i => !i.IsSuccessStatusCode,
                        2,
                        TimeSpan.FromSeconds(2));

            return await GetHeartbeat(route, token, policy);
        }

        public async Task<bool> GetHeartbeat(string route, CancellationToken token, IPolicy<HttpResponseMessage> policy)
        {
            _logger.LogInformation($"about to make a get heartbeat request to {route}");

            try
            {
                using (var httpClient = _httpClientFactory.ClientServiceHttpClient(
                    _dataLayerConfiguration.ClientServiceUrl,
                    _dataLayerConfiguration.SurveillanceUserApiAccessToken))
                {
                    var response = await httpClient.GetAsync(route, token);

                    if (!response.IsSuccessStatusCode)
                        _logger.LogError($"heartbeat for {route} was negative");
                    else
                        _logger.LogInformation($"heartbeat for {route} was positive");

                    return response.IsSuccessStatusCode;
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"heartbeat for {route} was negative {e.Message} {e.InnerException?.Message}");
            }

            return false;
        }
    }
}
