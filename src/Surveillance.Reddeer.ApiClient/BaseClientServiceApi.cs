namespace Surveillance.Reddeer.ApiClient
{
    using System;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using Infrastructure.Network.HttpClient.Interfaces;

    using Microsoft.Extensions.Logging;

    using Newtonsoft.Json;

    using PollyFacade.Policies.Interfaces;

    using Surveillance.Reddeer.ApiClient.Configuration.Interfaces;

    public class BaseClientServiceApi
    {
        private readonly IApiClientConfiguration _apiClientConfiguration;

        private readonly IHttpClientFactory _httpClientFactory;

        private readonly ILogger _logger;

        private readonly IPolicyFactory _policyFactory;

        public BaseClientServiceApi(
            IApiClientConfiguration apiClientConfiguration,
            IHttpClientFactory httpClientFactory,
            IPolicyFactory policyFactory,
            ILogger<BaseClientServiceApi> logger)
        {
            this._apiClientConfiguration =
                apiClientConfiguration ?? throw new ArgumentNullException(nameof(apiClientConfiguration));
            this._httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            this._policyFactory = policyFactory ?? throw new ArgumentNullException(nameof(policyFactory));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<T> Get<T>(string route)
            where T : class
        {
            this._logger.LogInformation($"get request initiating for {route}");

            var policy = this._policyFactory.PolicyTimeoutGeneric<HttpResponseMessage>(
                TimeSpan.FromMinutes(2),
                i => !i.IsSuccessStatusCode,
                3,
                TimeSpan.FromSeconds(15));

            return await this.Get<T>(route, policy);
        }

        public async Task<T> Get<T>(string route, IPolicy<HttpResponseMessage> policy)
            where T : class
        {
            this._logger.LogInformation($"get request initiating for {route}");

            try
            {
                using (var httpClient = this._httpClientFactory.ClientServiceHttpClient(
                    this._apiClientConfiguration.ClientServiceUrl,
                    this._apiClientConfiguration.SurveillanceUserApiAccessToken))
                {
                    HttpResponseMessage response = null;
                    await policy.ExecuteAsync(
                        async () =>
                            {
                                this._logger.LogInformation($"policy about to call get at {route}");
                                response = await httpClient.GetAsync(route);
                                this._logger.LogInformation($"policy received post response or timed out for {route}");
                                return response;
                            });

                    if (response == null || !response.IsSuccessStatusCode)
                    {
                        this._logger.LogWarning($"failed get request at {route} {response?.StatusCode}");
                        return null;
                    }

                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var deserialisedResponse = JsonConvert.DeserializeObject<T>(jsonResponse);

                    if (deserialisedResponse == null)
                        this._logger.LogWarning($"had a null deserialised response for {route}");

                    this._logger.LogInformation($"returning get result from {route}");
                    return deserialisedResponse;
                }
            }
            catch (Exception e)
            {
                this._logger.LogError($"exception on get request to {route} {e.Message} {e.InnerException?.Message}");
            }

            return null;
        }

        public async Task<bool> GetHeartbeat(string route, CancellationToken token)
        {
            var policy = this._policyFactory.PolicyTimeoutGeneric<HttpResponseMessage>(
                TimeSpan.FromSeconds(10),
                i => !i.IsSuccessStatusCode,
                2,
                TimeSpan.FromSeconds(2));

            return await this.GetHeartbeat(route, token, policy);
        }

        public async Task<bool> GetHeartbeat(string route, CancellationToken token, IPolicy<HttpResponseMessage> policy)
        {
            this._logger.LogInformation($"about to make a get heartbeat request to {route}");

            try
            {
                using (var httpClient = this._httpClientFactory.ClientServiceHttpClient(
                    this._apiClientConfiguration.ClientServiceUrl,
                    this._apiClientConfiguration.SurveillanceUserApiAccessToken))
                {
                    var response = await httpClient.GetAsync(route, token);

                    if (!response.IsSuccessStatusCode) this._logger.LogError($"heartbeat for {route} was negative");
                    else this._logger.LogInformation($"heartbeat for {route} was positive");

                    return response.IsSuccessStatusCode;
                }
            }
            catch (Exception e)
            {
                this._logger.LogError($"heartbeat for {route} was negative {e.Message} {e.InnerException?.Message}");
            }

            return false;
        }

        public async Task<T> Post<T>(T message, string route)
            where T : new()
        {
            var policy = this._policyFactory.PolicyTimeoutGeneric<HttpResponseMessage>(
                TimeSpan.FromMinutes(2),
                i => !i.IsSuccessStatusCode,
                3,
                TimeSpan.FromSeconds(15));

            return await this.Post(message, route, policy);
        }

        public async Task<T> Post<T>(T message, string route, IPolicy<HttpResponseMessage> policy)
            where T : new()
        {
            this._logger.LogInformation($"about to make a GET request to {route}");

            try
            {
                using (var httpClient = this._httpClientFactory.ClientServiceHttpClient(
                    this._apiClientConfiguration.ClientServiceUrl,
                    this._apiClientConfiguration.SurveillanceUserApiAccessToken))
                {
                    var json = JsonConvert.SerializeObject(message);

                    HttpResponseMessage response = null;
                    await policy.ExecuteAsync(
                        async () =>
                            {
                                this._logger.LogInformation("policy about to call post");
                                response = await httpClient.PostAsync(
                                               route,
                                               new StringContent(json, Encoding.UTF8, "application/json"));
                                this._logger.LogInformation("policy received post response or timed out");
                                return response;
                            });

                    if (response == null || !response.IsSuccessStatusCode)
                    {
                        this._logger.LogWarning(
                            $"unsuccessful repository GET request to {route}. {response?.StatusCode}");

                        return new T();
                    }

                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var deserialisedResponse = JsonConvert.DeserializeObject<T>(jsonResponse);

                    if (deserialisedResponse == null)
                    {
                        this._logger.LogError($"was unable to deserialise the response at {route} {jsonResponse}");
                        return new T();
                    }

                    this._logger.LogInformation($"returning deserialised GET response from {route}");

                    return deserialisedResponse;
                }
            }
            catch (Exception e)
            {
                this._logger.LogError(
                    $"Exception in {nameof(BaseClientServiceApi)} {e.Message} {e.InnerException?.Message}");
            }

            return new T();
        }
    }
}