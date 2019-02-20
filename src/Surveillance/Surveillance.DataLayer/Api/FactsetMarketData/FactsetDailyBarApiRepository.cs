using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DataSynchroniser.Api.Policies.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RedDeer.Contracts.SurveillanceService.Api.FactsetSecurityDaily;
using Surveillance.DataLayer.Api.FactsetMarketData.Interfaces;
using Surveillance.DataLayer.Configuration.Interfaces;

namespace Surveillance.DataLayer.Api.FactsetMarketData
{
    public class FactsetDailyBarApiRepository : BaseApiRepository, IFactsetDailyBarApiRepository
    {
        private const string HeartbeatRoute = "api/factset/heartbeat";
        private const string Route = "api/factset/surveillance/v1";

        private readonly IPolicyFactory _policyFactory;
        private readonly ILogger<FactsetDailyBarApiRepository> _logger;

        public FactsetDailyBarApiRepository(
            IDataLayerConfiguration dataLayerConfiguration,
            IPolicyFactory policyFactory,
            ILogger<FactsetDailyBarApiRepository> logger)
            : base(dataLayerConfiguration, logger)
        {
            _policyFactory = policyFactory ?? throw new ArgumentNullException(nameof(policyFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<FactsetSecurityResponseDto> GetWithTransientFaultHandling(FactsetSecurityDailyRequest request)
        {
            _logger.LogInformation($"FactsetDailyBarApiRepository GetWithTransientFaultHandling has received a request to get daily bars from the client service");

            if (request == null)
            {
                _logger.LogInformation($"FactsetDailyBarApiRepository GetWithTransientFaultHandling received a null request. Returning an empty response");

                return new FactsetSecurityResponseDto
                {
                    Request = null,
                    Responses = new FactsetSecurityDailyResponseItem[0]

                };
            }

            var httpClient = BuildHttpClient();
            var json = JsonConvert.SerializeObject(request);
            var policy = _policyFactory.PolicyTimeoutGeneric<HttpResponseMessage>(TimeSpan.FromMinutes(3), i => !i.IsSuccessStatusCode, 5, TimeSpan.FromMinutes(1));

            HttpResponseMessage responseMessage = null;
            await policy.ExecuteAsync(async () =>
            {
                responseMessage = await httpClient.PostAsync(Route, new StringContent(json, Encoding.UTF8, "application/json"));

                return responseMessage;
            });

            if (responseMessage == null
                || !responseMessage.IsSuccessStatusCode)
            {
                _logger.LogError($"FactsetDailyBarApiRepository GetWithTransientFaultHandling was unable to elicit a successful http response {responseMessage?.StatusCode}");
                return new FactsetSecurityResponseDto();
            }

            var jsonResponse = await responseMessage.Content.ReadAsStringAsync();
            var deserialisedResponse = JsonConvert.DeserializeObject<FactsetSecurityResponseDto>(jsonResponse);

            if (deserialisedResponse == null)
            {
                _logger.LogError($"FactsetDailyBarApiRepository GetWithTransientFaultHandling was unable to deserialise the response");
                return new FactsetSecurityResponseDto();
            }

            _logger.LogInformation($"FactsetDailyBarApiRepository GetWithTransientFaultHandling returning deserialised GET response");
            return deserialisedResponse;
        }

        public async Task<FactsetSecurityResponseDto> Get(FactsetSecurityDailyRequest request)
        {
            _logger.LogInformation($"FactsetDailyBarApiRepository has received a request to get daily bars from the client service");

            if (request == null)
            {
                _logger.LogInformation($"FactsetDailyBarApiRepository received a null request. Returning an empty response");

                return new FactsetSecurityResponseDto
                {
                    Request = null,
                    Responses = new FactsetSecurityDailyResponseItem[0]

                };
            }

            var httpClient = BuildHttpClient();

            try
            {
                var json = JsonConvert.SerializeObject(request);
                var response = await httpClient.PostAsync(Route, new StringContent(json, Encoding.UTF8, "application/json"));

                if (response == null
                    || !response.IsSuccessStatusCode)
                {
                    _logger.LogError($"FactsetDailyBarApiRepository Unsuccessful factset time bar api repository GET request. {response?.StatusCode}");

                    return new FactsetSecurityResponseDto();
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var deserialisedResponse = JsonConvert.DeserializeObject<FactsetSecurityResponseDto>(jsonResponse);

                if (deserialisedResponse == null)
                {
                    _logger.LogError($"FactsetDailyBarApiRepository was unable to deserialise the response");
                    return new FactsetSecurityResponseDto();
                }

                _logger.LogInformation($"FactsetDailyBarApiRepository returning deserialised GET response");
                return deserialisedResponse;
            }
            catch (Exception e)
            {
                _logger?.LogError($"FactsetDailyBarApiRepository Get encountered an exception: " + e.Message + " " + e.InnerException?.Message);
            }
            _logger?.LogInformation($"FactsetDailyBarApiRepository Get received a response from the client. Returning result.");

            return new FactsetSecurityResponseDto();
        }

        public async Task<bool> HeartBeating(CancellationToken token)
        {
            try
            {
                var httpClient = BuildHttpClient();

                var response = await httpClient.GetAsync(HeartbeatRoute, token);

                if (!response.IsSuccessStatusCode)
                    _logger.LogError($"HEARTBEAT FOR FACTSET TIME BAR DATA API REPOSITORY NEGATIVE");
                else
                    _logger.LogInformation($"HEARTBEAT POSITIVE FOR FACTSET TIME BAR API REPOSITORY");

                return response.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                _logger.LogError($"HEARTBEAT FOR FACTSET TIME BAR API REPOSITORY NEGATIVE", e);
            }

            return false;
        }
    }
}
