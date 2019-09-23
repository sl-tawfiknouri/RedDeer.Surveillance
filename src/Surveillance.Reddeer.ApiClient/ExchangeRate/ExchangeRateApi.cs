namespace Surveillance.Reddeer.ApiClient.ExchangeRate
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Infrastructure.Network.HttpClient.Interfaces;

    using Microsoft.Extensions.Logging;

    using Newtonsoft.Json;

    using PollyFacade.Policies.Interfaces;

    using RedDeer.Contracts.SurveillanceService.Api.ExchangeRate;

    using Surveillance.Reddeer.ApiClient.Configuration.Interfaces;
    using Surveillance.Reddeer.ApiClient.ExchangeRate.Interfaces;

    public class ExchangeRateApi : BaseClientServiceApi, IExchangeRateApi
    {
        private const string HeartbeatRoute = "api/exchangerates/heartbeat";

        private const string Route = "api/exchangerates/get/v1";

        private readonly IApiClientConfiguration _apiClientConfiguration;

        private readonly IHttpClientFactory _httpClientFactory;

        private readonly ILogger _logger;

        public ExchangeRateApi(
            IApiClientConfiguration apiClientConfiguration,
            IHttpClientFactory httpClientFactory,
            IPolicyFactory policyFactory,
            ILogger<ExchangeRateApi> logger)
            : base(apiClientConfiguration, httpClientFactory, policyFactory, logger)
        {
            this._apiClientConfiguration =
                apiClientConfiguration ?? throw new ArgumentNullException(nameof(apiClientConfiguration));
            this._httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IDictionary<DateTime, IReadOnlyCollection<ExchangeRateDto>>> Get(
            DateTime commencement,
            DateTime termination)
        {
            this._logger.LogInformation(
                $"ExchangeRateApiRepository GET request for date {commencement} to {termination}");

            try
            {
                // US date format as that's default when deserialising on a UK machine as asp.net mvc core
                // If this starts breaking the culture of the machine the client service is on would be worth investigating
                // for posterity this url worked @ 29/09/2018 (http://localhost:8080/api/exchangerates/get/v1?commencement=09/27/2017&termination=09/26/2018)
                var routeWithQString =
                    $"{Route}?commencement={commencement.ToString("MM/dd/yyyy")}&termination={termination.ToString("MM/dd/yyyy")}";

                using (var httpClient = this._httpClientFactory.ClientServiceHttpClient(
                    this._apiClientConfiguration.ClientServiceUrl,
                    this._apiClientConfiguration.SurveillanceUserApiAccessToken))
                {
                    var response = await httpClient.GetAsync(routeWithQString);

                    if (response == null || !response.IsSuccessStatusCode)
                    {
                        this._logger.LogWarning(
                            $"Unsuccessful exchange rate api repository GET request. {response?.StatusCode}");

                        return new Dictionary<DateTime, IReadOnlyCollection<ExchangeRateDto>>();
                    }

                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var deserialisedResponse = JsonConvert.DeserializeObject<ExchangeRateDto[]>(jsonResponse);

                    if (deserialisedResponse == null || !deserialisedResponse.Any())
                    {
                        this._logger.LogWarning(
                            "ExchangeRateApiRepository GET request returned a null or empty response");
                        return new Dictionary<DateTime, IReadOnlyCollection<ExchangeRateDto>>();
                    }

                    var result = deserialisedResponse.GroupBy(dr => dr.DateTime).ToDictionary(
                        i => i.Key,
                        i => i.ToList() as IReadOnlyCollection<ExchangeRateDto>);

                    this._logger.LogInformation("ExchangeRateApiRepository GET request returning results");

                    return result;
                }
            }
            catch (Exception e)
            {
                this._logger.LogError("ExchangeRateApiRepository: " + e.Message);
            }

            return new Dictionary<DateTime, IReadOnlyCollection<ExchangeRateDto>>();
        }

        public async Task<bool> HeartBeating(CancellationToken token)
        {
            return await this.GetHeartbeat(HeartbeatRoute, token);
        }
    }
}