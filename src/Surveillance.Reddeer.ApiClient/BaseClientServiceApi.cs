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

    /// <summary>
    /// The base client service.
    /// </summary>
    public class BaseClientServiceApi
    {
        /// <summary>
        /// The api client configuration.
        /// </summary>
        private readonly IApiClientConfiguration apiClientConfiguration;

        /// <summary>
        /// The http client factory.
        /// </summary>
        private readonly IHttpClientFactory httpClientFactory;

        /// <summary>
        /// The policy factory.
        /// </summary>
        private readonly IPolicyFactory policyFactory;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseClientServiceApi"/> class.
        /// </summary>
        /// <param name="apiClientConfiguration">
        /// The client configuration.
        /// </param>
        /// <param name="httpClientFactory">
        /// The http client factory.
        /// </param>
        /// <param name="policyFactory">
        /// The policy factory.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public BaseClientServiceApi(
            IApiClientConfiguration apiClientConfiguration,
            IHttpClientFactory httpClientFactory,
            IPolicyFactory policyFactory,
            ILogger<BaseClientServiceApi> logger)
        {
            this.apiClientConfiguration = apiClientConfiguration ?? throw new ArgumentNullException(nameof(apiClientConfiguration));
            this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            this.policyFactory = policyFactory ?? throw new ArgumentNullException(nameof(policyFactory));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// The get async.
        /// </summary>
        /// <param name="route">
        /// The route.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<T> GetAsync<T>(string route)
            where T : class
        {
            this.logger.LogInformation($"get request initiating for {route}");

            var policy = this.policyFactory.PolicyTimeoutGeneric<HttpResponseMessage>(
                TimeSpan.FromMinutes(30),
                i => !i.IsSuccessStatusCode,
                3,
                TimeSpan.FromSeconds(15));

            return await this.GetAsync<T>(route, policy);
        }

        /// <summary>
        /// The get async.
        /// </summary>
        /// <param name="route">
        /// The route.
        /// </param>
        /// <param name="policy">
        /// The policy.
        /// </param>
        /// <typeparam name="T">
        /// any class
        /// </typeparam>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<T> GetAsync<T>(string route, IPolicy<HttpResponseMessage> policy)
            where T : class
        {
            this.logger.LogInformation($"get request initiating for {route}");

            try
            {
                using (var httpClient = this.httpClientFactory.ClientServiceHttpClient(
                    this.apiClientConfiguration.ClientServiceUrl,
                    this.apiClientConfiguration.SurveillanceUserApiAccessToken))
                {
                    HttpResponseMessage response = null;
                    await policy.ExecuteAsync(async () =>
                            {
                                this.logger.LogInformation($"policy about to call get at {route}");
                                response = await httpClient.GetAsync(route);
                                this.logger.LogInformation($"policy received post response or timed out for {route}");
                                return response;
                            });

                    if (response == null || !response.IsSuccessStatusCode)
                    {
                        this.logger.LogWarning($"failed get request at {route} {response?.StatusCode}");
                        return null;
                    }

                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var deserialisedResponse = JsonConvert.DeserializeObject<T>(jsonResponse);

                    if (deserialisedResponse == null)
                    {
                        this.logger.LogWarning($"had a null deserialised response for {route}");
                    }

                    this.logger.LogInformation($"returning get result from {route}");
                    return deserialisedResponse;
                }
            }
            catch (Exception e)
            {
                this.logger.LogError($"exception on get request to {route} {e.Message} {e.InnerException?.Message}");
            }

            return null;
        }

        /// <summary>
        /// The get heartbeat async.
        /// </summary>
        /// <param name="route">
        /// The route.
        /// </param>
        /// <param name="token">
        /// The token.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<bool> GetHeartbeatAsync(string route, CancellationToken token)
        {
            var policy = this.policyFactory.PolicyTimeoutGeneric<HttpResponseMessage>(
                TimeSpan.FromSeconds(10),
                i => !i.IsSuccessStatusCode,
                2,
                TimeSpan.FromSeconds(2));

            return await this.GetHeartbeatAsync(route, token, policy);
        }

        /// <summary>
        /// The get heartbeat async.
        /// </summary>
        /// <param name="route">
        /// The route.
        /// </param>
        /// <param name="token">
        /// The token.
        /// </param>
        /// <param name="policy">
        /// The policy.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<bool> GetHeartbeatAsync(string route, CancellationToken token, IPolicy<HttpResponseMessage> policy)
        {
            this.logger.LogInformation($"about to make a get heartbeat request to {route}");

            try
            {
                using (var httpClient = this.httpClientFactory.ClientServiceHttpClient(
                    this.apiClientConfiguration.ClientServiceUrl,
                    this.apiClientConfiguration.SurveillanceUserApiAccessToken))
                {
                    var response = await httpClient.GetAsync(route, token);

                    if (!response.IsSuccessStatusCode) this.logger.LogError($"heartbeat for {route} was negative");
                    else this.logger.LogInformation($"heartbeat for {route} was positive");

                    return response.IsSuccessStatusCode;
                }
            }
            catch (Exception e)
            {
                this.logger.LogError($"heartbeat for {route} was negative {e.Message} {e.InnerException?.Message}");
            }

            return false;
        }

        /// <summary>
        /// The post async.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="route">
        /// The route.
        /// </param>
        /// <typeparam name="T">
        /// any class
        /// </typeparam>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<T> PostAsync<T>(T message, string route)
            where T : new()
        {
            var policy = this.policyFactory.PolicyTimeoutGeneric<HttpResponseMessage>(
                TimeSpan.FromMinutes(30),
                i => !i.IsSuccessStatusCode,
                3,
                TimeSpan.FromSeconds(15));

            return await this.PostAsync(message, route, policy);
        }

        /// <summary>
        /// The post async.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="route">
        /// The route.
        /// </param>
        /// <param name="policy">
        /// The policy.
        /// </param>
        /// <typeparam name="T">
        /// Any class
        /// </typeparam>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<T> PostAsync<T>(T message, string route, IPolicy<HttpResponseMessage> policy)
            where T : new()
        {
            this.logger.LogInformation($"about to make a GET request to {route}");

            try
            {
                using (var httpClient = this.httpClientFactory.ClientServiceHttpClient(
                    this.apiClientConfiguration.ClientServiceUrl,
                    this.apiClientConfiguration.SurveillanceUserApiAccessToken))
                {
                    var json = JsonConvert.SerializeObject(message);

                    HttpResponseMessage response = null;
                    await policy.ExecuteAsync(
                        async () =>
                            {
                                this.logger.LogInformation("policy about to call post");
                                response = await httpClient
                                               .PostAsync(
                                                   route,
                                                   new StringContent(json, Encoding.UTF8, "application/json"))
                                               ;
                                this.logger.LogInformation("policy received post response or timed out");
                                return response;
                            })
                        ;

                    if (response == null || !response.IsSuccessStatusCode)
                    {
                        this.logger.LogWarning(
                            $"unsuccessful repository GET request to {route}. {response?.StatusCode}");

                        return new T();
                    }

                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var deserialisedResponse = JsonConvert.DeserializeObject<T>(jsonResponse);

                    if (deserialisedResponse == null)
                    {
                        this.logger.LogError($"was unable to deserialise the response at {route} {jsonResponse}");
                        return new T();
                    }

                    this.logger.LogInformation($"returning deserialised GET response from {route}");

                    return deserialisedResponse;
                }
            }
            catch (Exception e)
            {
                this.logger.LogError(
                    $"Exception in {nameof(BaseClientServiceApi)} {e.Message} {e.InnerException?.Message}");
            }

            return new T();
        }
    }
}