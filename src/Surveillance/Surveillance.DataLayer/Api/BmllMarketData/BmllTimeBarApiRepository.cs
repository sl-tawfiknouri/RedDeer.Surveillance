﻿using System;
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
using Surveillance.DataLayer.Api.BmllMarketData.Interfaces;
using Surveillance.DataLayer.Configuration.Interfaces;

namespace Surveillance.DataLayer.Api.BmllMarketData
{
    public class BmllTimeBarApiRepository : IBmllTimeBarApiRepository
    {
        private const string HeartbeatRoute = "api/bmll/heartbeat";
        private const string RequestsRoute = "api/bmll/requests/v1";
        private const string StatusRoute = "api/bmll/status/v1";
        private const string MinuteBarRoute = "api/bmll/minutebars/v1";

        private readonly IDataLayerConfiguration _dataLayerConfiguration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IPolicyFactory _policyFactory;
        private readonly ILogger<BmllTimeBarApiRepository> _logger;

        public BmllTimeBarApiRepository(
            IDataLayerConfiguration dataLayerConfiguration,
            IHttpClientFactory httpClientFactory,
            IPolicyFactory policyFactory,
            ILogger<BmllTimeBarApiRepository> logger)
        {
            _dataLayerConfiguration = dataLayerConfiguration ?? throw new ArgumentNullException(nameof(dataLayerConfiguration));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _policyFactory = policyFactory ?? throw new ArgumentNullException(nameof(dataLayerConfiguration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task RequestMinuteBars(CreateMinuteBarRequestCommand createCommand)
        {
            if (createCommand == null
                || createCommand.Keys == null
                || !createCommand.Keys.Any())
            {
                _logger.LogError($"BmllTimeBarApiRepository RequestMinuteBars was passed 0 keys to request. Returning early.");
                return;
            }

            _logger.LogInformation($"BmllTimeBarApiRepository RequestMinuteBars received {createCommand.Keys.Count} keys to query BMLL for");

            try
            {
                using (var httpClient = _httpClientFactory.GenericHttpClient(_dataLayerConfiguration.BmllServiceUrl))
                {
                    var json = JsonConvert.SerializeObject(createCommand);
                    var policy = _policyFactory.PolicyTimeoutGeneric<HttpResponseMessage>(TimeSpan.FromMinutes(3), i => !i.IsSuccessStatusCode, 10, TimeSpan.FromMinutes(1));

                    HttpResponseMessage response = null;
                    await policy.ExecuteAsync(async () =>
                    {
                        response = await httpClient.PostAsync(RequestsRoute, new StringContent(json, Encoding.UTF8, "application/json"));

                        return response;
                    });

                    if (response == null
                        || !response.IsSuccessStatusCode)
                    {
                        _logger.LogError($"BmllTimeBarApiRepository RequestMinuteBars Unsuccessful bmll time bar api repository GET request. {response?.StatusCode}");

                        return;
                    }

                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var deserialisedResponse = JsonConvert.DeserializeObject<CreateMinuteBarRequestResponse>(jsonResponse);

                    if (deserialisedResponse == null)
                    {
                        _logger.LogError($"BmllTimeBarApiRepository RequestMinuteBars was unable to deserialise the response");
                        return;
                    }

                    _logger.LogInformation($"BmllTimeBarApiRepository RequestMinuteBars returning deserialised GET response");

                    return;
                }
            }
            catch (Exception e)
            {
                _logger?.LogError($"BmllTimeBarApiRepository RequestMinuteBars encountered an exception: " + e.Message + " " + e.InnerException?.Message);
            }

            _logger.LogInformation($"BmllTimeBarApiRepository RequestMinuteBars completed request for {createCommand.Keys.Count} keys to query BMLL for");
        }

        public async Task<BmllStatusMinuteBarResult> StatusMinuteBars(GetMinuteBarRequestStatusesRequest statusCommand)
        {
            if (statusCommand == null
                || statusCommand.Keys == null
                || !statusCommand.Keys.Any())
            {
                _logger.LogError($"BmllTimeBarApiRepository StatusMinuteBars was passed 0 keys to request. Returning early with success.");
                return BmllStatusMinuteBarResult.Completed;
            }

            _logger.LogInformation($"BmllTimeBarApiRepository StatusMinuteBars received {statusCommand.Keys.Count} keys to query BMLL for");

            try
            {
                using (var httpClient = _httpClientFactory.GenericHttpClient(_dataLayerConfiguration.BmllServiceUrl))
                {
                    var json = JsonConvert.SerializeObject(statusCommand);
                    var policy = _policyFactory.PolicyTimeoutGeneric<HttpResponseMessage>(TimeSpan.FromMinutes(3), i => !i.IsSuccessStatusCode, 3, TimeSpan.FromMinutes(1));

                    HttpResponseMessage response = null;
                    await policy.ExecuteAsync(async () =>
                    {
                        response = await httpClient.PostAsync(StatusRoute, new StringContent(json, Encoding.UTF8, "application/json"));

                        return response;
                    });

                    if (response == null
                        || !response.IsSuccessStatusCode)
                    {
                        _logger.LogError($"BmllTimeBarApiRepository StatusMinuteBars Unsuccessful bmll time bar api repository GET request. {response?.StatusCode}");
                        return BmllStatusMinuteBarResult.InProgress;
                    }

                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var deserialisedResponse = JsonConvert.DeserializeObject<GetMinuteBarRequestStatusesResponse>(jsonResponse);

                    if (deserialisedResponse == null)
                    {
                        _logger.LogError($"BmllTimeBarApiRepository StatusMinuteBars was unable to deserialise the response");
                        return BmllStatusMinuteBarResult.InProgress;
                    }

                    _logger.LogInformation($"BmllTimeBarApiRepository StatusMinuteBars returning deserialised GET response");

                    var acceptedRequests = new[]
                    {
                        MinuteBarRequestStatus.Completed,
                        MinuteBarRequestStatus.Failed,
                        MinuteBarRequestStatus.Requeued // agreed with test team to ignore requeuing until we do further work on the BMLL service
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
                    else
                    {
                        return BmllStatusMinuteBarResult.Completed;
                    }
                }
            }
            catch (Exception e)
            {
                _logger?.LogError($"BmllTimeBarApiRepository StatusMinuteBars encountered an exception: " + e.Message + " " + e.InnerException?.Message);
            }

            _logger.LogInformation($"BmllTimeBarApiRepository StatusMinuteBars completed request for {statusCommand.Keys.Count} keys to query BMLL for");

            return BmllStatusMinuteBarResult.InProgress;
        }

        public async Task<GetMinuteBarsResponse> GetMinuteBars(GetMinuteBarsRequest request)
        {
            if (request == null)
            {
                _logger?.LogInformation($"BmllTimeBarApiRepository Get received a null request. Returning an empty response");
                return new GetMinuteBarsResponse();
            }

            _logger?.LogInformation($"BmllTimeBarApiRepository Get received a get minute bars request for {request?.Figi} at {request?.From} to {request?.To}");

            try
            {
                using (var httpClient = _httpClientFactory.GenericHttpClient(_dataLayerConfiguration.BmllServiceUrl))
                {
                    var json = JsonConvert.SerializeObject(request);
                    var policy = _policyFactory.PolicyTimeoutGeneric<HttpResponseMessage>(TimeSpan.FromMinutes(3), i => !i.IsSuccessStatusCode, 1, TimeSpan.FromSeconds(30));

                    HttpResponseMessage response = null;
                    await policy.ExecuteAsync(async () =>
                    {
                        response = await httpClient.PostAsync(MinuteBarRoute, new StringContent(json, Encoding.UTF8, "application/json"));

                        return response;
                    });

                    if (response == null
                        || !response.IsSuccessStatusCode)
                    {
                        _logger.LogError($"BmllTimeBarApiRepository Unsuccessful bmll time bar api repository GET request. {response?.StatusCode}");

                        return new GetMinuteBarsResponse();
                    }

                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var deserialisedResponse = JsonConvert.DeserializeObject<GetMinuteBarsResponse>(jsonResponse);

                    if (deserialisedResponse == null)
                    {
                        _logger.LogError($"BmllTimeBarApiRepository was unable to deserialise the response");
                        return new GetMinuteBarsResponse();
                    }

                    _logger.LogInformation($"BmllTimeBarApiRepository returning deserialised GET response");

                    return deserialisedResponse;
                }
            }
            catch (Exception e)
            {
                _logger?.LogError($"BmllTimeBarApiRepository Get encountered an exception: " + e.Message + " " + e.InnerException?.Message);
            }
            _logger?.LogInformation($"BmllTimeBarApiRepository Get received a response from the client. Returning result.");

            return new GetMinuteBarsResponse();
        }

        public async Task<bool> HeartBeating(CancellationToken token)
        {
            try
            {
                using (var httpClient = _httpClientFactory.GenericHttpClient(_dataLayerConfiguration.BmllServiceUrl))
                {
                    var response = await httpClient.GetAsync(HeartbeatRoute, token);

                    if (!response.IsSuccessStatusCode)
                        _logger.LogError($"HEARTBEAT FOR BMLL TIME BAR DATA API REPOSITORY NEGATIVE");
                    else
                        _logger.LogInformation($"HEARTBEAT POSITIVE FOR TIME BAR API REPOSITORY");

                    return response.IsSuccessStatusCode;
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"HEARTBEAT FOR TIME BAR API REPOSITORY NEGATIVE", e);
            }

            return false;
        }
    }
}
