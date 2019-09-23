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

    public class FactsetDailyBarApi : IFactsetDailyBarApi
    {
        private const string HeartbeatRoute = "api/factset/heartbeat";

        private const string Route = "api/factset/surveillance/v1";

        private readonly IApiClientConfiguration _apiClientConfiguration;

        private readonly IHttpClientFactory _httpClientFactory;

        private readonly ILogger<FactsetDailyBarApi> _logger;

        private readonly IPolicyFactory _policyFactory;

        public FactsetDailyBarApi(
            IApiClientConfiguration dataLayerConfiguration,
            IHttpClientFactory httpClientFactory,
            IPolicyFactory policyFactory,
            ILogger<FactsetDailyBarApi> logger)
        {
            this._apiClientConfiguration =
                dataLayerConfiguration ?? throw new ArgumentNullException(nameof(dataLayerConfiguration));
            this._httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            this._policyFactory = policyFactory ?? throw new ArgumentNullException(nameof(policyFactory));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<FactsetSecurityResponseDto> Get(FactsetSecurityDailyRequest request)
        {
            this._logger.LogInformation("has received a request to get daily bars from the client service");

            if (request == null)
            {
                this._logger.LogInformation("received a null request. Returning an empty response");

                return new FactsetSecurityResponseDto
                           {
                               Request = null, Responses = new FactsetSecurityDailyResponseItem[0]
                           };
            }

            try
            {
                using (var httpClient = this._httpClientFactory.ClientServiceHttpClient(
                    this._apiClientConfiguration.ClientServiceUrl,
                    this._apiClientConfiguration.SurveillanceUserApiAccessToken))
                {
                    var json = JsonConvert.SerializeObject(request);
                    var response = await httpClient.PostAsync(
                                       Route,
                                       new StringContent(json, Encoding.UTF8, "application/json"));

                    if (response == null || !response.IsSuccessStatusCode)
                    {
                        this._logger.LogError(
                            $"Unsuccessful factset time bar api repository GET request. {response?.StatusCode}");

                        return new FactsetSecurityResponseDto();
                    }

                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var deserialisedResponse = JsonConvert.DeserializeObject<FactsetSecurityResponseDto>(jsonResponse);

                    if (deserialisedResponse == null)
                    {
                        this._logger.LogError("was unable to deserialise the response");
                        return new FactsetSecurityResponseDto();
                    }

                    this._logger.LogInformation("returning deserialised GET response");
                    return deserialisedResponse;
                }
            }
            catch (Exception e)
            {
                this._logger?.LogError("Get encountered an exception: " + e.Message + " " + e.InnerException?.Message);
            }

            this._logger?.LogInformation("Get received a response from the client. Returning result.");

            return new FactsetSecurityResponseDto();
        }

        public async Task<FactsetSecurityResponseDto> GetWithTransientFaultHandling(FactsetSecurityDailyRequest request)
        {
            this._logger.LogInformation(
                "GetWithTransientFaultHandling has received a request to get daily bars from the client service");

            if (request == null)
            {
                this._logger.LogInformation(
                    "GetWithTransientFaultHandling received a null request. Returning an empty response");

                return new FactsetSecurityResponseDto
                           {
                               Request = null, Responses = new FactsetSecurityDailyResponseItem[0]
                           };
            }

            using (var httpClient = this._httpClientFactory.ClientServiceHttpClient(
                this._apiClientConfiguration.ClientServiceUrl,
                this._apiClientConfiguration.SurveillanceUserApiAccessToken))
            {
                var json = JsonConvert.SerializeObject(request);
                var policy = this._policyFactory.PolicyTimeoutGeneric<HttpResponseMessage>(
                    TimeSpan.FromMinutes(3),
                    i => !i.IsSuccessStatusCode,
                    5,
                    TimeSpan.FromMinutes(1));

                HttpResponseMessage responseMessage = null;
                await policy.ExecuteAsync(
                    async () =>
                        {
                            responseMessage = await httpClient.PostAsync(
                                                  Route,
                                                  new StringContent(json, Encoding.UTF8, "application/json"));

                            return responseMessage;
                        });

                if (responseMessage == null || !responseMessage.IsSuccessStatusCode)
                {
                    this._logger.LogError(
                        $"GetWithTransientFaultHandling was unable to elicit a successful http response {responseMessage?.StatusCode}");
                    return new FactsetSecurityResponseDto();
                }

                var jsonResponse = await responseMessage.Content.ReadAsStringAsync();
                var deserialisedResponse = JsonConvert.DeserializeObject<FactsetSecurityResponseDto>(jsonResponse);

                if (deserialisedResponse == null)
                {
                    this._logger.LogError("GetWithTransientFaultHandling was unable to deserialise the response");
                    return new FactsetSecurityResponseDto();
                }

                this._logger.LogInformation("GetWithTransientFaultHandling returning deserialised GET response");

                return deserialisedResponse;
            }
        }

        public async Task<bool> HeartBeating(CancellationToken token)
        {
            try
            {
                using (var httpClient = this._httpClientFactory.ClientServiceHttpClient(
                    this._apiClientConfiguration.ClientServiceUrl,
                    this._apiClientConfiguration.SurveillanceUserApiAccessToken))
                {
                    var response = await httpClient.GetAsync(HeartbeatRoute, token);

                    if (!response.IsSuccessStatusCode)
                        this._logger.LogError("HEARTBEAT FOR FACTSET TIME BAR DATA API REPOSITORY NEGATIVE");
                    else this._logger.LogInformation("HEARTBEAT POSITIVE FOR FACTSET TIME BAR API REPOSITORY");

                    return response.IsSuccessStatusCode;
                }
            }
            catch (Exception e)
            {
                this._logger.LogError("HEARTBEAT FOR FACTSET TIME BAR API REPOSITORY NEGATIVE", e);
            }

            return false;
        }
    }
}