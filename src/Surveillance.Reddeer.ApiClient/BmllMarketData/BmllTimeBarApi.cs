namespace Surveillance.Reddeer.ApiClient.BmllMarketData
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using Firefly.Service.Data.BMLL.Shared.Commands;
    using Firefly.Service.Data.BMLL.Shared.Emuns;
    using Firefly.Service.Data.BMLL.Shared.Requests;

    using Infrastructure.Network.HttpClient.Interfaces;

    using Microsoft.Extensions.Logging;

    using Newtonsoft.Json;

    using PollyFacade.Policies.Interfaces;

    using Surveillance.Reddeer.ApiClient.BmllMarketData.Interfaces;
    using Surveillance.Reddeer.ApiClient.Configuration.Interfaces;

    /// <summary>
    /// The time bar client.
    /// </summary>
    public class BmllTimeBarApi : IBmllTimeBarApi
    {
        /// <summary>
        /// The heartbeat route.
        /// </summary>
        private const string HeartbeatRoute = "api/bmll/heartbeat";

        /// <summary>
        /// The minute bar route.
        /// </summary>
        private const string MinuteBarRoute = "api/bmll/minutebars/v1";

        /// <summary>
        /// The requests route.
        /// </summary>
        private const string RequestsRoute = "api/bmll/requests/v1";

        /// <summary>
        /// The status route.
        /// </summary>
        private const string StatusRoute = "api/bmll/status/v1";

        /// <summary>
        /// The application programming interface client configuration.
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
        private readonly ILogger<BmllTimeBarApi> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="BmllTimeBarApi"/> class.
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
        public BmllTimeBarApi(
            IApiClientConfiguration dataLayerConfiguration,
            IHttpClientFactory httpClientFactory,
            IPolicyFactory policyFactory,
            ILogger<BmllTimeBarApi> logger)
        {
            this.apiClientConfiguration = dataLayerConfiguration ?? throw new ArgumentNullException(nameof(dataLayerConfiguration));
            this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            this.policyFactory = policyFactory ?? throw new ArgumentNullException(nameof(dataLayerConfiguration));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// The get minute bars async.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<GetMinuteBarsResponse> GetMinuteBarsAsync(GetMinuteBarsRequest request)
        {
            if (request == null)
            {
                this.logger?.LogInformation(
                    "BmllTimeBarApiRepository Get received a null request. Returning an empty response");
                return new GetMinuteBarsResponse();
            }

            this.logger?.LogInformation(
                $"BmllTimeBarApiRepository Get received a get minute bars request for {request?.Figi} at {request?.From} to {request?.To}");

            try
            {
                using (var httpClient =
                    this.httpClientFactory.GenericHttpClient(this.apiClientConfiguration.BmllServiceUrl))
                {
                    var json = JsonConvert.SerializeObject(request);
                    var policy = this.policyFactory.PolicyTimeoutGeneric<HttpResponseMessage>(
                        TimeSpan.FromMinutes(3),
                        i => !i.IsSuccessStatusCode,
                        1,
                        TimeSpan.FromSeconds(30));

                    HttpResponseMessage response = null;
                    await policy
                        .ExecuteAsync(async () =>
                            {
                                response = await httpClient.PostAsync(
                                               MinuteBarRoute,
                                               new StringContent(json, Encoding.UTF8, "application/json"));
                                this.logger.LogInformation("GetMinuteBars policy received post response or timed out");
                                return response;
                            })
                        .ConfigureAwait(false);

                    if (response == null || !response.IsSuccessStatusCode)
                    {
                        this.logger.LogError(
                            $"BmllTimeBarApiRepository Unsuccessful bmll time bar api repository GET request. {response?.StatusCode}");

                        return new GetMinuteBarsResponse();
                    }

                    var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var deserialisedResponse = JsonConvert.DeserializeObject<GetMinuteBarsResponse>(jsonResponse);

                    if (deserialisedResponse == null)
                    {
                        this.logger.LogError("BmllTimeBarApiRepository was unable to deserialise the response");
                        return new GetMinuteBarsResponse();
                    }

                    this.logger.LogInformation("BmllTimeBarApiRepository returning deserialised GET response");

                    return deserialisedResponse;
                }
            }
            catch (Exception e)
            {
                this.logger?.LogError(
                    "BmllTimeBarApiRepository Get encountered an exception: " + e.Message + " "
                    + e.InnerException?.Message);
            }

            this.logger?.LogInformation(
                "BmllTimeBarApiRepository Get received a response from the client. Returning result.");

            return new GetMinuteBarsResponse();
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
                using (var httpClient =
                    this.httpClientFactory.GenericHttpClient(this.apiClientConfiguration.BmllServiceUrl))
                {
                    var response = await httpClient.GetAsync(HeartbeatRoute, token);

                    if (!response.IsSuccessStatusCode)
                    {
                        this.logger.LogError("HEARTBEAT FOR BMLL TIME BAR DATA API REPOSITORY NEGATIVE");
                    }
                    else
                    {
                        this.logger.LogInformation("HEARTBEAT POSITIVE FOR TIME BAR API REPOSITORY");
                    }

                    return response.IsSuccessStatusCode;
                }
            }
            catch (Exception e)
            {
                this.logger.LogError("HEARTBEAT FOR TIME BAR API REPOSITORY NEGATIVE", e);
            }

            return false;
        }

        /// <summary>
        /// The request minute bars async.
        /// </summary>
        /// <param name="createCommand">
        /// The create command.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task RequestMinuteBarsAsync(CreateMinuteBarRequestCommand createCommand)
        {
            if (createCommand == null || createCommand.Keys == null || !createCommand.Keys.Any())
            {
                this.logger.LogError("BmllTimeBarApiRepository RequestMinuteBars was passed 0 keys to request. Returning early.");

                return;
            }

            this.logger.LogInformation($"BmllTimeBarApiRepository RequestMinuteBars received {createCommand.Keys.Count} keys to query BMLL for");

            try
            {
                using (var httpClient =
                    this.httpClientFactory.GenericHttpClient(this.apiClientConfiguration.BmllServiceUrl))
                {
                    var json = JsonConvert.SerializeObject(createCommand);
                    var policy = this.policyFactory.PolicyTimeoutGeneric<HttpResponseMessage>(
                        TimeSpan.FromMinutes(3),
                        i => !i.IsSuccessStatusCode,
                        10,
                        TimeSpan.FromMinutes(1));

                    HttpResponseMessage response = null;
                    await policy
                        .ExecuteAsync(async () =>
                        {
                            response = await httpClient.PostAsync(
                                           RequestsRoute,
                                           new StringContent(json, Encoding.UTF8, "application/json"));
                            this.logger.LogInformation(
                                "RequestMinuteBars policy received post response or timed out");
                            return response;
                        })
                        .ConfigureAwait(false);

                    if (response == null || !response.IsSuccessStatusCode)
                    {
                        this.logger.LogError(
                            $"BmllTimeBarApiRepository RequestMinuteBars Unsuccessful bmll time bar api repository GET request. {response?.StatusCode}");

                        return;
                    }

                    var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var deserialisedResponse = JsonConvert.DeserializeObject<CreateMinuteBarRequestResponse>(jsonResponse);

                    if (deserialisedResponse == null)
                    {
                        this.logger.LogError("BmllTimeBarApiRepository RequestMinuteBars was unable to deserialise the response");

                        return;
                    }

                    this.logger.LogInformation("BmllTimeBarApiRepository RequestMinuteBars returning deserialised GET response");

                    return;
                }
            }
            catch (Exception e)
            {
                this.logger?.LogError(
                    "BmllTimeBarApiRepository RequestMinuteBars encountered an exception: " + e.Message + " "
                    + e.InnerException?.Message);
            }

            this.logger.LogInformation(
                $"BmllTimeBarApiRepository RequestMinuteBars completed request for {createCommand.Keys.Count} keys to query BMLL for");
        }

        /// <summary>
        /// The status minute bars async.
        /// </summary>
        /// <param name="statusCommand">
        /// The status command.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<BmllStatusMinuteBarResult> StatusMinuteBarsAsync(GetMinuteBarRequestStatusesRequest statusCommand)
        {
            if (statusCommand == null || statusCommand.Keys == null || !statusCommand.Keys.Any())
            {
                this.logger.LogError(
                    "BmllTimeBarApiRepository StatusMinuteBars was passed 0 keys to request. Returning early with success.");
                return BmllStatusMinuteBarResult.Completed;
            }

            this.logger.LogInformation(
                $"BmllTimeBarApiRepository StatusMinuteBars received {statusCommand.Keys.Count} keys to query BMLL for");

            try
            {
                using (var httpClient =
                    this.httpClientFactory.GenericHttpClient(this.apiClientConfiguration.BmllServiceUrl))
                {
                    var json = JsonConvert.SerializeObject(statusCommand);
                    var policy = this.policyFactory.PolicyTimeoutGeneric<HttpResponseMessage>(
                        TimeSpan.FromMinutes(3),
                        i => !i.IsSuccessStatusCode,
                        3,
                        TimeSpan.FromMinutes(1));

                    HttpResponseMessage response = null;
                    await policy.ExecuteAsync(
                        async () =>
                            {
                                response = await httpClient.PostAsync(
                                               StatusRoute,
                                               new StringContent(json, Encoding.UTF8, "application/json"));
                                this.logger.LogInformation(
                                    "StatusMinuteBars policy received post response or timed out");
                                return response;
                            }).ConfigureAwait(false);

                    if (response == null || !response.IsSuccessStatusCode)
                    {
                        this.logger.LogError(
                            $"BmllTimeBarApiRepository StatusMinuteBars Unsuccessful bmll time bar api repository GET request. {response?.StatusCode}");
                        return BmllStatusMinuteBarResult.InProgress;
                    }

                    var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var deserialisedResponse =
                        JsonConvert.DeserializeObject<GetMinuteBarRequestStatusesResponse>(jsonResponse);

                    if (deserialisedResponse == null)
                    {
                        this.logger.LogError(
                            "BmllTimeBarApiRepository StatusMinuteBars was unable to deserialise the response");
                        return BmllStatusMinuteBarResult.InProgress;
                    }

                    this.logger.LogInformation(
                        "BmllTimeBarApiRepository StatusMinuteBars returning deserialised GET response");

                    var acceptedRequests = new[]
                       {
                           MinuteBarRequestStatus.Completed, MinuteBarRequestStatus.Failed,
                           MinuteBarRequestStatus.NotFound,
                           MinuteBarRequestStatus
                               .Requeued // agreed with test team to ignore requeuing until we do further work on the BMLL service
                       };

                    var completed = deserialisedResponse.Statuses.All(i => acceptedRequests.Contains(i.Status));

                    if (!completed)
                    {
                        return BmllStatusMinuteBarResult.InProgress;
                    }

                    if (deserialisedResponse.Statuses.Any(i => i.Status == MinuteBarRequestStatus.Failed))
                    {
                        return BmllStatusMinuteBarResult.CompletedWithFailures;
                    }

                    return BmllStatusMinuteBarResult.Completed;
                }
            }
            catch (Exception e)
            {
                this.logger?.LogError(
                    "BmllTimeBarApiRepository StatusMinuteBars encountered an exception: " + e.Message + " "
                    + e.InnerException?.Message);
            }

            this.logger.LogInformation(
                $"BmllTimeBarApiRepository StatusMinuteBars completed request for {statusCommand.Keys.Count} keys to query BMLL for");

            return BmllStatusMinuteBarResult.InProgress;
        }
    }
}