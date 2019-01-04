using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Firefly.Service.Data.BMLL.Shared.Requests;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Surveillance.DataLayer.Api.BmllMarketData.Interfaces;
using Surveillance.DataLayer.Configuration.Interfaces;

namespace Surveillance.DataLayer.Api.BmllMarketData
{
    public class BmllTimeBarApiRepository : BaseApiRepository, IBmllTimeBarApiRepository
    {
        private const string HeartbeatRoute = "api/bmll/heartbeat";
        private const string Route = "api/bmll/timebars/v1";
        private readonly ILogger<BmllTimeBarApiRepository> _logger;

        public BmllTimeBarApiRepository(
            IDataLayerConfiguration dataLayerConfiguration,
            ILogger<BmllTimeBarApiRepository> logger)
            : base(dataLayerConfiguration)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<GetMinuteBarsResponse> Get(GetMinuteBarsRequest request)
        {
            if (request == null)
            {
                _logger?.LogInformation($"BmllTimeBarApiRepository Get received a null request. Returning an empty response");
                return new GetMinuteBarsResponse();
            }

            _logger?.LogInformation($"BmllTimeBarApiRepository Get received a get minute bars request for {request?.Figi} at {request?.From} to {request?.To}");

            var httpClient = BuildHttpClient();

            try
            {
                var json = JsonConvert.SerializeObject(request);
                var response = await httpClient.PostAsync(Route, new StringContent(json, Encoding.UTF8, "application/json"));

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
                var httpClient = BuildHttpClient();

                var response = await httpClient.GetAsync(HeartbeatRoute, token);

                if (!response.IsSuccessStatusCode)
                    _logger.LogError($"HEARTBEAT FOR BMLL TIME BAR DATA API REPOSITORY NEGATIVE");
                else
                    _logger.LogInformation($"HEARTBEAT POSITIVE FOR TIME BAR API REPOSITORY");

                return response.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                _logger.LogError($"HEARTBEAT FOR TIME BAR API REPOSITORY NEGATIVE", e);
            }

            return false;
        }
    }
}
