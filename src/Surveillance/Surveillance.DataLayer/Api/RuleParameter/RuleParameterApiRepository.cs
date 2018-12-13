using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;
using Surveillance.DataLayer.Api.RuleParameter.Interfaces;
using Surveillance.DataLayer.Configuration.Interfaces;

namespace Surveillance.DataLayer.Api.RuleParameter
{
    public class RuleParameterApiRepository : BaseApiRepository, IRuleParameterApiRepository
    {
        private const string HeartbeatRoute = "api/surveillanceruleparameter/heartbeat";
        private const string Route = "api/surveillanceruleparameter/get/v1";
        private readonly ILogger _logger;

        public RuleParameterApiRepository(
            IDataLayerConfiguration dataLayerConfiguration,
            ILogger<RuleParameterApiRepository> logger) 
            : base(dataLayerConfiguration)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<RuleParameterDto> Get()
        {
            var httpClient = BuildHttpClient();
            _logger.LogInformation($"RuleParameterApiRepository GET request initiating");

            try
            {
                var response = await httpClient.GetAsync(Route);

                if (response == null
                    || !response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"Unsuccessful rule parameter api repository GET request. {response?.StatusCode}");

                    return new RuleParameterDto();
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var deserialisedResponse = JsonConvert.DeserializeObject<RuleParameterDto>(jsonResponse);

                if (deserialisedResponse == null)
                {
                    _logger.LogWarning($"RuleParameterApiRepository has a null deserialised response for GET request");
                }

                _logger.LogInformation($"RuleParameterApiRepository GET request returning response");

                return deserialisedResponse ?? new RuleParameterDto();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }

            return new RuleParameterDto();
        }

        public async Task<bool> HeartBeating(CancellationToken token)
        {
            var client = BuildHttpClient();
            var result = await client.GetAsync(HeartbeatRoute, token);

            if (!result.IsSuccessStatusCode)
                _logger.LogError($"RuleParameterApiRepository HEARTBEAT NEGATIVE");
            else
                _logger.LogInformation($"RuleParameterApiRepository HEARTBEAT POSITIVE");

            return result.IsSuccessStatusCode;
        }
    }
}
