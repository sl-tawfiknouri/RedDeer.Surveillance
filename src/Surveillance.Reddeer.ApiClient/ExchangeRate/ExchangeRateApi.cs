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

    /// <summary>
    /// The exchange rate.
    /// </summary>
    public class ExchangeRateApi : BaseClientServiceApi, IExchangeRateApi
    {
        /// <summary>
        /// The heartbeat route.
        /// </summary>
        private const string HeartbeatRoute = "api/exchangerates/heartbeat";

        /// <summary>
        /// The route.
        /// </summary>
        private const string Route = "api/exchangerates/get/v1";

        /// <summary>
        /// The client configuration.
        /// </summary>
        private readonly IApiClientConfiguration apiClientConfiguration;

        /// <summary>
        /// The http client factory.
        /// </summary>
        private readonly IHttpClientFactory httpClientFactory;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExchangeRateApi"/> class.
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
        public ExchangeRateApi(
            IApiClientConfiguration apiClientConfiguration,
            IHttpClientFactory httpClientFactory,
            IPolicyFactory policyFactory,
            ILogger<ExchangeRateApi> logger)
            : base(apiClientConfiguration, httpClientFactory, policyFactory, logger)
        {
            this.apiClientConfiguration =
                apiClientConfiguration ?? throw new ArgumentNullException(nameof(apiClientConfiguration));
            this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// The get async.
        /// </summary>
        /// <param name="commencement">
        /// The commencement.
        /// </param>
        /// <param name="termination">
        /// The termination.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<IDictionary<DateTime, IReadOnlyCollection<ExchangeRateDto>>> GetAsync(
            DateTime commencement,
            DateTime termination)
        {
            this.logger.LogInformation($"ExchangeRateApiRepository GET request for date {commencement} to {termination}");

            try
            {
                // US date format as that's default when deserialising on a UK machine as asp.net mvc core
                // If this starts breaking the culture of the machine the client service is on would be worth investigating
                // for posterity this url worked @ 29/09/2018 (http://localhost:8080/api/exchangerates/get/v1?commencement=09/27/2017&termination=09/26/2018)
                var routeWithQString =
                    $"{Route}?commencement={commencement.ToString("MM/dd/yyyy")}&termination={termination.ToString("MM/dd/yyyy")}";

                using (var httpClient = this.httpClientFactory.ClientServiceHttpClient(
                    this.apiClientConfiguration.ClientServiceUrl,
                    this.apiClientConfiguration.SurveillanceUserApiAccessToken))
                {
                    var response = await httpClient.GetAsync(routeWithQString);

                    if (response == null || !response.IsSuccessStatusCode)
                    {
                        this.logger.LogWarning(
                            $"Unsuccessful exchange rate api repository GET request. {response?.StatusCode}");

                        return new Dictionary<DateTime, IReadOnlyCollection<ExchangeRateDto>>();
                    }

                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var deserialisedResponse = JsonConvert.DeserializeObject<ExchangeRateDto[]>(jsonResponse);

                    if (deserialisedResponse == null || !deserialisedResponse.Any())
                    {
                        this.logger.LogWarning(
                            "ExchangeRateApiRepository GET request returned a null or empty response");
                        return new Dictionary<DateTime, IReadOnlyCollection<ExchangeRateDto>>();
                    }

                    var result = deserialisedResponse.GroupBy(dr => dr.DateTime).ToDictionary(
                        i => i.Key,
                        i => i.ToList() as IReadOnlyCollection<ExchangeRateDto>);

                    this.logger.LogInformation("ExchangeRateApiRepository GET request returning results");

                    return result;
                }
            }
            catch (Exception e)
            {
                this.logger.LogError(e, "ExchangeRateApiRepository");
            }

            return new Dictionary<DateTime, IReadOnlyCollection<ExchangeRateDto>>();
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
            return await this.GetHeartbeatAsync(HeartbeatRoute, token);
        }
    }
}