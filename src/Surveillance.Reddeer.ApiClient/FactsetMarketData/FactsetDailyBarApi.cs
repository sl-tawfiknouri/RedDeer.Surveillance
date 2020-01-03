namespace Surveillance.Reddeer.ApiClient.FactsetMarketData
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

    using RedDeer.Contracts.SurveillanceService.Api.FactsetSecurityDaily;

    using Surveillance.Reddeer.ApiClient.Configuration.Interfaces;
    using Surveillance.Reddeer.ApiClient.FactsetMarketData.Interfaces;

    /// <summary>
    /// The factset daily bar.
    /// </summary>
    public class FactsetDailyBarApi : IFactsetDailyBarApi
    {
        /// <summary>
        /// The heartbeat route.
        /// </summary>
        private const string HeartbeatRoute = "api/factset/heartbeat";

        /// <summary>
        /// The route.
        /// </summary>
        private const string Route = "api/factset/surveillance/v1";

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
        private readonly ILogger<FactsetDailyBarApi> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="FactsetDailyBarApi"/> class.
        /// </summary>
        /// <param name="dataLayerConfiguration">
        /// The data layer configuration.
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
        public FactsetDailyBarApi(
            IApiClientConfiguration dataLayerConfiguration,
            IHttpClientFactory httpClientFactory,
            IPolicyFactory policyFactory,
            ILogger<FactsetDailyBarApi> logger)
        {
            this.apiClientConfiguration =
                dataLayerConfiguration ?? throw new ArgumentNullException(nameof(dataLayerConfiguration));
            this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            this.policyFactory = policyFactory ?? throw new ArgumentNullException(nameof(policyFactory));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// The get async.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<FactsetSecurityResponseDto> GetAsync(FactsetSecurityDailyRequest request)
        {
            this.logger.LogInformation("has received a request to get daily bars from the client service");

            if (request == null)
            {
                this.logger.LogInformation("received a null request. Returning an empty response");

                return new FactsetSecurityResponseDto
                           {
                               Request = null, Responses = new FactsetSecurityDailyResponseItem[0]
                           };
            }

            try
            {
                using (var httpClient = this.httpClientFactory.ClientServiceHttpClient(
                    this.apiClientConfiguration.ClientServiceUrl,
                    this.apiClientConfiguration.SurveillanceUserApiAccessToken))
                {
                    var json = JsonConvert.SerializeObject(request);

                    var response =
                        await httpClient
                            .PostAsync(
                                       Route,
                                       new StringContent(json, Encoding.UTF8, "application/json"))
                            ;

                    if (response == null || !response.IsSuccessStatusCode)
                    {
                        this.logger.LogError(
                            $"Unsuccessful factset time bar api repository GET request. {response?.StatusCode}");

                        return new FactsetSecurityResponseDto();
                    }

                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var deserialisedResponse = JsonConvert.DeserializeObject<FactsetSecurityResponseDto>(jsonResponse);

                    if (deserialisedResponse == null)
                    {
                        this.logger.LogError("was unable to deserialise the response");
                        return new FactsetSecurityResponseDto();
                    }

                    this.logger.LogInformation("returning deserialised GET response");
                    return deserialisedResponse;
                }
            }
            catch (Exception e)
            {
                this.logger?.LogError(e, "Get encountered an exception");
            }

            this.logger?.LogInformation("Get received a response from the client. Returning result.");

            return new FactsetSecurityResponseDto();
        }

        /// <summary>
        /// The get with transient fault handling async.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<FactsetSecurityResponseDto> GetWithTransientFaultHandlingAsync(FactsetSecurityDailyRequest request)
        {
            this.logger.LogInformation(
                "GetWithTransientFaultHandling has received a request to get daily bars from the client service");

            if (request == null)
            {
                this.logger.LogInformation(
                    "GetWithTransientFaultHandling received a null request. Returning an empty response");

                return new FactsetSecurityResponseDto
                           {
                               Request = null, Responses = new FactsetSecurityDailyResponseItem[0]
                           };
            }

            using (var httpClient = this.httpClientFactory.ClientServiceHttpClient(
                this.apiClientConfiguration.ClientServiceUrl,
                this.apiClientConfiguration.SurveillanceUserApiAccessToken))
            {
                var json = JsonConvert.SerializeObject(request);
                var policy = this.policyFactory.PolicyTimeoutGeneric<HttpResponseMessage>(
                    TimeSpan.FromMinutes(30),
                    i => !i.IsSuccessStatusCode,
                    5,
                    TimeSpan.FromMinutes(1));

                HttpResponseMessage responseMessage = null;
                await policy
                    .ExecuteAsync(async () =>
                        {
                            responseMessage = 
                                await httpClient
                                    .PostAsync(
                                          Route,
                                          new StringContent(json, Encoding.UTF8, "application/json"))
                                      ;

                            return responseMessage;
                        })
                    ;

                if (responseMessage == null || !responseMessage.IsSuccessStatusCode)
                {
                    this.logger.LogError(
                        $"GetWithTransientFaultHandling was unable to elicit a successful http response {responseMessage?.StatusCode}");
                    return new FactsetSecurityResponseDto();
                }

                var jsonResponse = await responseMessage.Content.ReadAsStringAsync();
                var deserialisedResponse = JsonConvert.DeserializeObject<FactsetSecurityResponseDto>(jsonResponse);

                if (deserialisedResponse == null)
                {
                    this.logger.LogError("GetWithTransientFaultHandling was unable to deserialise the response");
                    return new FactsetSecurityResponseDto();
                }

                this.logger.LogInformation("GetWithTransientFaultHandling returning deserialised GET response");

                return deserialisedResponse;
            }
        }

        /// <summary>
        /// The heart beating async.
        /// </summary>
        /// <param name="token">
        /// The token.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<bool> HeartBeatingAsync(CancellationToken token)
        {
            try
            {
                using (var httpClient = this.httpClientFactory.ClientServiceHttpClient(
                    this.apiClientConfiguration.ClientServiceUrl,
                    this.apiClientConfiguration.SurveillanceUserApiAccessToken))
                {
                    var response = await httpClient.GetAsync(HeartbeatRoute, token);

                    if (!response.IsSuccessStatusCode)
                    {
                        this.logger.LogError("HEARTBEAT FOR FACTSET TIME BAR DATA API REPOSITORY NEGATIVE");
                    }
                    else
                    {
                        this.logger.LogInformation("HEARTBEAT POSITIVE FOR FACTSET TIME BAR API REPOSITORY");
                    }

                    return response.IsSuccessStatusCode;
                }
            }
            catch (Exception e)
            {
                this.logger.LogError(e, "HEARTBEAT FOR FACTSET TIME BAR API REPOSITORY NEGATIVE");
            }

            return false;
        }
    }
}