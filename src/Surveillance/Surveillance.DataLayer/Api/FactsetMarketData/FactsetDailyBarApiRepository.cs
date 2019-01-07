using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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

        private readonly ILogger<FactsetDailyBarApiRepository> _logger;

        public FactsetDailyBarApiRepository(
            IDataLayerConfiguration dataLayerConfiguration,
            ILogger<FactsetDailyBarApiRepository> logger)
            : base(dataLayerConfiguration)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
